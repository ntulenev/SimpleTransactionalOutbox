using Newtonsoft.Json;

namespace Serialization.Tests
{
    class TestClass
    {
        [JsonProperty("test_id")]
        public int Id { get; set; }

        [JsonProperty("test_value")]
        public string Value { get; set; } = default!;
    }
}
