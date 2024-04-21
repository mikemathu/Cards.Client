using Cards.Client.Models;
using Cards.Client.Services;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace Cards.Client.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuthRepository _authRepository;
        private string tokenCacheKey = "tokenCacheKey";

        public TokenRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void CacheAccessToken(AccessToken accessToken)
        {
            _memoryCache.Set(tokenCacheKey, accessToken.Token);
        }            

        public JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }
        public Dictionary<string, string> DecodeJwt(JwtSecurityToken token)
        {
            Dictionary<string, string> claims = token.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);

            return claims;
        }

        public async Task<string?> FetchToken()
        {
            var cachedtoken = _memoryCache.Get(tokenCacheKey);

            if (!_memoryCache.TryGetValue(tokenCacheKey, out cachedtoken))
            {
                //cachedtoken = await GetTokenFromApi();

                var options = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(10.00));

                _memoryCache.Set(tokenCacheKey, tokenCacheKey, options);
            }
            if (cachedtoken != null)
            {
                return cachedtoken.ToString();
            }
            return null;

        }

        private async Task<string> GetTokenFromApi()//TODO: User refresh token instead
        {

            var appUser = new AppUser { Email = "kev@gmail.com", Password = "kevP@ssword1" };
            AccessToken accessToken = await _authRepository.AuthenticateAsync(appUser);
            return accessToken.Token;
        }
        public void ClearCache()
        {
            _memoryCache.Remove("TOKEN");
        }

    }
}
