using Newtonsoft.Json;

using Abstractions.Serialization;

namespace Serialization;

/// <summary>
/// Json serializer.
/// </summary>
/// <typeparam name="T">Type of model for serialization.</typeparam>
public class JsonSerializer<T> : ISerializer<T>
{
    /// <inheritdoc/>
    public string Serialize(T obj)
    {
        if (obj is null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return JsonConvert.SerializeObject(obj);
    }
}
