namespace Challenge
{
    public class Constants
    {
        public const int RefitPerPage = 50;
        public const int RefitPreloadPerPage = RefitPerPage - 10 < 0 ? 1 : RefitPerPage - 10;
        public const int RefitCacheInSeconds = 60 * 15;
        public const int SearchThrottlingInMiliseconds = 300;
    }
}
