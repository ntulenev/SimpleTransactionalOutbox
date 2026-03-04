using System.Diagnostics;

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
    /// <param name="logger">Logger.</param>
    /// <param name="hostApplicationLifetime">Lifetime app param.</param>
    /// <param name="serviceScopeFactory">Factory that creates DI Scopes.</param>
    /// <param name="delayProvider">Provider for idle polling delays.</param>
    public OutboxHostedService(ILogger<OutboxHostedService> logger,
                         IHostApplicationLifetime hostApplicationLifetime,
                         IServiceScopeFactory serviceScopeFactory,
                         IOutboxBackoffDelayProvider delayProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _delayProvider = delayProvider ?? throw new ArgumentNullException(nameof(delayProvider));
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (true)
        {
            try
            {
                _hostApplicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

                var processedAny = false;

                using var scope = _serviceScopeFactory.CreateScope();
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    processedAny = await outbox.RunProcessingAsync(_hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                }

                if (!processedAny)
                {
                    var delay = _delayProvider.GetNextDelay();
                    await Task.Delay(delay, _hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
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
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;
}
