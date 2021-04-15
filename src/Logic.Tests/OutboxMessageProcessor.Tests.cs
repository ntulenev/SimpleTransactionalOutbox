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

        [Fact(DisplayName = "OutboxMessageProcessor returns false on send error.")]
        [Trait("Category", "Unit")]
        public async Task ProcessWithFalseOnSendError()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();
            var processor = new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object);
            var cts = new CancellationTokenSource();
            var message = new Mock<IOutboxMessage>();

            sender.Setup(x => x.SendAsync(message.Object, cts.Token)).ThrowsAsync(new Exception());


            bool result = true;
            // Act
            var exception = await Record.ExceptionAsync(async () => result = await processor.TryProcessAsync(message.Object, cts.Token));

            // Assert
            exception.Should().BeNull();
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "OutboxMessageProcessor returns false on remove error.")]
        [Trait("Category", "Unit")]
        public async Task ProcessWithFalseOnRemoveError()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();
            var processor = new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object);
            var cts = new CancellationTokenSource();
            var message = new Mock<IOutboxMessage>();

            uow.Setup(x => x.RemoveOutboxMessageAsync(message.Object, cts.Token)).ThrowsAsync(new Exception());


            bool result = true;
            // Act
            var exception = await Record.ExceptionAsync(async () => result = await processor.TryProcessAsync(message.Object, cts.Token));

            // Assert
            exception.Should().BeNull();
            result.Should().BeFalse();
        }


        [Fact(DisplayName = "OutboxMessageProcessor returns false on save error.")]
        [Trait("Category", "Unit")]
        public async Task ProcessWithFalseOnSaveError()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();
            var processor = new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object);
            var cts = new CancellationTokenSource();
            var message = new Mock<IOutboxMessage>();

            uow.Setup(x => x.SaveAsync(cts.Token)).ThrowsAsync(new Exception());


            bool result = true;
            // Act
            var exception = await Record.ExceptionAsync(async () => result = await processor.TryProcessAsync(message.Object, cts.Token));

            // Assert
            exception.Should().BeNull();
            result.Should().BeFalse();
        }

        [Fact(DisplayName = "OutboxMessageProcessor returns true on correct process.")]
        [Trait("Category", "Unit")]
        public async Task ProcessWithTrueOnAllSucceed()
        {

            // Arrange
            var uow = new Mock<IOutboxUnitOfWork>();
            var sender = new Mock<IOutboxSender>();
            var ilogger = new Mock<ILogger<OutboxMessageProcessor>>();
            var processor = new OutboxMessageProcessor(uow.Object, sender.Object, ilogger.Object);
            var cts = new CancellationTokenSource();
            var message = new Mock<IOutboxMessage>();

            int callOrder = 0;
            sender.Setup(x => x.SendAsync(message.Object, cts.Token)).Callback(() => callOrder++.Should().Be(0));
            uow.Setup(x => x.RemoveOutboxMessageAsync(message.Object, cts.Token)).Callback(() => callOrder++.Should().Be(1));
            uow.Setup(x => x.SaveAsync(cts.Token)).Callback(() => callOrder++.Should().Be(2));


            bool result = true;
            // Act
            var exception = await Record.ExceptionAsync(async () => result = await processor.TryProcessAsync(message.Object, cts.Token));

            // Assert
            exception.Should().BeNull();
            result.Should().BeTrue();

            sender.Verify(x => x.SendAsync(message.Object, cts.Token), Times.Once);
            uow.Verify(x => x.RemoveOutboxMessageAsync(message.Object, cts.Token), Times.Once);
            uow.Verify(x => x.SaveAsync(cts.Token), Times.Once);
        }
    }
}
