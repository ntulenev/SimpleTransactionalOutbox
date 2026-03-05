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
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);

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
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);

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
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
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
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);

        // Act
        var exception = Record.Exception(() => new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "OutboxProcessor stops immediately when cancellation is already requested.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncStopsImmediatelyOnPreCancelledTokenAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);
        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var outbox = new Mock<IOutbox>(MockBehavior.Strict);

        var createScopeCalls = 0;
        var getNextDelayCalls = 0;
        var resetCalls = 0;

        scopeFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.SetupGet(x => x.ServiceProvider).Returns(serviceProvider.Object);
        scope.Setup(x => x.Dispose());
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token)).ReturnsAsync(false);

        delayProvider.Setup(x => x.GetNextDelay())
            .Callback(() => getNextDelayCalls++)
            .Returns(TimeSpan.Zero);
        delayProvider.Setup(x => x.Reset()).Callback(() => resetCalls++);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeAssignableTo<OperationCanceledException>();
        createScopeCalls.Should().Be(0);
        getNextDelayCalls.Should().Be(0);
        resetCalls.Should().Be(0);
    }

    [Fact(DisplayName = "OutboxProcessor resets backoff delay when outbox processed at least one message.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncResetsBackoffDelayWhenProcessedAnyAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);
        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var outbox = new Mock<IOutbox>(MockBehavior.Strict);

        var createScopeCalls = 0;
        var runProcessingCalls = 0;
        var resetCalls = 0;
        var getNextDelayCalls = 0;
        var disposeCalls = 0;

        scopeFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        scope.Setup(x => x.Dispose()).Callback(() => disposeCalls++);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        delayProvider.Setup(x => x.Reset()).Callback(() => resetCalls++);
        delayProvider.Setup(x => x.GetNextDelay())
            .Callback(() => getNextDelayCalls++)
            .Returns(TimeSpan.Zero);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token))
            .Callback(() =>
            {
                runProcessingCalls++;
                cts.Cancel();
            })
            .ReturnsAsync(true);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeAssignableTo<OperationCanceledException>();
        createScopeCalls.Should().Be(1);
        runProcessingCalls.Should().Be(1);
        resetCalls.Should().Be(1);
        getNextDelayCalls.Should().Be(0);
        disposeCalls.Should().Be(1);
    }

    [Fact(DisplayName = "OutboxProcessor waits with backoff delay when there are no messages to process.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncWaitWithBackoffDelayWhenNothingProcessedAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);
        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var outbox = new Mock<IOutbox>(MockBehavior.Strict);

        var createScopeCalls = 0;
        var runProcessingCalls = 0;
        var getNextDelayCalls = 0;
        var resetCalls = 0;
        var disposeCalls = 0;

        scopeFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        scope.Setup(x => x.Dispose()).Callback(() => disposeCalls++);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token))
            .Callback(() =>
            {
                runProcessingCalls++;
                cts.Cancel();
            })
            .ReturnsAsync(false);
        delayProvider.Setup(x => x.GetNextDelay())
            .Callback(() => getNextDelayCalls++)
            .Returns(TimeSpan.Zero);
        delayProvider.Setup(x => x.Reset()).Callback(() => resetCalls++);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeAssignableTo<OperationCanceledException>();
        createScopeCalls.Should().Be(1);
        runProcessingCalls.Should().Be(1);
        getNextDelayCalls.Should().Be(1);
        resetCalls.Should().Be(0);
        disposeCalls.Should().Be(1);
    }

    [Fact(DisplayName = "OutboxProcessor rethrows non-cancellation exception.")]
    [Trait("Category", "Unit")]
    public async Task ProcessAsyncRethrowsNonCancellationExceptionAsync()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var logger = new Mock<ILogger<OutboxProcessor>>();
        var scopeFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var delayProvider = new Mock<IOutboxBackoffDelayProvider>(MockBehavior.Strict);
        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var serviceProvider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var outbox = new Mock<IOutbox>(MockBehavior.Strict);

        var createScopeCalls = 0;
        var runProcessingCalls = 0;
        var getNextDelayCalls = 0;
        var resetCalls = 0;
        var disposeCalls = 0;

        scopeFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);
        scope.Setup(x => x.Dispose()).Callback(() => disposeCalls++);
        serviceProvider.Setup(x => x.GetService(typeof(IOutbox))).Returns(outbox.Object);
        outbox.Setup(x => x.RunProcessingAsync(cts.Token))
            .Callback(() => runProcessingCalls++)
            .ThrowsAsync(new InvalidOperationException("Boom."));
        delayProvider.Setup(x => x.GetNextDelay())
            .Callback(() => getNextDelayCalls++)
            .Returns(TimeSpan.Zero);
        delayProvider.Setup(x => x.Reset()).Callback(() => resetCalls++);

        var processor = new OutboxProcessor(logger.Object, scopeFactory.Object, delayProvider.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await processor.ProcessAsync(cts.Token));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<InvalidOperationException>();
        createScopeCalls.Should().Be(1);
        runProcessingCalls.Should().Be(1);
        getNextDelayCalls.Should().Be(0);
        resetCalls.Should().Be(0);
        disposeCalls.Should().Be(1);
    }
}

