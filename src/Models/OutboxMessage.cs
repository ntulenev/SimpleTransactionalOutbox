using System.ComponentModel;

using Abstractions.Models;

namespace Models
{
    /// <summary>
    /// Outbox message model.
    /// </summary>
    public class OutboxMessage : IOutboxMessage
    {
        /// <inheritdoc/>
        public Guid MessageId { get; }

        /// <inheritdoc/>
        public DateTime OccurredOn { get; }

        /// <inheritdoc/>
        public OutboxMessageType MessageType { get; }

        /// <inheritdoc/>
        public string Body { get; }

        /// <summary>
        /// Creates <see cref="OutboxMessage"/>.
        /// </summary>
        /// <param name="messageId">Message id.</param>
        /// <param name="occurredOn">Created date/time.</param>
        /// <param name="type">Message type.</param>
        /// <param name="body">Message body.</param>
        public OutboxMessage(Guid messageId, DateTime occurredOn, OutboxMessageType type, string body)
        {
            if (body is null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new ArgumentException("Body is empty or contains only whitespaces.", nameof(body));
            }

            if (!Enum.IsDefined(typeof(OutboxMessageType), type))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int)type, typeof(OutboxMessageType));
            }

            MessageId = messageId;
            OccurredOn = occurredOn;
            MessageType = type;
            Body = body;
        }
    }
}
