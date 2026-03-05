namespace Abstractions.Service;

/// <summary>
/// Outbox processor loop abstraction.
/// </summary>
public interface IOutboxProcessor
{
    /// <summary>
    /// Runs outbox processing loop until cancellation.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
