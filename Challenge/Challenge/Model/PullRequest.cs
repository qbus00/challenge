using Newtonsoft.Json;

namespace Challenge.Model
{
    public class PullRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("html_url")]
        public string HtmlUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}
