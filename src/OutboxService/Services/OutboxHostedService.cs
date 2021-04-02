using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Abstractions.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OutboxService.Config;

namespace OutboxService.Services
{
    public class OutboxHostedService : IHostedService
    {
        public OutboxHostedService(ILogger<OutboxHostedService> logger,
                             IOutbox outbox,
                             IHostApplicationLifetime hostApplicationLifetime,
                             IOptions<OutboxHostedServiceOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _outbox = outbox ?? throw new ArgumentNullException(nameof(outbox));
            _hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value is null)
            {
                throw new ArgumentException(nameof(options));
            }

            _delay = options.Value.DelayInSeconds;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _outboxTask = Task.Run(async () =>
            {
                while (!_hostApplicationLifetime.ApplicationStopping.IsCancellationRequested)
                {
                    await _outbox.RunProcessingAsync(_hostApplicationLifetime.ApplicationStopping).ConfigureAwait(false);
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

        private readonly int _delay;
        private readonly ILogger<OutboxHostedService> _logger;
        private readonly IOutbox _outbox;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
    }
}
