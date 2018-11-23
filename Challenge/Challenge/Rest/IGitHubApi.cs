using System;
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
        [Get("/search/repositories?q={searchPhrase}+in:name+language:JavaScript&sort=stars&page={page}&per_page=20&cacheKey={cacheKey}")]
        Task<ItemsCollection<Repository>> GetRepositories(
            [RefitCachePrimaryKey]string cacheKey,
            int page, 
            string searchPhrase, 
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
