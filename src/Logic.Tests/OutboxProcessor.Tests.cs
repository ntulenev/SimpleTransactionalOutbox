using Abstractions.Service;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Logic.Tests;

public class OutboxProcessorTests
{
    [Fact(DisplayName = "OutboxProcessor cant be created with null logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLogger()
    {
        // Arrange
        var logger = (ILogger<OutboxProcessor>)null!;
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();

        // Act
        var exception = Record.Exception(() => new OutboxProcessor(logger, scopeFactory.Object, delayProvider.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxProcessor cant be created with null scope factory.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullScopeFactory()
    {
        // Arrange
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = (IServiceScopeFactory)null!;
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();

        // Act
        var exception = Record.Exception(() => new OutboxProcessor(logger.Object, scopeFactory, delayProvider.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxProcessor cant be created with null delay provider.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullDelayProvider()
    {
        // Arrange
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = (IOutboxBackoffDelayProvider)null!;

        // Act
        var exception = Record.Exception(() => new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxProcessor can be created with valid params.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {
        // Arrange
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();

        // Act
        var exception = Record.Exception(() => new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxProcessor resets backoff delay when outbox processed at least one message.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncResetsBackoffDelayWhenProcessedAnyAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();
        var scope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        var outbox = new Mock<IOutbox>();

        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token))
            .Callback(cts.Cancel)
            .ReturnsAsync(true);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeAssignableTo<OperationCanceledException>();
        scopeFactory.Verify(x => x.CreateScope(), Times.Once);
        outbox.Verify(x => x.RunProcessingAsync(cts.Token), Times.Once);
        delayProvider.Verify(x => x.Reset(), Times.Once);
        delayProvider.Verify(x => x.GetNextDelay(), Times.Never);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "OutboxProcessor waits with backoff delay when there are no messages to process.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncWaitWithBackoffDelayWhenNothingProcessedAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();
        var scope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        var outbox = new Mock<IOutbox>();

        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token))
            .Callback(cts.Cancel)
            .ReturnsAsync(false);
        delayProvider.Setup(x => x.GetNextDelay()).Returns(TimeSpan.Zero);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeAssignableTo<OperationCanceledException>();
        scopeFactory.Verify(x => x.CreateScope(), Times.Once);
        outbox.Verify(x => x.RunProcessingAsync(cts.Token), Times.Once);
        delayProvider.Verify(x => x.GetNextDelay(), Times.Once);
        delayProvider.Verify(x => x.Reset(), Times.Never);
        scope.Verify(x => x.Dispose(), Times.Once);
    }

    [Fact(DisplayName = "OutboxProcessor rethrows non-cancellation exception.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncRethrowsNonCancellationExceptionAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>();
        var scope = new Mock<IServiceScope>();
        var serviceProvider = new Mock<IServiceProvider>();
        var outbox = new Mock<IOutbox>();

        scopeFactory.Setup(x => x.CreateScope()).Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token)).ThrowsAsync(new InvalidOperationException("Boom."));

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        scopeFactory.Verify(x => x.CreateScope(), Times.Once);
        outbox.Verify(x => x.RunProcessingAsync(cts.Token), Times.Once);
        delayProvider.Verify(x => x.GetNextDelay(), Times.Never);
        delayProvider.Verify(x => x.Reset(), Times.Never);
        scope.Verify(x => x.Dispose(), Times.Once);
    }
}
