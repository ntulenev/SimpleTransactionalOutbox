using System;

using Newtonsoft.Json;

using Abstractions.Serialization;

namespace Serialization
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public string Serialize(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return JsonConvert.SerializeObject(obj);
        }
    }
}
