using System.Threading;
using System.Threading.Tasks;

namespace Abstractions.DB
{
    /// <summary>
    /// Interface that represents UnitOfWork pattern.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commits operation results to the storage.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
