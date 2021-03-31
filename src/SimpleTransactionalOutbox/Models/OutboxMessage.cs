using System;
using System.ComponentModel;

using Abstractions.Models;

namespace Models
{
    public class OutboxMessage : IOutboxMessage
    {
        public Guid MessageId { get; }

        public DateTime OccurredOn { get; }

        public OutboxMessageType MessageType { get; }

        public string Body { get; }

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
