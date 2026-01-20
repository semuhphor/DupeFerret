using System;
using System.IO;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class FileInfoHandlerTests : TestBase
    {
        [Fact]
        public void Constructor_ValidFile_LoadsAllProperties()
        {
            // Arrange & Act
            var handler = new FileInfoHandler(FQTestFileName);

            // Assert
            Assert.True(handler.Length > 0);
            Assert.Equal("Dup_1_A", handler.Name);
            Assert.True(handler.CreationTime > DateTime.MinValue);
            Assert.True(handler.LastWriteTime > DateTime.MinValue);
        }

        [Fact]
        public void Length_CachedAfterConstruction_ReturnsSameValue()
        {
            // Arrange
            var handler = new FileInfoHandler(FQTestFileName);
            var firstLength = handler.Length;

            // Act
            var secondLength = handler.Length;

            // Assert
            Assert.Equal(firstLength, secondLength);
        }

        [Fact]
        public void Name_ReturnsFileNameWithoutPath()
        {
            // Arrange
            var handler = new FileInfoHandler(FQTestFileName);

            // Act
            var name = handler.Name;

            // Assert
            Assert.DoesNotContain(Path.DirectorySeparatorChar.ToString(), name);
            Assert.DoesNotContain(Path.AltDirectorySeparatorChar.ToString(), name);
        }

        [Fact]
        public void CreationTime_IsValidDateTime()
        {
            // Arrange & Act
            var handler = new FileInfoHandler(FQTestFileName);

            // Assert
            Assert.True(handler.CreationTime > new DateTime(2000, 1, 1));
            Assert.True(handler.CreationTime <= DateTime.Now.AddDays(1));
        }

        [Fact]
        public void LastWriteTime_IsValidDateTime()
        {
            // Arrange & Act
            var handler = new FileInfoHandler(FQTestFileName);

            // Assert
            Assert.True(handler.LastWriteTime > new DateTime(2000, 1, 1));
            Assert.True(handler.LastWriteTime <= DateTime.Now.AddDays(1));
        }

        [Fact]
        public void LastWriteTime_IsValid()
        {
            // Arrange & Act
            var handler = new FileInfoHandler(FQTestFileName);

            // Assert - Both times should be valid and reasonable
            // File system times can vary, so just ensure they're populated and reasonable
            Assert.True(handler.LastWriteTime > new DateTime(2000, 1, 1));
            Assert.True(handler.CreationTime > new DateTime(2000, 1, 1));
            Assert.True(handler.LastWriteTime <= DateTime.Now.AddDays(1));
            Assert.True(handler.CreationTime <= DateTime.Now.AddDays(1));
        }

        [Fact]
        public void DefaultConstructor_CreatesEmptyHandler()
        {
            // Act
            var handler = new FileInfoHandler();

            // Assert - Should not throw and properties should have default values
            Assert.Equal(0, handler.Length);
            Assert.Null(handler.Name);
            Assert.Equal(default(DateTime), handler.CreationTime);
            Assert.Equal(default(DateTime), handler.LastWriteTime);
        }
    }
}
