using System.Threading;
using System.Threading.Tasks;
using Challenge.Model;
using Refit;

namespace Challenge.Rest
{
    public interface IGitHubApi
    {
        [Headers("User-Agent: Awesome App")]
        [Get("/search/repositories?q=language:JavaScript&sort=stars&page={page}&per_page=20")]
        Task<ItemsCollection<Repository>> GetRepositories(int page, CancellationToken cancellationToken = default(CancellationToken));
    }
}
