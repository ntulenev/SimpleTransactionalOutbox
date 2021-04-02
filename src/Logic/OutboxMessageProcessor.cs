using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Abstractions.Bus;
using Abstractions.DB;
using Abstractions.Models;
using Abstractions.Service;

namespace Logic
{
    public class OutboxMessageProcessor : IOutboxMessageProcessor
    {
        public OutboxMessageProcessor(IOutboxUnitOfWork uow,
                                      IOutboxSender sender,
                                      ILogger<OutboxMessageProcessor> logger
                                      )
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> TryProcessAsync(IOutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            using var scope = _logger.BeginScope("Processing outbox message {@message}.", message);

            try
            {
                _logger.LogInformation("Start sending message.");
                await _sender.SendAsync(message, cancellationToken).ConfigureAwait(false);

                _logger.LogInformation("Start removing outbox message.");

                await _uow.RemoveOutboxMessageAsync(message, cancellationToken).ConfigureAwait(false);

                await _uow.SaveAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on processing outbox message.");
                return false;
            }
        }

        private readonly IOutboxUnitOfWork _uow;
        private readonly IOutboxSender _sender;
        private readonly ILogger<OutboxMessageProcessor> _logger;
    }
}
