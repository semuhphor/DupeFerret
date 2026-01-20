using System;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class ErrorMessagesTests
    {
        [Fact]
        public void InvalidDirectory_HasCorrectFormat()
        {
            // Arrange
            var testDirectory = @"C:\NonExistent\Directory";

            // Act
            var message = ErrorMessages.InvalidDirectory.Format(testDirectory);

            // Assert
            Assert.Contains("Invalid directory:", message);
            Assert.Contains(testDirectory, message);
        }

        [Fact]
        public void DuplicateBaseDirectory_HasCorrectFormat()
        {
            // Arrange
            var testDirectory = @"C:\Duplicate\Directory";

            // Act
            var message = ErrorMessages.DuplicateBaseDirectory.Format(testDirectory);

            // Assert
            Assert.Contains("Duplicate base directory:", message);
            Assert.Contains(testDirectory, message);
        }

        [Fact]
        public void DirectoryNotInFQFN_HasCorrectFormat()
        {
            // Arrange
            var testDirectory = @"C:\Some\Directory";

            // Act
            var message = ErrorMessages.DirectoryNotInFQFN.Format(testDirectory);

            // Assert
            Assert.Contains("Directory", message);
            Assert.Contains(testDirectory, message);
            Assert.Contains("not in FQFN", message);
        }

        [Fact]
        public void ErrorMessages_AreNotNull()
        {
            // Assert
            Assert.NotNull(ErrorMessages.InvalidDirectory);
            Assert.NotNull(ErrorMessages.DuplicateBaseDirectory);
            Assert.NotNull(ErrorMessages.DirectoryNotInFQFN);
        }

        [Theory]
        [InlineData("")]
        [InlineData("test")]
        [InlineData(@"C:\Windows\System32")]
        [InlineData("Special characters: !@#$%")]
        public void ErrorMessages_FormatWithVariousInputs(string input)
        {
            // Act
            var message1 = ErrorMessages.InvalidDirectory.Format(input);
            var message2 = ErrorMessages.DuplicateBaseDirectory.Format(input);
            var message3 = ErrorMessages.DirectoryNotInFQFN.Format(input);

            // Assert
            Assert.NotNull(message1);
            Assert.NotNull(message2);
            Assert.NotNull(message3);
            Assert.Contains(input, message1);
            Assert.Contains(input, message2);
            Assert.Contains(input, message3);
        }
    }
}
