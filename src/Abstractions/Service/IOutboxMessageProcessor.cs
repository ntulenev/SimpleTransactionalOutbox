using Abstractions.Models;

namespace Abstractions.Service;

/// <summary>
/// Processor for single outbox message.
/// </summary>
public interface IOutboxMessageProcessor
{
    /// <summary>
    /// Attempts to process single outbox message.
    /// </summary>
    /// <param name="message">Outbox message for processing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status of operation (true if success, false otherwise).</returns>
    Task<bool> TryProcessAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
}
