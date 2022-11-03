using Microsoft.Extensions.Caching.Distributed;
using System.Text;
namespace MovieCatalogAPI.Services
{
    public interface ITokenCacheService
    {
        Task<bool> IsTokenInDB(string token);
        Task AddToken(string token);
    }
    public class TokenCacheService: ITokenCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;

        public TokenCacheService(IDistributedCache cache)
        {
            _options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddHours(5))
                .SetSlidingExpiration(TimeSpan.FromHours(3));
            _cache = cache;
        }

        public async Task AddToken(string token)
        {
            await _cache.SetAsync(token,Encoding.UTF8.GetBytes("Expired"),_options);
        }

        public async Task<bool> IsTokenInDB(string token)
        {
            var tokenCache = await _cache.GetAsync(token);
            if (tokenCache == null) { return false; }
            return true;
        }
    }
}
