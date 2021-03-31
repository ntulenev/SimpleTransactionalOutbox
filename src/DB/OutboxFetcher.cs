using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Abstractions.DB;
using Abstractions.Models;
using System.Linq;

namespace DB
{
    public class OutboxFetcher : IOutboxFetcher
    {

        public OutboxFetcher(
            OutboxContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.OutboxMessages
                                 .OrderBy(x => x.OccurredOn)
                                 .Take(LIMIT)
                                 .Select(x => new Models.OutboxMessage(x.MessageId, x.OccurredOn, x.MessageType, x.Body))
                                 .ToListAsync(cancellationToken)
                                 .ConfigureAwait(false);
        }

        private readonly OutboxContext _context;

        private const int LIMIT = 10;

    }
}
