using FluentAssertions;

using Xunit;

namespace Serialization.Tests
{
    public class JsonDeserializerTests
    {
        [Fact(DisplayName = "JsonDeserializer can Deserialize data.")]
        [Trait("Category", "Unit")]
        public void CanDeserialize()
        {

            // Arrange
            var deserializer = new JsonDeserializer<TestClass>();
            var testString = "{ \"test_value\" : \"test 123\", \"test_id\" : 42 }";

            // Act
            TestClass result = null!;
            var exception = Record.Exception(() => result = deserializer.Deserialize(testString));

            // Assert
            exception.Should().BeNull();
            result.Id.Should().Be(42);
            result.Value.Should().Be("test 123");
        }

        [Fact(DisplayName = "JsonDeserializer cant Deserialize null.")]
        [Trait("Category", "Unit")]
        public void CantDeserializeNull()
        {

            // Arrange
            var deserializer = new JsonDeserializer<TestClass>();
            string testString = null!;

            // Act
            TestClass result = null!;
            var exception = Record.Exception(() => result = deserializer.Deserialize(testString));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }
    }
}
