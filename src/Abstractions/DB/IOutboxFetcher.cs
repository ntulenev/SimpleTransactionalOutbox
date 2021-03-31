using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.DB
{
    public interface IOutboxFetcher
    {
        Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default);
    }
}
