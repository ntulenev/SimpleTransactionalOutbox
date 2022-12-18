using Abstractions.Models;

namespace Abstractions.Bus;

/// <summary>
/// Message publisher.
/// </summary>
public interface IOutboxSender
{
    /// <summary>
    /// Publishes <paramref name="message"/> to external system.
    /// </summary>
    /// <param name="message">Message to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default);
}
