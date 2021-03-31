using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.Service
{
    public interface IOutboxMessageProcessor
    {
        Task<bool> TryProcessAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
