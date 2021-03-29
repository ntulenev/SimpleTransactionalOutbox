using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Infracructure.Models;

namespace Infracructure.DB
{
    public interface IProcessingOutboxUnitOfWork : IUnitOfWork
    {
        Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default);

        Task RemoveOutboxMessageAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
