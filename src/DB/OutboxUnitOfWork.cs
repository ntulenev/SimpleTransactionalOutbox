using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Models;

namespace DB
{
    /// <summary>
    /// Unit of work for outbox logic.
    /// </summary>
    public class OutboxUnitOfWork : UnitOfWork<OutboxContext>, IOutboxUnitOfWork
    {
        /// <summary>
        /// Creates <see cref="OutboxUnitOfWork"/>.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger.</param>
        public OutboxUnitOfWork(
            OutboxContext context,
            ILogger<OutboxUnitOfWork> logger)
           : base(context, System.Data.IsolationLevel.Serializable, logger)
        {
        }

        /// <inheritdoc/>
        public async Task RemoveOutboxMessageAsync(IOutboxMessage message, CancellationToken cancellationToken = default)
        {

            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _logger.LogInformation("Removing message {@message}.", message);

            var item = await _context.OutboxMessages.SingleAsync(x => x.MessageId == message.MessageId, cancellationToken).ConfigureAwait(false);

            _context.OutboxMessages.Remove(item);
        }
    }
}
