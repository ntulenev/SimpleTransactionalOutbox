using System;

namespace Abstractions.Models
{
    public interface IOutboxMessage
    {
        public Guid MessageId { get; }

        public DateTime OccurredOn { get; }

        public OutboxMessageType MessageType { get; }

        public string Body { get; }  
    }

 
}
