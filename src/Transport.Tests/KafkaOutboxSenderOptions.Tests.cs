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
        var exception = Record.Exception(() => new KafkaOutboxSenderOptions() { TopicName = null! });

        // Assert
        exception.Should().BeNull();
    }
}
