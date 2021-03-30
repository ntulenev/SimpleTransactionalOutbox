using System.Threading;
using System.Threading.Tasks;

using Infracructure.Models;

namespace Infracructure.DB
{
    public interface IProcessingDataUnitOfWork : IUnitOfWork
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
