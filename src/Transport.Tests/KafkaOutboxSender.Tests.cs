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
            var exception = Record.Exception(() => new KafkaOutboxSender(producer, serializer.Object,options.Object,logger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

    }
}
