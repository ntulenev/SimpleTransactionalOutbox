using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Abstractions.DB;
using Abstractions.Models;
using Abstractions.Service;


namespace Logic
{
    /// <summary>
    /// Processor for incomming data message.
    /// </summary>
    public class DataProcessor : IDataProcessor
    {

        /// <summary>
        /// Creates <see cref="DataProcessor"/>.
        /// </summary>
        /// <param name="uow">Unit of work.</param>
        /// <param name="logger">Logger.</param>
        public DataProcessor(IProcessingDataUnitOfWork uow,
                             ILogger<DataProcessor> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var _ = _logger.BeginScope("Processing data {@data}.");

            _logger.LogInformation("Start processing.");

            await _uow.ProcessDataAsync(data, cancellationToken).ConfigureAwait(false);

            await _uow.SaveAsync(cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("End processing.");
        }

        private readonly IProcessingDataUnitOfWork _uow;
        private readonly ILogger<DataProcessor> _logger;
    }
}
