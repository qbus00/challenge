using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Challenge.Model;
using Refit;
using Refit.Insane.PowerPack.Caching;

namespace Challenge.Rest
{
    public interface IGitHubApi
    {
        [RefitCache(Constants.RefitCacheInSeconds)]
        [Headers("User-Agent: Awesome App")]
        [Get("/search/repositories?q={searchPhrase}+in:name+language:JavaScript&sort=stars&page={page}&per_page={perPage}")]
        Task<ItemsCollection<Repository>> GetRepositories(
            [RefitCachePrimaryKey] string cacheKey,
            int page,
            int perPage,
            string searchPhrase,
            CancellationToken cancellationToken = default(CancellationToken));

        [RefitCache(Constants.RefitCacheInSeconds)]
        [Headers("User-Agent: Awesome App")]
        [Get("/repos/{user}/{repo}/pulls?sort=created&page={page}&state=all&direction=desc&per_page={perPage}")]
        Task<IEnumerable<PullRequest>> GetPullRequests(
            [RefitCachePrimaryKey] string cacheKey,
            int page,
            int perPage,
            string user,
            string repo,
            CancellationToken cancellationToken = default(CancellationToken));

    }
}
