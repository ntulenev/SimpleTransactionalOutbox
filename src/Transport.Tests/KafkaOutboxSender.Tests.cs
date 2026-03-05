using Abstractions.Models;

using Confluent.Kafka;

using FluentAssertions;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

namespace Transport.Tests;

public class KafkaOutboxSenderTests
{
    [Fact(DisplayName = "KafkaOutboxSender cant be created with null producer.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullProducer()
    {

        // Arrange
        var producer = (IProducer<Null, string>)null!;
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        var logger = new Mock<ILogger<KafkaOutboxSender>>();

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer, serializer.Object, options.Object, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender cant be created with null serializer.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullSerializer()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = (Abstractions.Serialization.ISerializer<IOutboxMessage>)null!;
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        var logger = new Mock<ILogger<KafkaOutboxSender>>();

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer, options.Object, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender cant be created with null options.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullOptions()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = (IOptions<KafkaOutboxSenderOptions>)null!;
        var logger = new Mock<ILogger<KafkaOutboxSender>>();

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer.Object, options, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender cant be created with null logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLogger()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        var logger = (ILogger<KafkaOutboxSender>)null!;

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender cant be created with null options value.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullOptionsValue()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        options.Setup(x => x.Value).Returns((KafkaOutboxSenderOptions)null!);
        var logger = new Mock<ILogger<KafkaOutboxSender>>();

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender can be created with valid params.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        options.Setup(x => x.Value).Returns(new KafkaOutboxSenderOptions() { TopicName = "A" });
        var logger = new Mock<ILogger<KafkaOutboxSender>>();

        // Act
        var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger.Object));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "KafkaOutboxSender cant send null message.")]
    [Trait("Category", "Unit")]
    public async Task CantSendNullMessageAsync()
    {

        // Arrange
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        options.Setup(x => x.Value).Returns(new KafkaOutboxSenderOptions() { TopicName = "A" });
        var logger = new Mock<ILogger<KafkaOutboxSender>>();
        var sender = new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger.Object);
        using var token = new CancellationTokenSource();

        // Act
        var exception = await Record.ExceptionAsync(async () => await sender.SendAsync(null!, token.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "KafkaOutboxSender can send valid message.")]
    [Trait("Category", "Unit")]
    public async Task CanSendValidMessageAsync()
    {

        // Arrange
        var topicName = "A";
        var producer = new Mock<IProducer<Null, string>>(MockBehavior.Strict);
        var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>(MockBehavior.Strict);
        var options = new Mock<IOptions<KafkaOutboxSenderOptions>>(MockBehavior.Strict);
        options.Setup(x => x.Value).Returns(new KafkaOutboxSenderOptions() { TopicName = topicName });
        var logger = new Mock<ILogger<KafkaOutboxSender>>();
        var sender = new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger.Object);
        using var token = new CancellationTokenSource();
        var message = new Mock<IOutboxMessage>(MockBehavior.Strict);

        var jsonStr = "test";
        serializer.Setup(x => x.Serialize(message.Object)).Returns(jsonStr);
        producer.Setup(x => x.ProduceAsync(topicName, It.IsAny<Message<Null, string>>(), token.Token))
            .ReturnsAsync(new DeliveryResult<Null, string>());


        // Act
        var exception = await Record.ExceptionAsync(async () => await sender.SendAsync(message.Object, token.Token));

        // Assert
        exception.Should().BeNull();
        serializer.Verify(x => x.Serialize(message.Object), Times.Once);
        producer.Verify(x => x.ProduceAsync(topicName, It.Is<Message<Null, string>>(x => x.Value == jsonStr), token.Token), Times.Once);
    }
}

