using Abstractions.Models;

namespace Abstractions.DB;

/// <summary>
/// Outbox messages fetcher.
/// </summary>
public interface IOutboxFetcher
{
    /// <summary>
    /// Pools actual outbox messages from storage.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of actual outbox messages</returns>
    Task<IReadOnlyCollection<IOutboxMessage>> ReadOutboxMessagesAsync(CancellationToken cancellationToken = default);
}
