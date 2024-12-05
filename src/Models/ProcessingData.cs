using Abstractions.Models;

namespace Models;

/// <summary>
/// Processing data message.
/// </summary>
/// <remarks>
/// Creates <see cref="ProcessingData"/>.
/// </remarks>
/// <param name="id">Data id.</param>
/// <param name="value">Data value.</param>
public class ProcessingData(int id, int value) : IProcessingData
{
    /// <inheritdoc/>
    public long Id { get; } = id;

    /// <inheritdoc/>
    public int Value { get; } = value;
}
