using System;
using Xunit;
using dupeferret.business;

namespace dupeferret.business.tests
{
    public class FormattableTests
    {
        [Fact]
        public void Constructor_StoresMessage()
        {
            // Arrange
            var message = "Test message with {0}";

            // Act
            var formattable = new Formattable(message);

            // Assert
            Assert.NotNull(formattable);
        }

        [Fact]
        public void Format_SingleParameter_ReplacesPlaceholder()
        {
            // Arrange
            var formattable = new Formattable("Error in file: {0}");
            var fileName = "test.txt";

            // Act
            var result = formattable.Format(fileName);

            // Assert
            Assert.Equal("Error in file: test.txt", result);
        }

        [Fact]
        public void Format_WithInteger_FormatsCorrectly()
        {
            // Arrange
            var formattable = new Formattable("Found {0} items");
            var count = 42;

            // Act
            var result = formattable.Format(count);

            // Assert
            Assert.Equal("Found 42 items", result);
        }

        [Fact]
        public void Format_WithNull_HandlesGracefully()
        {
            // Arrange
            var formattable = new Formattable("Value: {0}");

            // Act
            var result = formattable.Format(null);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Value:", result);
        }

        [Fact]
        public void Format_WithEmptyString_ReplacesPlaceholder()
        {
            // Arrange
            var formattable = new Formattable("Text: '{0}'");

            // Act
            var result = formattable.Format(string.Empty);

            // Assert
            Assert.Equal("Text: ''", result);
        }

        [Fact]
        public void Format_WithComplexObject_UsesToString()
        {
            // Arrange
            var formattable = new Formattable("Date: {0}");
            var date = new DateTime(2024, 1, 15);

            // Act
            var result = formattable.Format(date);

            // Assert
            Assert.Contains("2024", result);
            Assert.Contains("Date:", result);
        }

        [Theory]
        [InlineData("Error: {0}", "test", "Error: test")]
        [InlineData("Count: {0}", "100", "Count: 100")]
        [InlineData("{0} is the value", "First", "First is the value")]
        public void Format_VariousTemplates_FormatsCorrectly(string template, string value, string expected)
        {
            // Arrange
            var formattable = new Formattable(template);

            // Act
            var result = formattable.Format(value);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Format_NoPlaceholder_ReturnsOriginalMessage()
        {
            // Arrange
            var message = "This is a message without placeholder";
            var formattable = new Formattable(message);

            // Act
            var result = formattable.Format("ignored");

            // Assert
            Assert.Equal(message, result);
        }
    }
}
