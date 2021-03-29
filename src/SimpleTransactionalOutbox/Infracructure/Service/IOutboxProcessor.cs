using System.Threading;
using System.Threading.Tasks;

using Infracructure.Models;

namespace Infracructure.Service
{
    public interface IOutboxProcessor
    {
        Task ProcessAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
