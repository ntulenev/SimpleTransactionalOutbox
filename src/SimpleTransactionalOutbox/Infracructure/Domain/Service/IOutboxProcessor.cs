using System.Threading;
using System.Threading.Tasks;

using Infracructure.Domain.Models;

namespace Infracructure.Domain.Service
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
