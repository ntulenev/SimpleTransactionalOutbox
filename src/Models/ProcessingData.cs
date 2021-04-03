using Abstractions.Models;

namespace Models
{
    /// <summary>
    /// Processing data message.
    /// </summary>
    public class ProcessingData : IProcessingData
    {
        /// <inheritdoc/>
        public long Id { get; }

        /// <inheritdoc/>
        public int Value { get; }

        /// <summary>
        /// Creates <see cref="ProcessingData"/>.
        /// </summary>
        /// <param name="id">Data id.</param>
        /// <param name="value">Data value.</param>
        public ProcessingData(int id, int value)
        {
            Id = id;
            Value = value;
        }
    }
}
