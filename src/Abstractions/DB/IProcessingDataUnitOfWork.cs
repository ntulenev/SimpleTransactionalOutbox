using System.Threading;
using System.Threading.Tasks;

using Abstractions.Models;

namespace Abstractions.DB
{
    public interface IProcessingDataUnitOfWork : IUnitOfWork
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
