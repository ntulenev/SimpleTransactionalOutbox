using System;

using Confluent.Kafka;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using Xunit;

using Abstractions.Models;

using FluentAssertions;

namespace Transport.Tests
{
    public class KafkaOutboxSenderTests
    {
        [Fact(DisplayName = "KafkaOutboxSender cant be created with null producer.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullProducer()
        {

            // Arrange
            var producer = (IProducer<Null, string>)null!;
            var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>();
            var options = new Mock<IOptions<KafkaOutboxSenderOptions>>();
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
            var producer = new Mock<IProducer<Null, string>>();
            var serializer = (Abstractions.Serialization.ISerializer<IOutboxMessage>)null!;
            var options = new Mock<IOptions<KafkaOutboxSenderOptions>>();
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
            var producer = new Mock<IProducer<Null, string>>();
            var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>();
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
            var producer = new Mock<IProducer<Null, string>>();
            var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>();
            var options = new Mock<IOptions<KafkaOutboxSenderOptions>>();
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
            var producer = new Mock<IProducer<Null, string>>();
            var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>();
            var options = new Mock<IOptions<KafkaOutboxSenderOptions>>();
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
            var producer = new Mock<IProducer<Null, string>>();
            var serializer = new Mock<Abstractions.Serialization.ISerializer<IOutboxMessage>>();
            var options = new Mock<IOptions<KafkaOutboxSenderOptions>>();
            options.Setup(x => x.Value).Returns(new KafkaOutboxSenderOptions() { TopicName = "A" });
            var logger = new Mock<ILogger<KafkaOutboxSender>>();

            // Act
            var exception = Record.Exception(() => new KafkaOutboxSender(producer.Object, serializer.Object, options.Object, logger.Object));

            // Assert
            exception.Should().BeNull();
        }

    }
}
