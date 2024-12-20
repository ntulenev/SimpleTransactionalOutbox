﻿namespace OutboxService.Config;

/// <summary>
/// Options for Outbox service.
/// </summary>
public class OutboxHostedServiceOptions
{
    /// <summary>
    /// Starting interval.
    /// </summary>
    public required TimeSpan Delay { get; init; }
}
