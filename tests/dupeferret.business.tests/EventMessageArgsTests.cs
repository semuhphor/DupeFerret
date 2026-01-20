using System;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class EventMessageArgsTests
    {
        [Fact]
        public void Constructor_SetsMessage()
        {
            // Arrange
            var message = "Test message";

            // Act
            var args = new EventMessageArgs(message);

            // Assert
            Assert.Equal(message, args.Message);
        }

        [Fact]
        public void Constructor_WithEmptyString_SetsMessage()
        {
            // Arrange
            var message = string.Empty;

            // Act
            var args = new EventMessageArgs(message);

            // Assert
            Assert.Equal(message, args.Message);
        }

        [Fact]
        public void Constructor_WithNull_SetsMessage()
        {
            // Arrange
            string message = null;

            // Act
            var args = new EventMessageArgs(message);

            // Assert
            Assert.Null(args.Message);
        }

        [Fact]
        public void Message_CanBeModified()
        {
            // Arrange
            var args = new EventMessageArgs("Initial");

            // Act
            args.Message = "Modified";

            // Assert
            Assert.Equal("Modified", args.Message);
        }

        [Fact]
        public void InheritsFromEventArgs()
        {
            // Arrange
            var args = new EventMessageArgs("test");

            // Assert
            Assert.IsAssignableFrom<EventArgs>(args);
        }

        [Theory]
        [InlineData("Simple message")]
        [InlineData("Message with special chars: !@#$%^&*()")]
        [InlineData("Message with path: C:\\Windows\\System32")]
        [InlineData("Very long message that contains many words to test if the class can handle longer strings without any issues")]
        public void Constructor_WithVariousMessages_StoresCorrectly(string message)
        {
            // Act
            var args = new EventMessageArgs(message);

            // Assert
            Assert.Equal(message, args.Message);
        }

        [Fact]
        public void Message_SetterWorks()
        {
            // Arrange
            var args = new EventMessageArgs("Initial message");
            var newMessage = "New message";

            // Act
            args.Message = newMessage;

            // Assert
            Assert.Equal(newMessage, args.Message);
        }
    }
}
