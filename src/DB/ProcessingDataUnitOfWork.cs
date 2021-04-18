using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Abstractions.Serialization;
using Abstractions.DB;
using Abstractions.Models;

namespace DB
{
    /// <summary>
    /// Unit of work for ProcessingData logic.
    /// </summary>
    public class ProcessingDataUnitOfWork : UnitOfWork<OutboxContext>, IProcessingDataUnitOfWork
    {
        /// <summary>
        /// Creates <see cref="ProcessingDataUnitOfWork"/>.
        /// </summary>
        /// <param name="context">Databaes contex.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="serializer">Serializer.</param>
        public ProcessingDataUnitOfWork(
             OutboxContext context,
             ILogger<ProcessingDataUnitOfWork> logger,
             ISerializer<IProcessingData> serializer)
            : base(context, System.Data.IsolationLevel.Serializable, logger)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <inheritdoc/>
        public async Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _logger.LogInformation("Starting process {@data}.", data);

            var item = await _context.ProcessingData.SingleOrDefaultAsync(x => x.Id == data.Id, cancellationToken).ConfigureAwait(false);

            if (item is not null)
            {
                _logger.LogInformation("Updating exists data from {oldValue} => {newValue}.", item.Value, data.Value);

                item.Value = data.Value;
            }
            else
            {
                _logger.LogInformation("Creating new data item.");

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
