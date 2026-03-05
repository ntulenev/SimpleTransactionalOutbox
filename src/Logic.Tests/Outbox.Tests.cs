using Abstractions.DB;
using Abstractions.Models;
using Abstractions.Service;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Logic.Tests;

public class OutboxTests
{

    [Fact(DisplayName = "Outbox cant be created with null fetcher.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullFetcher()
    {
        // Arrange
        var fetcher = (IOutboxFetcher)null!;
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = new Mock<ILogger<Outbox>>();

        // Act
        var exception = Record.Exception(() => new Outbox(fetcher, scopedFactory.Object, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }


    [Fact(DisplayName = "Outbox cant be created with null scopedFactory.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullFactory()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = (IServiceScopeFactory)null!;
        var logger = new Mock<ILogger<Outbox>>();

        // Act
        var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory, logger.Object));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "Outbox cant be created with null Logger.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullLogger()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = (ILogger<Outbox>)null!;

        // Act
        var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory.Object, logger));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "Outbox can be created with valid params.")]
    [Trait("Category", "Unit")]
    public void CanCreate()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = new Mock<ILogger<Outbox>>();

        // Act
        var exception = Record.Exception(() => new Outbox(fetcher.Object, scopedFactory.Object, logger.Object));

        // Assert
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "Outbox process messages correctly.")]
    [Trait("Category", "Unit")]
    public async Task RunProcessingAsyncProcessMessagesCorrectlyAsync()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = new Mock<ILogger<Outbox>>();
        var outBox = new Outbox(fetcher.Object, scopedFactory.Object, logger.Object);
        using var tokenSource = new CancellationTokenSource();

        var msg = new TestMessage();
        var readOutboxMessagesCalls = 0;
        var createScopeCalls = 0;
        var serviceProviderCalls = 0;
        var getServiceCalls = 0;
        var tryProcessCalls = 0;
        var disposeCalls = 0;

        fetcher.Setup(x => x.ReadOutboxMessagesAsync(tokenSource.Token))
            .Callback(() => readOutboxMessagesCalls++)
            .ReturnsAsync([msg]);

        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        scopedFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);

        var provider = new Mock<IServiceProvider>(MockBehavior.Strict);
        scope.SetupGet(x => x.ServiceProvider)
            .Callback(() => serviceProviderCalls++)
            .Returns(provider.Object);
        scope.Setup(x => x.Dispose()).Callback(() => disposeCalls++);

        var processor = new Mock<IOutboxMessageProcessor>(MockBehavior.Strict);
        provider.Setup(x => x.GetService(typeof(IOutboxMessageProcessor)))
            .Callback(() => getServiceCalls++)
            .Returns(processor.Object);
        processor.Setup(x => x.TryProcessAsync(msg, tokenSource.Token))
            .Callback(() => tryProcessCalls++)
            .ReturnsAsync(true);

        // Act
        var result = await outBox.RunProcessingAsync(tokenSource.Token);

        // Assert
        result.Should().BeTrue();
        readOutboxMessagesCalls.Should().Be(1);
        createScopeCalls.Should().Be(1);
        serviceProviderCalls.Should().Be(1);
        getServiceCalls.Should().Be(1);
        tryProcessCalls.Should().Be(1);
        disposeCalls.Should().Be(1);
    }

    [Fact(DisplayName = "Outbox returns false when there are no messages.")]
    [Trait("Category", "Unit")]
    public async Task RunProcessingAsyncReturnsFalseWhenNoMessagesAsync()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = new Mock<ILogger<Outbox>>();
        var outBox = new Outbox(fetcher.Object, scopedFactory.Object, logger.Object);
        using var tokenSource = new CancellationTokenSource();

        var readOutboxMessagesCalls = 0;
        var createScopeCalls = 0;

        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var provider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var processor = new Mock<IOutboxMessageProcessor>(MockBehavior.Strict);

        fetcher.Setup(x => x.ReadOutboxMessagesAsync(tokenSource.Token))
            .Callback(() => readOutboxMessagesCalls++)
            .ReturnsAsync([]);
        scopedFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.SetupGet(x => x.ServiceProvider).Returns(provider.Object);
        scope.Setup(x => x.Dispose());
        provider.Setup(x => x.GetService(typeof(IOutboxMessageProcessor))).Returns(processor.Object);
        processor.Setup(x => x.TryProcessAsync(It.IsAny<IOutboxMessage>(), tokenSource.Token)).ReturnsAsync(true);

        // Act
        var result = await outBox.RunProcessingAsync(tokenSource.Token);

        // Assert
        result.Should().BeFalse();
        readOutboxMessagesCalls.Should().Be(1);
        createScopeCalls.Should().Be(0);
    }

    [Fact(DisplayName = "Outbox stops processing after first failed message and returns true.")]
    [Trait("Category", "Unit")]
    public async Task RunProcessingAsyncStopsOnFirstFailedMessageAsync()
    {
        // Arrange
        var fetcher = new Mock<IOutboxFetcher>(MockBehavior.Strict);
        var scopedFactory = new Mock<IServiceScopeFactory>(MockBehavior.Strict);
        var logger = new Mock<ILogger<Outbox>>();
        var outBox = new Outbox(fetcher.Object, scopedFactory.Object, logger.Object);
        using var tokenSource = new CancellationTokenSource();

        var msg1 = new TestMessage();
        var msg2 = new TestMessage();
        var readOutboxMessagesCalls = 0;
        var createScopeCalls = 0;
        var tryProcessMsg1Calls = 0;
        var tryProcessMsg2Calls = 0;
        var disposeCalls = 0;

        fetcher.Setup(x => x.ReadOutboxMessagesAsync(tokenSource.Token))
            .Callback(() => readOutboxMessagesCalls++)
            .ReturnsAsync([msg1, msg2]);

        var scope = new Mock<IServiceScope>(MockBehavior.Strict);
        var provider = new Mock<IServiceProvider>(MockBehavior.Strict);
        var processor = new Mock<IOutboxMessageProcessor>(MockBehavior.Strict);

        scopedFactory.Setup(x => x.CreateScope())
            .Callback(() => createScopeCalls++)
            .Returns(scope.Object);
        scope.Setup(x => x.ServiceProvider).Returns(provider.Object);
        scope.Setup(x => x.Dispose()).Callback(() => disposeCalls++);
        provider.Setup(x => x.GetService(typeof(IOutboxMessageProcessor))).Returns(processor.Object);
        processor.Setup(x => x.TryProcessAsync(msg1, tokenSource.Token))
            .Callback(() => tryProcessMsg1Calls++)
            .ReturnsAsync(false);
        processor.Setup(x => x.TryProcessAsync(msg2, tokenSource.Token))
            .Callback(() => tryProcessMsg2Calls++)
            .ReturnsAsync(true);

        // Act
        var result = await outBox.RunProcessingAsync(tokenSource.Token);

        // Assert
        result.Should().BeTrue();
        readOutboxMessagesCalls.Should().Be(1);
        createScopeCalls.Should().Be(1);
        tryProcessMsg1Calls.Should().Be(1);
        tryProcessMsg2Calls.Should().Be(0);
        disposeCalls.Should().Be(1);
    }
}
