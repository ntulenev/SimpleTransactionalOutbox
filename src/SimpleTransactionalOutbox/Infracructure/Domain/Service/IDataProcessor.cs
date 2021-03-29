using System.Threading;
using System.Threading.Tasks;

using Infracructure.Domain.Models;

namespace Infracructure.Domain.Service
{
    public interface IDataProcessor
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
