using Abstractions.Models;

namespace Models
{
    public class ProcessingData : IProcessingData
    {
        public long Id { get; }

        public int Value { get; }

        public ProcessingData(int id, int value)
        {
            Id = id;
            Value = value;
        }
    }
}
