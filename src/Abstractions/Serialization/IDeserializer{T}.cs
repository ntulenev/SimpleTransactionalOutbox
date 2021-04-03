namespace Abstractions.Serialization
{
    /// <summary>
    /// Deserializer string to model representation.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    public interface IDeserializer<T>
    {
        /// <summary>
        /// Deserializes string to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="obj">Data in string format.</param>
        /// <returns>Deserialized model.</returns>
        public T Deserialize(string obj);
    }
}
