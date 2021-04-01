﻿using System;

using Newtonsoft.Json;

using Abstractions.Serialization;

namespace Serialization
{
    class JsonDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(string obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentException("Message is empty of contains only spaces.", nameof(obj));
            }

            return JsonConvert.DeserializeObject<T>(obj)!;
        }
    }
}
