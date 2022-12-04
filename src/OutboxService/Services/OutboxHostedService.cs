using System.Diagnostics;

using Microsoft.Extensions.Options;

using Abstractions.Service;
using OutboxService.Config;

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
    /// <param name="options">Service configuration.</param>
    /// <param name="serviceScopeFactory">Factory that creates DI Scopes.</param>
    public OutboxHostedService(ILogger<OutboxHostedService> logger,
                         IHostApplicationLifetime hostApplicationLifetime,
                         IOptions<OutboxHostedServiceOptions> options,
                         IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        ArgumentNullException.ThrowIfNull(options);

        if (options.Value is null)
        {
            throw new ArgumentException("Options value is not set", nameof(options));
        }

        _delay = TimeSpan.FromSeconds(options.Value.DelayInSeconds);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    _hostApplicationLifetime.ApplicationStopping.ThrowIfCancellationRequested();

                    using var scope = _serviceScopeFactory.CreateScope();
                    {
                        var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                        await outbox.RunProcessingAsync(_hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                    }

                    await Task.Delay(_delay).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    _logger.LogError(ex, "Error while processing outbox");
                    throw;
                }
            }
        }, _hostApplicationLifetime.ApplicationStopping);
    }

    private readonly TimeSpan _delay;
    private readonly ILogger<OutboxHostedService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;
}
