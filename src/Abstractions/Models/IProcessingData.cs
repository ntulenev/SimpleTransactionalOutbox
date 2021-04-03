namespace Abstractions.Models
{
    /// <summary>
    /// Data model interface.
    /// </summary>
    public interface IProcessingData
    {
        /// <summary>
        /// Id of the processing data.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Value of the proccesing data.
        /// </summary>
        public int Value { get; }
    }
}
