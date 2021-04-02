using System.Collections.Generic;

namespace OutboxService.Config
{
    public class KafkaProducerOptions
    {
        public List<string>? BootstrapServers { get; set; }
    }
}
