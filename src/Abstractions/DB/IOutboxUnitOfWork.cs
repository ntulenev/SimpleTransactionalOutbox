using Abstractions.Models;

namespace Abstractions.DB
{
    /// <summary>
    /// <see cref="IUnitOfWork"/> for outbox logic.
    /// </summary>
    public interface IOutboxUnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Deletes not actual outbox message.
        /// </summary>
        /// <param name="message">Outbox message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task RemoveOutboxMessageAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
    }
}
