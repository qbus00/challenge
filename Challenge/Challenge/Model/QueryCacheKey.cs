using Newtonsoft.Json;

namespace Challenge.Model
{
    public class QueryCacheKey
    {
        public int Page { get; set; }
        public string SearchPhrase { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
