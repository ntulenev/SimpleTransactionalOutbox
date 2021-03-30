using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Models;

namespace DB
{
    public class OutboxFetcherUnitOfWork : UnitOfWork<OutboxContext>, IOutboxFetcherUnitOfWork
    {

        public OutboxFetcherUnitOfWork(
            OutboxContext context,
            ILogger<OutboxFetcherUnitOfWork> logger)
           : base(context, isolationLevel: null, logger)
        {

        }

        public async Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            throw new NotImplementedException();
        }
    }
}
