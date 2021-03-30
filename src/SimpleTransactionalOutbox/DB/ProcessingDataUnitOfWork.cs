using Abstractions.DB;
using Abstractions.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Logging;

using Abstractions.Serialization;

namespace DB
{
    public class ProcessingDataUnitOfWork : UnitOfWork<OutboxContext>, IProcessingDataUnitOfWork
    {
        public ProcessingDataUnitOfWork(
             OutboxContext context,
             ILogger<ProcessingDataUnitOfWork> logger,
             ISerializer<IProcessingData> serializer)
            : base(context, System.Data.IsolationLevel.Serializable, logger)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var item = await _context.ProcessingData.SingleOrDefaultAsync(x => x.Id == data.Id, cancellationToken).ConfigureAwait(false);

            if (item is not null)
            {
                item.Value = data.Value;
            }
            else
            {
                _context.ProcessingData.Add(new ProcessingData
                {
                    Id = data.Id,
                    Value = data.Value
                });
            }

            _context.OutboxMessages.Add(new OutboxMessage()
            {
                MessageType = OutboxMessageType.ProcessingDataMessage,
                OccurredOn = DateTime.UtcNow,
                Body = _serializer.Serialize(data)
            });
        }

        private readonly ISerializer<IProcessingData> _serializer;
    }
}
