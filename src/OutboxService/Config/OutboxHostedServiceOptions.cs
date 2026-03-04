namespace OutboxService.Config;

/// <summary>
/// Options for Outbox service.
/// </summary>
public class OutboxHostedServiceOptions
{
    /// <summary>
    /// Minimum delay between polling attempts.
    /// </summary>
    public TimeSpan MinDelay { get; init; } = TimeSpan.FromMilliseconds(10);

    /// <summary>
    /// Maximum delay between polling attempts.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Number of empty polling attempts required to reach the maximum delay.
    /// </summary>
    public int StepsCount { get; init; } = 10;
}
