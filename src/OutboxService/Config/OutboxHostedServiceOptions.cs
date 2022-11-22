namespace OutboxService.Config;

/// <summary>
/// Options for Outbox service.
/// </summary>
public class OutboxHostedServiceOptions
{
    /// <summary>
    /// Starting interval (in seconds).
    /// </summary>
    public int DelayInSeconds { get; set; }
}
