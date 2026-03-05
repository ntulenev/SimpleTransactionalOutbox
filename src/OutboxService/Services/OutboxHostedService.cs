using Abstractions.Service;

namespace OutboxService.Services;

/// <summary>
/// Outbox processing service.
/// </summary>
public class OutboxHostedService : BackgroundService
{
    /// <summary>
    /// Creates <see cref="OutboxHostedService"/>.
    /// </summary>
    /// <param name="processor">Outbox processor.</param>
    public OutboxHostedService(IOutboxProcessor processor)
    {
        ArgumentNullException.ThrowIfNull(processor);
        _processor = processor;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        _processor.ProcessAsync(stoppingToken);

    private readonly IOutboxProcessor _processor;
}
