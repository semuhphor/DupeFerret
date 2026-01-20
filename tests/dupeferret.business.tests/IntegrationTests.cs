using System;
using System.IO;
using System.Linq;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class IntegrationTests : TestBase
    {
        [Fact]
        public void EndToEnd_FindDuplicates_CompleteWorkflow()
        {
            // Arrange
            var traverser = new Traverser();
            int directoriesFound = 0;
            int duplicatesFound = 0;

            traverser.RaiseFoundDirectoryEvent += (s, e) => directoriesFound++;
            traverser.RaiseDupeFoundEvent += (s, e) => duplicatesFound++;

            // Act
            traverser.AddBaseDirectory(TestDataDirectory);
            var dupeSets = traverser.GetDupeSets();

            // Assert
            Assert.True(traverser.UniqueFiles.Count > 0, "Should find files");
            Assert.True(directoriesFound > 0, "Should traverse directories");
            Assert.True(dupeSets.Count > 0, "Should find duplicate sets");
            Assert.True(duplicatesFound > 0, "Should fire duplicate events");

            // Verify all duplicate sets have matching content
            foreach (var set in dupeSets)
            {
                var firstHash = set[0].FullHash();
                foreach (var file in set)
                {
                    Assert.Equal(firstHash, file.FullHash());
                }
            }
        }

        [Fact]
        public void EndToEnd_JsonOutput_ProducesValidStructure()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            var jsonDupes = traverser.GetJsonFriendlyDupeSets();

            // Assert
            Assert.NotNull(jsonDupes);
            Assert.NotNull(jsonDupes.Dupes);
            Assert.Equal("DupeSets", jsonDupes.Duplicates);
            
            foreach (var dupeSet in jsonDupes.Dupes)
            {
                Assert.True(dupeSet.Count >= 2);
                foreach (var file in dupeSet)
                {
                    Assert.NotNull(file.FQFN);
                    Assert.NotNull(file.Name);
                    Assert.True(file.Length > 0);
                }
            }
        }

        [Fact]
        public void EndToEnd_MultipleDirectories_CombinesResults()
        {
            // Arrange
            var traverser = new Traverser();

            // Act
            traverser.AddBaseDirectory(TestDiretorySet1);
            traverser.AddBaseDirectory(TestDiretorySet2);
            var dupeSets = traverser.GetDupeSets();

            // Assert
            var totalFiles = traverser.UniqueFiles.Count;
            Assert.True(totalFiles > 0);
            Assert.Equal(2, traverser.BaseDirectories.Count);

            // Verify files from both directories are included
            var filesFromSet1 = traverser.UniqueFiles.Values
                .Count(f => f.FQFN.Contains("Set1"));
            var filesFromSet2 = traverser.UniqueFiles.Values
                .Count(f => f.FQFN.Contains("Set2"));

            Assert.True(filesFromSet1 > 0);
            Assert.True(filesFromSet2 > 0);
        }

        [Fact]
        public void EndToEnd_DuplicateDetection_ThreeStageProcess()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDataDirectory);

            // Act - Stage 1: Get files grouped by size
            var stage1 = traverser.GetAllFiles();
            Assert.NotNull(stage1);

            // Act - Stage 2: Get groups by first hash
            var stage2 = traverser.GetDupesByHash(stage1, Traverser.HashType.Small);
            Assert.NotNull(stage2);

            // Act - Stage 3: Get groups by full hash
            var stage3 = traverser.GetDupesByHash(stage2, Traverser.HashType.Full);
            Assert.NotNull(stage3);

            // Assert - Each stage should reduce or maintain the number of groups
            Assert.True(stage3.Count <= stage2.Count, "Full hash should not increase groups");
        }

        [Fact]
        public void EndToEnd_ErrorHandling_ContinuesOnError()
        {
            // Arrange
            var traverser = new Traverser();

            // Act - Add valid directory
            traverser.AddBaseDirectory(TestDiretorySet1);
            
            // Try to add invalid directory (should throw)
            Assert.Throws<DirectoryNotFoundException>(() => 
                traverser.AddBaseDirectory(@"C:\NonExistent\Path\12345"));

            // Continue with valid scan
            var dupeSets = traverser.GetDupeSets();

            // Assert - Should still work with valid directory
            Assert.True(traverser.UniqueFiles.Count > 0);
        }

        [Fact]
        public void EndToEnd_FileEntry_ImplementsIComparable()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDataDirectory);
            var dupeSets = traverser.GetDupeSets();

            // Act - Sort files using IComparable
            foreach (var set in dupeSets)
            {
                var sorted = set.OrderBy(f => f).ToList();
                
                // Assert - Should be sorted by creation time, then last write, then path
                for (int i = 1; i < sorted.Count; i++)
                {
                    var comparison = sorted[i - 1].CompareTo(sorted[i]);
                    Assert.True(comparison <= 0, "Files should be in sorted order");
                }
            }
        }

        [Fact]
        public void EndToEnd_Events_FireInCorrectOrder()
        {
            // Arrange
            var traverser = new Traverser();
            var eventLog = new System.Collections.Generic.List<string>();

            traverser.RaiseFoundDirectoryEvent += (s, e) => 
                eventLog.Add($"DIR:{e.Message}");
            traverser.RaiseDupeFoundEvent += (s, e) => 
                eventLog.Add("DUPE");

            // Act
            traverser.AddBaseDirectory(TestDataDirectory);
            traverser.GetDupeSets();

            // Assert
            Assert.NotEmpty(eventLog);
            
            // Directory events should come before duplicate events
            var firstDupeIndex = eventLog.FindIndex(e => e == "DUPE");
            if (firstDupeIndex > 0)
            {
                var directoriesBeforeDupes = eventLog
                    .Take(firstDupeIndex)
                    .Count(e => e.StartsWith("DIR:"));
                Assert.True(directoriesBeforeDupes > 0);
            }
        }

        [Fact]
        public void EndToEnd_ParallelProcessing_ProducesSameResults()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDataDirectory);

            // Act - Run twice to ensure deterministic results
            var dupeSets1 = traverser.GetDupeSets();
            
            // Reset
            traverser.UniqueFiles.Clear();
            traverser.FilesByLength.Clear();
            traverser.BaseDirectories.Clear();
            traverser.AddBaseDirectory(TestDataDirectory);
            
            var dupeSets2 = traverser.GetDupeSets();

            // Assert - Should get same number of duplicate sets
            Assert.Equal(dupeSets1.Count, dupeSets2.Count);
        }

        [Fact]
        public void EndToEnd_BaseDirectoryEntry_TracksSourceDirectory()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDiretorySet1);
            traverser.AddBaseDirectory(TestDiretorySet2);

            // Act
            traverser.GetDupeSets();

            // Assert
            Assert.Equal(2, traverser.BaseDirectories.Count);
            Assert.Equal(1, traverser.BaseDirectories[1].Number);
            Assert.Equal(2, traverser.BaseDirectories[2].Number);
            Assert.Contains("Set1", traverser.BaseDirectories[1].Directory);
            Assert.Contains("Set2", traverser.BaseDirectories[2].Directory);
        }

        [Fact]
        public void EndToEnd_FileFiltering_ExcludesProperFiles()
        {
            // Arrange
            var traverser = new Traverser();
            traverser.AddBaseDirectory(TestDataDirectory);

            // Act
            traverser.GetAllFiles();

            // Assert
            // No hidden files (starting with .)
            Assert.DoesNotContain(traverser.UniqueFiles.Values, 
                f => f.Info.Name.StartsWith("."));

            // No zero-length files
            Assert.DoesNotContain(traverser.UniqueFiles.Values, 
                f => f.Info.Length == 0);
        }
    }
}
