using System.Threading;
using System.Threading.Tasks;

using Infracructure.Domain.Models;

namespace Infracructure.Domain.DB
{
    interface IProcessingDataUnitOfWork : IUnitOfWork
    {
        Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
    }
}
