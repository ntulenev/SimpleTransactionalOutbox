using System;

using Abstractions.Models;

namespace Logic.Tests
{
    public class TestMessage : IOutboxMessage
    {
        public Guid MessageId { get; set; }

        public DateTime OccurredOn { get; set; }

        public OutboxMessageType MessageType { get; set; }

        public string Body { get; set; } = default!;
    }
}
