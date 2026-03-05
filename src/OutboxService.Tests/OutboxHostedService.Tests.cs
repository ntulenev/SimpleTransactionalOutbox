using Abstractions.Service;

using FluentAssertions;

using Moq;

using OutboxService.Services;

using Xunit;

namespace OutboxService.Tests;

public class OutboxHostedServiceTests
{
    [Fact(DisplayName = "OutboxHostedService cant be created with null processor.")]
    [Trait("Category", "Unit")]
    public void CantCreateWithNullProcessor()
    {
        // Arrange
        var processor = (IOutboxProcessor)null!;

        // Act
        var exception = Record.Exception(() => _ = new OutboxHostedService(processor));

        // Assert
        exception.Should().NotBeNull().And.BeOfType<ArgumentNullException>();
    }

    [Fact(DisplayName = "OutboxHostedService delegates execution to processor.")]
    [Trait("Category", "Unit")]
    public async Task ExecuteAsyncDelegatesToProcessorAsync()
    {
        // Arrange
        var processor = new Mock<IOutboxProcessor>(MockBehavior.Strict);
        using var cts = new CancellationTokenSource();
        processor.Setup(x => x.ProcessAsync(cts.Token)).Returns(Task.CompletedTask);

        using var service = new TestableOutboxHostedService(processor.Object);

        // Act
        var exception = await Record.ExceptionAsync(async () => await service.InvokeExecuteAsync(cts.Token));

        // Assert
        exception.Should().BeNull();
        processor.Verify(x => x.ProcessAsync(cts.Token), Times.Once);
    }

    private sealed class TestableOutboxHostedService(IOutboxProcessor processor) : OutboxHostedService(processor)
    {
        public Task InvokeExecuteAsync(CancellationToken stoppingToken) => ExecuteAsync(stoppingToken);
    }
}
