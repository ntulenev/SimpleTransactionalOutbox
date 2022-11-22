using Abstractions.Models;

namespace DB;

/// <summary>
/// DB Outbox message model.
/// </summary>
public class OutboxMessage
{
    /// <summary>
    /// Message Id.
    /// </summary>
    public Guid MessageId { get; }

    /// <summary>
    /// Message created date/time.
    /// </summary>
    public DateTime OccurredOn { get; set; }

    /// <summary>
    /// Message type.
    /// </summary>
    public OutboxMessageType MessageType { get; set; }

    /// <summary>
    /// Message body.
    /// </summary>
    public string Body { get; set; } = default!;

    /// <summary>
    /// Creates <see cref="OutboxMessage"/>.
    /// </summary>
    public OutboxMessage()
    {
        MessageId = Guid.NewGuid();
    }
}
