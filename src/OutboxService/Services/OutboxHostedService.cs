using System.Diagnostics;

using Microsoft.Extensions.Options;

using Abstractions.Service;
using OutboxService.Config;

namespace OutboxService.Services;

/// <summary>
/// Outbox processing service.
/// </summary>
public class OutboxHostedService : IHostedService
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

    /// <summary>
    /// Starts service logic.
    /// </summary>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _outboxTask = Task.Run(async () =>
        {
            while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                {
                    var outbox = scope.ServiceProvider.GetRequiredService<IOutbox>();
                    await outbox.RunProcessingAsync(_hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
                }

                await Task.Delay(_delay).ConfigureAwait(false);
            }
        }, _hostApplicationLifetime.ApplicationStopping);

        _ = _outboxTask.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                _logger?.LogError("Outbox processing encountered error", t.Exception);

                _logger?.LogInformation("Stopping the application");

                _hostApplicationLifetime.StopApplication();
            }
        });

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops service logic.
    /// </summary>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            Debug.Assert(_outboxTask != null);
            await _outboxTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }

        _logger?.LogInformation("The service is stopped");
    }

    private Task? _outboxTask;

    private readonly TimeSpan _delay;
    private readonly ILogger<OutboxHostedService> _logger;
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IServiceScopeFactory _serviceScopeFactory;
}
