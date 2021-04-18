using System;
using System.ComponentModel;

using FluentAssertions;

using Xunit;

namespace Models.Tests
{
    public class OutboxMessageTests
    {
        [Fact(DisplayName = "OutboxMessage can't be created with null body.")]
        [Trait("Category", "Unit")]
        public void CantCreateOutboxMessageTestsWithNullBody()
        {

            // Arrange
            string body = null!;

            // Act
            var exception = Record.Exception(() => new OutboxMessage(Guid.NewGuid(), DateTime.UtcNow, Abstractions.Models.OutboxMessageType.ProcessingDataMessage, body));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxMessage can't be created with empty body.")]
        [Trait("Category", "Unit")]
        public void CantCreateOutboxMessageTestsWithEmptyBody()
        {
            // Arrange
            string body = string.Empty;

            // Act
            var exception = Record.Exception(() => new OutboxMessage(Guid.NewGuid(), DateTime.UtcNow, Abstractions.Models.OutboxMessageType.ProcessingDataMessage, body));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "OutboxMessage can't be created with witespaces body.")]
        [Trait("Category", "Unit")]
        public void CantCreateOutboxMessageTestsWithWhitespacesBody()
        {
            // Arrange
            string body = "  ";

            // Act
            var exception = Record.Exception(() => new OutboxMessage(Guid.NewGuid(), DateTime.UtcNow, Abstractions.Models.OutboxMessageType.ProcessingDataMessage, body));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentException>();
        }

        [Fact(DisplayName = "OutboxMessage can't be created with invalid message type.")]
        [Trait("Category", "Unit")]
        public void CantCreateOutboxMessageTestsWithBadMessageType()
        {
            // Arrange
            string body = "test";
            int messageType = int.MaxValue;

            // Act
            var exception = Record.Exception(() => new OutboxMessage(Guid.NewGuid(), DateTime.UtcNow, (Abstractions.Models.OutboxMessageType)messageType, body));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<InvalidEnumArgumentException>();
        }

        [Fact(DisplayName = "OutboxMessage can be created with valid params.")]
        [Trait("Category", "Unit")]
        public void OutboxMessageShouldBeCreatedWithValidParams()
        {
            // Arrange
            string body = "test";
            Abstractions.Models.OutboxMessageType messageType = Abstractions.Models.OutboxMessageType.ProcessingDataMessage;
            var id = Guid.NewGuid();
            var date = DateTime.UtcNow;

            // Act
            OutboxMessage res = null!;
            var exception = Record.Exception(() => res = new OutboxMessage(id, date, messageType, body));

            // Assert
            exception.Should().BeNull();

            res.MessageId.Should().Be(id);
            res.OccurredOn.Should().Be(date);
            res.MessageType.Should().Be(messageType);
            res.Body.Should().Be(body);

        }
    }
}
