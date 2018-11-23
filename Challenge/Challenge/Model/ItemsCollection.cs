using System.Collections.Generic;
using Newtonsoft.Json;

namespace Challenge.Model
{
    public class ItemsCollection<T> where T : class
    {
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }

        [JsonProperty("items")]
        public IEnumerable<T> Items { get; set; }
    }
}
