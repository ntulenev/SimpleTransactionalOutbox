namespace Abstractions.Models;

/// <summary>
/// Outbox message interface.
/// </summary>
public interface IOutboxMessage
{
    /// <summary>
    /// Id of the message.
    /// </summary>
    Guid MessageId { get; }

    /// <summary>
    /// Creation date/time.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Type of outbox message.
    /// </summary>
    OutboxMessageType MessageType { get; }

    /// <summary>
    /// Message body.
    /// </summary>
    string Body { get; }
}


