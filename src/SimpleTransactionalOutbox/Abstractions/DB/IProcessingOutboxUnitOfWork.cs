using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.DB
{
    public interface IProcessingOutboxUnitOfWork : IUnitOfWork
    {
        Task RemoveOutboxMessageAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
