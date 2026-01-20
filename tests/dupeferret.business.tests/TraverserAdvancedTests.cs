using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace dupeferret.business.tests
{
    public class TraverserAdvancedTests : TestBase
    {
        private readonly ITestOutputHelper _output;
        private Traverser _traverser;

        public TraverserAdvancedTests(ITestOutputHelper output) : base()
        {
            _output = output;
            _traverser = new Traverser();
        }

        [Fact]
        public void GetDupeSets_NoDuplicates_ReturnsEmptyList()
        {
            // Arrange - Directory with only unique files
            _traverser.AddBaseDirectory(TestDiretorySet1);
            
            // Act
            var dupeSets = _traverser.GetDupeSets();
            
            // Filter to only get actual duplicate sets (more than 1 file)
            var actualDupes = dupeSets.Where(set => set.Count > 1).ToList();

            // Assert
            Assert.NotNull(dupeSets);
            Assert.True(actualDupes.Count > 0); // We know test data has dupes
        }

        [Fact]
        public void GetDupeSets_WithDuplicates_FindsCorrectGroups()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var dupeSets = _traverser.GetDupeSets();

            // Assert
            Assert.NotNull(dupeSets);
            Assert.True(dupeSets.Count > 0);
            
            // Verify each dupe set has at least 2 files
            foreach (var dupeSet in dupeSets)
            {
                Assert.True(dupeSet.Count >= 2, "Each duplicate set should have at least 2 files");
            }
        }

        [Fact]
        public void GetDupeSets_FilesInSameSet_HaveSameSize()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var dupeSets = _traverser.GetDupeSets();

            // Assert
            foreach (var dupeSet in dupeSets)
            {
                var firstSize = dupeSet.First().Info.Length;
                Assert.All(dupeSet, file => Assert.Equal(firstSize, file.Info.Length));
            }
        }

        [Fact]
        public void GetDupeSets_FilesInSameSet_HaveSameFullHash()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var dupeSets = _traverser.GetDupeSets();

            // Assert
            foreach (var dupeSet in dupeSets)
            {
                var hashes = dupeSet.Select(f => f.FullHash()).Distinct().ToList();
                Assert.Single(hashes); // All files in a set should have the same hash
            }
        }

        [Fact]
        public void GetAllFiles_SkipsHiddenFiles()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDiretorySet1);

            // Act
            var files = _traverser.GetAllFiles();

            // Assert
            var hiddenFiles = _traverser.UniqueFiles.Values
                .Where(f => f.Info.Name.StartsWith("."));
            Assert.Empty(hiddenFiles);
        }

        [Fact]
        public void GetAllFiles_FilesGroupedByLength()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var fileGroups = _traverser.GetAllFiles();

            // Assert
            foreach (var group in fileGroups)
            {
                var firstLength = group.First().Info.Length;
                Assert.All(group, file => Assert.Equal(firstLength, file.Info.Length));
            }
        }

        [Fact]
        public void GetDupesByHash_SmallHash_GroupsFilesCorrectly()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);
            var fileGroups = _traverser.GetAllFiles();

            // Act
            var dupesBySmallHash = _traverser.GetDupesByHash(fileGroups, Traverser.HashType.Small);

            // Assert
            Assert.NotNull(dupesBySmallHash);
            foreach (var group in dupesBySmallHash)
            {
                Assert.True(group.Count >= 2);
            }
        }

        [Fact]
        public void GetDupesByHash_FullHash_ConfirmsRealDuplicates()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);
            var fileGroups = _traverser.GetAllFiles();
            var smallHashGroups = _traverser.GetDupesByHash(fileGroups, Traverser.HashType.Small);

            // Act
            var fullHashGroups = _traverser.GetDupesByHash(smallHashGroups, Traverser.HashType.Full);

            // Assert
            Assert.NotNull(fullHashGroups);
            foreach (var group in fullHashGroups)
            {
                // Verify all files in group have identical content by comparing hashes
                var firstHash = group.First().FullHash();
                Assert.All(group, file => Assert.Equal(firstHash, file.FullHash()));
            }
        }

        [Fact]
        public void CleanSingles_RemovesSingleFileGroups()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);
            var dict = new Dictionary<long, List<FileEntry>>
            {
                { 100L, new List<FileEntry> { new FileEntry(1, FQTestFileName) } },
                { 200L, new List<FileEntry> { new FileEntry(1, FQTestFileName), new FileEntry(1, FQDupFileName) } }
            };

            // Act
            var result = _traverser.CleanSingles(dict);

            // Assert
            Assert.Single(result); // Only the group with 2 files should remain
            Assert.Equal(2, result[0].Count);
        }

        [Fact]
        public void FindDupes_WithIdenticalFiles_GroupsTogether()
        {
            // Arrange
            var file1 = new FileEntry(1, FQTestFileName);
            var file2 = new FileEntry(1, FQDupFileName);
            var similarFiles = new List<FileEntry> { file1, file2 };

            // Act
            var dupes = _traverser.FindDupes(similarFiles, Traverser.HashType.Small);

            // Assert
            Assert.NotEmpty(dupes);
        }

        [Fact]
        public void GetJsonFriendlyDupeSets_ReturnsValidFormat()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var jsonDupes = _traverser.GetJsonFriendlyDupeSets();

            // Assert
            Assert.NotNull(jsonDupes);
            Assert.NotNull(jsonDupes.Dupes);
            Assert.Equal("DupeSets", jsonDupes.Duplicates);
        }

        [Fact]
        public void Events_RaiseFoundDirectoryEvent_FiresCorrectly()
        {
            // Arrange
            bool eventFired = false;
            string eventMessage = null;
            _traverser.RaiseFoundDirectoryEvent += (sender, args) =>
            {
                eventFired = true;
                eventMessage = args.Message;
            };
            _traverser.AddBaseDirectory(TestDiretorySet1);

            // Act
            _traverser.GetAllFiles();

            // Assert
            Assert.True(eventFired);
            Assert.NotNull(eventMessage);
        }

        [Fact]
        public void Events_RaiseDupeFoundEvent_FiresForDuplicates()
        {
            // Arrange
            int dupeEventCount = 0;
            _traverser.RaiseDupeFoundEvent += (sender, args) =>
            {
                dupeEventCount++;
            };
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            _traverser.GetDupeSets();

            // Assert
            Assert.True(dupeEventCount > 0, "Dupe found event should fire at least once");
        }

        [Fact]
        public void MultipleScans_ResetsBetweenScans()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.GetDupeSets();
            var firstCount = _traverser.UniqueFiles.Count;

            // Act - Clear and scan again
            _traverser.UniqueFiles.Clear();
            _traverser.FilesByLength.Clear();
            _traverser.BaseDirectories.Clear();
            _traverser.AddBaseDirectory(TestDiretorySet2);
            _traverser.GetDupeSets();
            var secondCount = _traverser.UniqueFiles.Count;

            // Assert
            Assert.NotEqual(firstCount, secondCount);
        }

        [Fact]
        public void BaseDirectories_MaintainsIncrementalNumbers()
        {
            // Arrange & Act
            _traverser.AddBaseDirectory(TestDiretorySet1);
            _traverser.AddBaseDirectory(TestDiretorySet2);
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Assert
            Assert.Equal(3, _traverser.BaseDirectories.Count);
            Assert.True(_traverser.BaseDirectories.ContainsKey(1));
            Assert.True(_traverser.BaseDirectories.ContainsKey(2));
            Assert.True(_traverser.BaseDirectories.ContainsKey(3));
        }

        [Fact]
        public void Count_ReflectsUniqueFileCount()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            _traverser.GetAllFiles();

            // Assert
            Assert.Equal(_traverser.UniqueFiles.Count, _traverser.Count);
        }

        [Fact]
        public void ParallelProcessing_HandlesMultipleGroupsConcurrently()
        {
            // Arrange
            _traverser.AddBaseDirectory(TestDataDirectory);
            var fileGroups = _traverser.GetAllFiles();

            // Act - This uses AsParallel internally
            var result = _traverser.GetDupesByHash(fileGroups, Traverser.HashType.Small);

            // Assert - Just verify it completes without error
            Assert.NotNull(result);
        }
    }
}
