using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.DB
{
    public interface IUnitOfWork
    {
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
