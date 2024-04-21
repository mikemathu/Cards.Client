using Cards.Client.Models;
using Cards.Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Cards.Client.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(IAuthRepository loginApiRepository, ITokenRepository tokenRepository)
        {
            _authRepository = loginApiRepository;
            _tokenRepository = tokenRepository;
        }

        [Route("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        //[Route("Login")]
        public async Task<IActionResult> Login(AppUser appUser)
        {
            AccessToken accessToken = await _authRepository.AuthenticateAsync(appUser);

            JwtSecurityToken jwtSecurityToken = _tokenRepository.ConvertJwtStringToJwtSecurityToken(accessToken.Token);

            Dictionary<string, string> decodedJwt = _tokenRepository.DecodeJwt(jwtSecurityToken);

            ClaimsIdentity claimsIdentity = _authRepository.CreateClaims(decodedJwt);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("GetCards", "Card");
            //return RedirectToAction("GetCards", "Card", new { DecodedJwt = decodedJwt });

        }

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _tokenRepository.ClearCache();
            return LocalRedirect("/Auth/Login");
        }
    }
}
