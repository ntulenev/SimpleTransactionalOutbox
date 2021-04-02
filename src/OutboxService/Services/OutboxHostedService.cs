using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Abstractions.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OutboxService.Config;

namespace OutboxService.Services
{
    public class OutboxHostedService : IHostedService
    {
        public OutboxHostedService(ILogger<OutboxHostedService> logger,
                             IHostApplicationLifetime hostApplicationLifetime,
                             IOptions<OutboxHostedServiceOptions> options,
                             IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value is null)
            {
                throw new ArgumentException(nameof(options));
            }

            _delay = TimeSpan.FromSeconds(options.Value.DelayInSeconds);
        }

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
}
