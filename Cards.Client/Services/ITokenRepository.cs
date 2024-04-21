using Cards.Client.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Cards.Client.Services
{
    public interface ITokenRepository
    {
        JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt);
        Dictionary<string, string> DecodeJwt(JwtSecurityToken token);
        void CacheAccessToken(AccessToken accessToken);
        Task<string?> FetchToken();

        void ClearCache();
    }
}
