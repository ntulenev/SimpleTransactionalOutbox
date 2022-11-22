using FluentAssertions;
using FluentAssertions.Json;

using Newtonsoft.Json.Linq;

using Xunit;

namespace Serialization.Tests;

public class JsonSerializerTests
{
    [Fact(DisplayName = "JsonDeserializer can Deserialize data.")]
    [Trait("Category", "Unit")]
    public void CanDeserialize()
    {

        // Arrange
        var deserializer = new JsonSerializer<TestClass>();
        var testData = new TestClass
        {
            Id = 42,
            Value = "test 123"
        };

        var testString = "{ \"test_value\" : \"test 123\", \"test_id\" : 42 }";

        // Act
        string testJson = null!;
        var exception = Record.Exception(() => testJson = deserializer.Serialize(testData));

        // Assert
        exception.Should().BeNull();
        var testJObject = JToken.Parse(testJson);
        var exampleObject = JToken.Parse(testString);

        testJObject.Count().Should().Be(2);

        testJObject["test_value"].Should().BeEquivalentTo(exampleObject["test_value"]);
        testJObject["test_id"].Should().BeEquivalentTo(exampleObject["test_id"]);
    }

    [Fact(DisplayName = "JsonDeserializer cant Serialize null.")]
    [Trait("Category", "Unit")]
    public void CantSerializeNull()
    {

        // Arrange
        var deserializer = new JsonSerializer<TestClass>();
        TestClass data = null!;

        // Act
        string result = null!;
        var exception = Record.Exception(() => result = deserializer.Serialize(data));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }
}
