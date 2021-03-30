using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Models;

namespace DB
{
    public class OutboxFetcher : IOutboxFetcher
    {

        public OutboxFetcher(
            OutboxContext context,
            ILogger<OutboxFetcher> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }

        private readonly OutboxContext _context;
        private readonly ILogger<OutboxFetcher> _logger;

    }
}
