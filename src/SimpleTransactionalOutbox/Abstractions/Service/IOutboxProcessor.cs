using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.Service
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
