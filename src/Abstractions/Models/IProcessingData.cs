namespace Abstractions.Models;

/// <summary>
/// Data model interface.
/// </summary>
public interface IProcessingData
{
    /// <summary>
    /// Id of the processing data.
    /// </summary>
    long Id { get; }

    /// <summary>
    /// Value of the processing data.
    /// </summary>
    int Value { get; }
}
