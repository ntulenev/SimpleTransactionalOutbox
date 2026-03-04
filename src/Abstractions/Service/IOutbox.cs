namespace Abstractions.Service;

/// <summary>
/// Outbox service interface.
/// </summary>
public interface IOutbox
{
    /// <summary>
    /// Processes outbox work phase.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// <see langword="true"/> when there were messages to process; otherwise
    /// <see langword="false"/>.
    /// </returns>
    Task<bool> RunProcessingAsync(CancellationToken cancellationToken);
}
