﻿namespace OutboxService.Config;

/// <summary>
/// Config options for Kafka producer.
/// </summary>
public class KafkaProducerOptions
{
    /// <summary>
    /// List of Kafka bootstrap servers.
    /// </summary>
    public List<string>? BootstrapServers { get; set; }
}
