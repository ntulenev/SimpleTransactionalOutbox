using Abstractions.Service;

namespace OutboxService.Services;

/// <summary>
/// Outbox processing loop implementation.
/// </summary>
public class OutboxProcessor : IOutboxProcessor
{
    /// <summary>
    /// Creates <see cref="OutboxProcessor"/>.
    /// </summary>
    /// <param name="logger">Logger.</param>
    /// <param name="serviceScopeFactory">Factory that creates DI scopes.</param>
    /// <param name="delayProvider">Provider for idle polling delays.</param>
    public OutboxProcessor(
        ILogger<OutboxProcessor> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOutboxBackoffDelayProvider delayProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _delayProvider = delayProvider ?? throw new ArgumentNullException(nameof(delayProvider));
    }

    /// <inheritdoc/>
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var processedAny = false;

                using var scope = _serviceScopeFactory.CreateScope();
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    processedAny = await outbox.RunProcessingAsync(cancellationToken).ConfigureAwait(false);
                }

                if (!processedAny)
                {
                    var delay = _delayProvider.GetNextDelay();
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    _delayProvider.Reset();
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error while processing outbox");
                throw;
            }
        }
    }

    private readonly IOutboxBackoffDelayProvider _delayProvider;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
}
