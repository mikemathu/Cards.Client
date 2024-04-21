using Cards.Client.Models;
using System.Security.Claims;

namespace Cards.Client.Services
{
    public interface IAuthRepository
    {
       Task<AccessToken> AuthenticateAsync(AppUser appUser);
       ClaimsIdentity CreateClaims(Dictionary<string, string> decodedJwt);    
    }
}
