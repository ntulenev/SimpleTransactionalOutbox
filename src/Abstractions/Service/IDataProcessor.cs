﻿using Abstractions.Models;

namespace Abstractions.Service;

/// <summary>
/// Processor for incoming data message.
/// </summary>
public interface IDataProcessor
{
    /// <summary>
    /// Handles new data item.
    /// </summary>
    /// <param name="data">Processing data item to be added to the system.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ProcessDataAsync(IProcessingData data, CancellationToken cancellationToken = default);
}
