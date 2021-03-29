using System.Threading;
using System.Threading.Tasks;

namespace Infracructure.DB
{
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
