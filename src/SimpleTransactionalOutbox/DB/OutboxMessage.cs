using System;

using Abstractions.Models;

namespace DB
{
    public class OutboxMessage
    {
        public Guid MessageId { get; }

        public DateTime OccurredOn { get; set; }

        public OutboxMessageType MessageType { get; set; }

        public string Body { get; set; } = default!;

        public OutboxMessage()
        {
            MessageId = Guid.NewGuid();
        }
    }
}
