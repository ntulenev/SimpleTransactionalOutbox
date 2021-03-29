using System.Threading;
using System.Threading.Tasks;

using Infracructure.Models;

namespace Infracructure.Service
{
    public interface IDataProcessor
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
