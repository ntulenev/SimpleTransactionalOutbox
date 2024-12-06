namespace Transport;

/// <summary>
/// Config options for Kafka sender.
/// </summary>
public class KafkaOutboxSenderOptions
{
    /// <summary>
    /// Kafka topic name.
    /// </summary>
    public required string TopicName { get; init; }
}
