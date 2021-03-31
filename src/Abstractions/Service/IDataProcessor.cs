using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.Service
{
    public interface IDataProcessor
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
