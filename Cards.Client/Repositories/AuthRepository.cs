using Cards.Client.Models;
using Cards.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Cards.Client.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenRepository _tokenRepository;
        private readonly JsonSerializerOptions _serializerOptions;
        public AuthRepository(HttpClient httpClient, ITokenRepository tokenRepository)
        {
            _httpClient = httpClient;
            _tokenRepository = tokenRepository;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<AccessToken> AuthenticateAsync(AppUser appUser)
        {

            var body = JsonSerializer.Serialize(new { appUser.Email, appUser.Password }, _serializerOptions);
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var result = await _httpClient.PostAsync("login", content);

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();

            var deserializeToken = JsonSerializer.Deserialize<AccessToken>(response, _serializerOptions);

            _tokenRepository.CacheAccessToken(deserializeToken);

            return deserializeToken;
        }

        public ClaimsIdentity CreateClaims(Dictionary<string, string> decodedJwt)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, decodedJwt["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]),//
                        new Claim("AppUserId", decodedJwt["AppUserId"]),
                        new Claim(ClaimTypes.Role, decodedJwt["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"])
                    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return claimsIdentity;
        }     
    }
}
