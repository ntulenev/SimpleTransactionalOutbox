using FluentAssertions;

using Xunit;

namespace Transport.Tests;

public class KafkaOutboxSenderOptionsTests
{
    [Fact(DisplayName = "KafkaOutboxSenderOptions can be created.")]
    [Trait("Category", "Unit")]
    public void CanBeCreated()
    {

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSenderOptions());

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "KafkaOutboxSenderOptions can get an set topic name.")]
    [Trait("Category", "Unit")]
    public void CanGetAndSetTopicName()
    {
        // Arrange
        var topicName = "data";
        var options = new KafkaOutboxSenderOptions();

        // Act
        var exception = Record.Exception(() => options.TopicName = topicName);

        // Assert
        exception.Should().BeNull();
        options.TopicName.Should().Be(topicName);
    }
}
