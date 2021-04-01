namespace Abstractions.Serialization
{
    public interface IDeserializer<T>
    {
        public T Deserialize(string obj);
    }
}
