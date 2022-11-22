namespace Abstractions.Models;

/// <summary>
/// Outbox message interface.
/// </summary>
public interface IOutboxMessage
{
    /// <summary>
    /// Id of the message.
    /// </summary>
    public Guid MessageId { get; }

    /// <summary>
    /// Creation date/time.
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Type of outbox message.
    /// </summary>
    public OutboxMessageType MessageType { get; }

    /// <summary>
    /// Message body.
    /// </summary>
    public string Body { get; }  
}


