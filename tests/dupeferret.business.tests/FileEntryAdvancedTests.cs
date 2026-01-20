using System;
using System.IO;
using System.Linq;
using Xunit;
using FakeItEasy;

namespace dupeferret.business.tests
{
    public class FileEntryAdvancedTests : TestBase
    {
        [Fact]
        public void Constructor_WithHandler_InitializesCorrectly()
        {
            // Arrange
            var handler = new FileInfoHandler(FQTestFileName);

            // Act
            var entry = new FileEntry(1, FQTestFileName, handler);

            // Assert
            Assert.Equal(1, entry.BaseDirectoryKey);
            Assert.Equal(FQTestFileName, entry.FQFN);
            Assert.Same(handler, entry.Info);
        }

        [Fact]
        public void Constructor_WithoutHandler_CreatesHandler()
        {
            // Arrange & Act
            var entry = new FileEntry(5, FQTestFileName);

            // Assert
            Assert.Equal(5, entry.BaseDirectoryKey);
            Assert.Equal(FQTestFileName, entry.FQFN);
            Assert.NotNull(entry.Info);
            Assert.Equal("Dup_1_A", entry.Info.Name);
        }

        [Fact]
        public void FirstHash_CalledMultipleTimes_ReturnsSameValue()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var hash1 = entry.FirstHash();
            var hash2 = entry.FirstHash();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.NotNull(entry.First512Hash);
        }

        [Fact]
        public void FullHash_CalledMultipleTimes_ReturnsSameValue()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var hash1 = entry.FullHash();
            var hash2 = entry.FullHash();

            // Assert
            Assert.Equal(hash1, hash2);
            Assert.NotNull(entry.FullFileHash);
        }

        [Fact]
        public void FirstHash_DifferentFiles_ProducesDifferentHashes()
        {
            // Arrange
            var entry1 = new FileEntry(1, FQTestFileName);
            var entry2 = new FileEntry(1, Path.Combine(TestDiretorySet1, "NotDup_1"));

            // Act
            var hash1 = entry1.FirstHash();
            var hash2 = entry2.FirstHash();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void FullHash_DifferentFiles_ProducesDifferentHashes()
        {
            // Arrange
            var entry1 = new FileEntry(1, FQTestFileName);
            var entry2 = new FileEntry(1, Path.Combine(TestDiretorySet1, "NotDup_1"));

            // Act
            var hash1 = entry1.FullHash();
            var hash2 = entry2.FullHash();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void FirstHash_IdenticalFiles_ProducesSameHashes()
        {
            // Arrange
            var entry1 = new FileEntry(1, FQTestFileName);
            var entry2 = new FileEntry(2, FQDupFileName);

            // Act
            var hash1 = entry1.FirstHash();
            var hash2 = entry2.FirstHash();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void FullHash_IdenticalFiles_ProducesSameHashes()
        {
            // Arrange
            var entry1 = new FileEntry(1, FQTestFileName);
            var entry2 = new FileEntry(2, FQDupFileName);

            // Act
            var hash1 = entry1.FullHash();
            var hash2 = entry2.FullHash();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void Hash_OutputFormat_IsHexadecimal()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var hash = entry.FirstHash();

            // Assert
            Assert.Matches("^[a-f0-9]+$", hash);
            Assert.True(hash.Length > 0);
        }

        [Fact]
        public void Hash_SHA512_ProducesExpectedLength()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var hash = entry.FullHash();

            // Assert
            // SHA512 produces 512 bits = 64 bytes = 128 hex characters
            Assert.Equal(128, hash.Length);
        }

        [Fact]
        public void ISimpleFileEntry_ExposesCorrectProperties()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);
            ISimpleFileEntry simple = entry;

            // Assert
            Assert.Equal(entry.FQFN, simple.FQFN);
            Assert.Equal(entry.Info.Name, simple.Name);
            Assert.Equal(entry.Info.Length, simple.Length);
            Assert.Equal(entry.Info.CreationTime, simple.CreationTime);
            Assert.Equal(entry.Info.LastWriteTime, simple.LastWriteTime);
        }

        [Fact]
        public void CompareTo_NullObject_ReturnsOne()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var result = entry.CompareTo(null);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void CompareTo_SameObject_ReturnsZero()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var result = entry.CompareTo(entry);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CompareTo_NonFileEntry_ThrowsException()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);
            var notFileEntry = new object();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => entry.CompareTo(notFileEntry));
        }

        [Fact]
        public void CompareTo_OrdersByCreationTime_ThenLastWriteTime_ThenPath()
        {
            // Arrange
            var now = DateTime.Now;
            var earlier = now.AddDays(-1);
            
            var handler1 = A.Fake<FileInfoHandler>();
            A.CallTo(() => handler1.CreationTime).Returns(earlier);
            A.CallTo(() => handler1.LastWriteTime).Returns(earlier);
            
            var handler2 = A.Fake<FileInfoHandler>();
            A.CallTo(() => handler2.CreationTime).Returns(now);
            A.CallTo(() => handler2.LastWriteTime).Returns(now);

            var entry1 = new FileEntry(1, "A", handler1);
            var entry2 = new FileEntry(1, "B", handler2);

            // Act
            var result = entry1.CompareTo(entry2);

            // Assert
            Assert.True(result < 0, "Earlier file should come before later file");
        }

        [Fact]
        public void CompareTo_SameDateTime_OrdersByPath()
        {
            // Arrange
            var now = DateTime.Now;
            
            var handler = A.Fake<FileInfoHandler>();
            A.CallTo(() => handler.CreationTime).Returns(now);
            A.CallTo(() => handler.LastWriteTime).Returns(now);

            var entry1 = new FileEntry(1, "A_Path", handler);
            var entry2 = new FileEntry(1, "B_Path", handler);

            // Act
            var result = entry1.CompareTo(entry2);

            // Assert
            Assert.True(result < 0, "A_Path should come before B_Path alphabetically");
        }

        [Fact]
        public void CompareTo_TransitiveProperty_Holds()
        {
            // Arrange
            var now = DateTime.Now;
            var handler = A.Fake<FileInfoHandler>();
            A.CallTo(() => handler.CreationTime).Returns(now);
            A.CallTo(() => handler.LastWriteTime).Returns(now);

            var entry1 = new FileEntry(1, "A", handler);
            var entry2 = new FileEntry(1, "B", handler);
            var entry3 = new FileEntry(1, "C", handler);

            // Act
            var result1 = entry1.CompareTo(entry2);
            var result2 = entry2.CompareTo(entry3);
            var result3 = entry1.CompareTo(entry3);

            // Assert - if A < B and B < C, then A < C
            if (result1 < 0 && result2 < 0)
            {
                Assert.True(result3 < 0);
            }
        }

        [Fact]
        public void BaseDirectoryKey_StoresCorrectValue()
        {
            // Arrange & Act
            var entry = new FileEntry(42, FQTestFileName);

            // Assert
            Assert.Equal(42, entry.BaseDirectoryKey);
        }

        [Fact]
        public void FQFN_StoresFullPath()
        {
            // Arrange
            var fullPath = Path.GetFullPath(FQTestFileName);

            // Act
            var entry = new FileEntry(1, fullPath);

            // Assert
            Assert.Equal(fullPath, entry.FQFN);
        }

        [Fact]
        public void HashOperations_WorkWithSmallFiles()
        {
            // Arrange
            var entry = new FileEntry(1, FQTestFileName);

            // Act
            var firstHash = entry.FirstHash();
            var fullHash = entry.FullHash();

            // Assert
            Assert.NotNull(firstHash);
            Assert.NotNull(fullHash);
            // For small files (< 512 bytes), first hash and full hash will be the same
            // since they hash the entire file in both cases
            Assert.Equal(128, firstHash.Length); // SHA512 = 128 hex chars
            Assert.Equal(128, fullHash.Length);
        }
    }
}
