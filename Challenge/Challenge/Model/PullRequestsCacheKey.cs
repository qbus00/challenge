using Newtonsoft.Json;

namespace Challenge.Model
{
    public class PullRequestsCacheKey
    {
        public int Page { get; set; }
        public string User { get; set; }
        public string Repo { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }
}