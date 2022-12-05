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
        private readonly ILogger<TokenCacheService> _logger;

        public TokenCacheService(IDistributedCache cache, ILogger<TokenCacheService> logger)
        {
            _options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddHours(5))
                .SetSlidingExpiration(TimeSpan.FromHours(3));
            _cache = cache;
            _logger = logger;
        }

        public async Task AddToken(string token)
        {
            try
            {
                await _cache.SetAsync(token, Encoding.UTF8.GetBytes("Expired"), _options);
            }
            catch
            {
                _logger.LogError("Token database not connected");
            }
           
        }

        public async Task<bool> IsTokenInDB(string token)
        {
            try
            {
                var tokenCache = await _cache.GetAsync(token);
                if (tokenCache == null) { return false; }
                return true;
            }            
            catch
            {
                _logger.LogError("Token database not connected");
               throw new Exception();
            }
        }
    }
}
