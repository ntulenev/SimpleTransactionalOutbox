namespace Abstractions.Service;

/// <summary>
/// Provides adaptive delay values for outbox polling.
/// </summary>
public interface IOutboxBackoffDelayProvider
{
    /// <summary>
    /// Returns the next delay for an empty polling attempt.
    /// </summary>
    /// <returns>Delay to wait before the next polling attempt.</returns>
    TimeSpan GetNextDelay();

    /// <summary>
    /// Resets the delay sequence to the minimum value.
    /// </summary>
    void Reset();
}
