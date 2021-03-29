using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Infracructure.Domain.Models;

namespace Infracructure.Domain.DB
{
    public interface IProcessingOutboxUnitOfWork : IUnitOfWork
    {
        Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default);

        Task RemoveOutboxMessageAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
