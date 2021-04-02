using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Confluent.Kafka;

using Abstractions.Bus;
using Abstractions.Models;

using AS = Abstractions.Serialization;

namespace Transport
{
    public class KafkaOutboxSender : IOutboxSender
    {
        public KafkaOutboxSender(IProducer<Null, string> producer,
                                 AS.ISerializer<IOutboxMessage> serializer,
                                 IOptions<KafkaOutboxSenderOptions> options,
                                 ILogger<KafkaOutboxSender> logger
                                )
        {
            _producer = producer ?? throw new ArgumentNullException(nameof(producer));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (options.Value is null)
            {
                throw new ArgumentException("Options is not set.", nameof(options));
            }

            _topicName = options.Value.TopicName;
        }

        public async Task SendAsync(IOutboxMessage message, CancellationToken cancellationToken = default)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var strMessage = _serializer.Serialize(message);

            try
            {
                _logger.LogInformation("Sending data {strMessage}", strMessage);

                var dr = await _producer.ProduceAsync(_topicName, new Message<Null, string>
                {
                    Value = strMessage
                }, cancellationToken).ConfigureAwait(false);

            }
            catch (ProduceException<Null, string> e)
            {
                _logger.LogError(e, $"Delivery failed: {e.Error.Reason}");
                throw;
            }
        }

        private readonly string _topicName;
        private readonly IProducer<Null, string> _producer;
        private readonly ILogger<KafkaOutboxSender> _logger;
        private readonly AS.ISerializer<IOutboxMessage> _serializer;
    }
}
