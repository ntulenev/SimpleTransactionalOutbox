using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Models;

namespace DB
{
    public class OutboxUnitOfWork : UnitOfWork<OutboxContext>, IOutboxUnitOfWork
    {
        public OutboxUnitOfWork(
            OutboxContext context,
            ILogger<ProcessingDataUnitOfWork> logger)
           : base(context, System.Data.IsolationLevel.Serializable, logger)
        {
        }

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
