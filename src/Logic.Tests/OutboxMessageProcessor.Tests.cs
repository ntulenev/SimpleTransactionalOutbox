using System;

using Xunit;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Abstractions.DB;
using Abstractions.Bus;
using System.Threading;
using Abstractions.Models;
using System.Threading.Tasks;

namespace Logic.Tests
{
    public class OutboxMessageProcessorTests
    {
        [Fact(DisplayName = "OutboxMessageProcessor cant be created with null UOW.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullUOW()
        {

            // Arrange
            var uow = (IOutboxUnitOfWork)null!;
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();

            // Act
            var exception = Record.Exception(() => new OutboxMessageProcessor(uow, sender.Object, ilogger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxMessageProcessor cant be created with null sender.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullSender()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = (IOutboxSender)null!;
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();

            // Act
            var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender, ilogger.Object));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxMessageProcessor cant be created with null sender.")]
        [Trait("Category", "Unit")]
        public void CantCreateWithNullLogger()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = (ILogger<OutboxMessageProcessor>)null!;

            // Act
            var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender.Object, ilogger));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }

        [Fact(DisplayName = "OutboxMessageProcessor can be created.")]
        [Trait("Category", "Unit")]
        public void CanCreate()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();

            // Act
            var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object));

            // Assert
            exception.Should().BeNull();
        }

        [Fact(DisplayName = "OutboxMessageProcessor cant process null message.")]
        [Trait("Category", "Unit")]
        public async Task CantProcessNullMessageAsync()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();
            var processor = new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object);
            var cts = new CancellationTokenSource();
            IOutboxMessage message = null!;

            // Act
            var exception = await Record.ExceptionAsync(async () => _ = await processor.TryProcessAsync(message, cts.Token));

            // Assert
            exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
        }
    }
}
