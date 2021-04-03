namespace Abstractions.Serialization
{
    /// <summary>
    /// Serialize model to string representation.
    /// </summary>
    /// <typeparam name="T">Model type.</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serializes model to string format.
        /// </summary>
        /// <param name="obj">Model to be serialized.</param>
        /// <returns>String representation of the model.</returns>
        public string Serialize(T obj);
    }
}
