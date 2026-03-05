using Abstractions.Bus;
using Abstractions.DB;
using Abstractions.Models;

using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Logic.Tests;

public class OutboxMessageProcessorTests
{
    [Fact(DisplayName = "OutboxMessageProcessor cant be created with null UOW.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullUOW()
    {

        // Arrange
        var uow = (IOutboxUnitOfWork)null!;
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();

        // Act
        var exception = Record.Exception(() => new OutboxMessageProcessor(uow, sender.Object, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxMessageProcessor cant be created with null sender.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullSender()
    {

        // Arrange
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = (IOutboxSender)null!;
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();

        // Act
        var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxMessageProcessor cant be created with null sender.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLogger()
    {

        // Arrange
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = (ILogger<OutboxMessageProcessor>)null!;

        // Act
        var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender.Object, logger));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxMessageProcessor can be created.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {

        // Arrange
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();

        // Act
        var exception = Record.Exception(() => new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxMessageProcessor cant process null message.")]
    [Trait("Category", "Unit")]
    public async Task CantProcessNullMessageAsync()
    {

        // Arrange
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();
        var processor = new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object);
        using var cts = new CancellationTokenSource();
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
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();
        var processor = new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object);
        using var cts = new CancellationTokenSource();
        var message = new Mock<IOutboxMessage>(MockBehavior.Strict);

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
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();
        var processor = new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object);
        using var cts = new CancellationTokenSource();
        var message = new Mock<IOutboxMessage>(MockBehavior.Strict);

        sender.Setup(x => x.SendAsync(message.Object, cts.Token)).Returns(Task.CompletedTask);
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
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();
        var processor = new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object);
        using var cts = new CancellationTokenSource();
        var message = new Mock<IOutboxMessage>(MockBehavior.Strict);

        sender.Setup(x => x.SendAsync(message.Object, cts.Token)).Returns(Task.CompletedTask);
        uow.Setup(x => x.RemoveOutboxMessageAsync(message.Object, cts.Token)).Returns(Task.CompletedTask);
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
        var uow = new Mock<IOutboxUnitOfWork>(MockBehavior.Strict);
        var sender = new Mock<IOutboxSender>(MockBehavior.Strict);
        var logger = new Mock<ILogger<OutboxMessageProcessor>>();
        var processor = new OutboxMessageProcessor(uow.Object, sender.Object, logger.Object);
        using var cts = new CancellationTokenSource();
        var message = new Mock<IOutboxMessage>(MockBehavior.Strict);

        var sendCalls = 0;
        var removeCalls = 0;
        var saveCalls = 0;

        sender.Setup(x => x.SendAsync(message.Object, cts.Token))
            .Callback(() => sendCalls++)
            .Returns(Task.CompletedTask);
        uow.Setup(x => x.RemoveOutboxMessageAsync(message.Object, cts.Token))
            .Callback(() => removeCalls++)
            .Returns(Task.CompletedTask);
        uow.Setup(x => x.SaveAsync(cts.Token))
            .Callback(() => saveCalls++)
            .Returns(Task.CompletedTask);


        bool result = true;
        // Act
        var exception = await Record.ExceptionAsync(async () => result = await processor.TryProcessAsync(message.Object, cts.Token));

        // Assert
        exception.Should().BeNull();
        result.Should().BeTrue();
        sendCalls.Should().Be(1);
        removeCalls.Should().Be(1);
        saveCalls.Should().Be(1);
    }
}

