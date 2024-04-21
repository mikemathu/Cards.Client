using Cards.Client.Models;
using Cards.Client.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Cards.Client.Controllers
{
    [Authorize]
    [Route("Card")]
    public class CardController : Controller
    {
        private readonly ICardRepository _cardRepository;
        private readonly IAuthRepository _loginApiRepository;
        private readonly ITokenRepository _tokenRepository;

        public CardController(ICardRepository cardRepository, IAuthRepository loginApiRepository, ITokenRepository tokenRepository)
        {
            _cardRepository = cardRepository;
            _loginApiRepository = loginApiRepository;
            _tokenRepository = tokenRepository;
        }

        // GET: /
        [Route("/")]
        public async Task<IActionResult> GetCards()
        {
            Dictionary<string, string>? decodedJwt = await RetrieveAndDecodeCachedJwt();

            if (decodedJwt == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (decodedJwt.TryGetValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", out string? role))
            {
                if (role == "Admin")
                {
                    return RedirectToAction(nameof(GetAllCards));
                }
                else if (role == "Member")
                {
                    return RedirectToAction(nameof(GetCardsForUser));
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        // GET: Card/all
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCards()
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();
            if (decodedJwt == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            IEnumerable<Card> cards = await _cardRepository.GetAllCardsAsync(decodedJwt["AppUserId"]);

            return View("Dashboard", cards);

        }

        // GET: Card/forUser
        /*   [Authorize(Roles = "Member")]
           [HttpGet("forUser")]
           public async Task<IActionResult> GetCardsForUser()
           {
               Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();
               if (decodedJwt == null)
               {
                   return RedirectToAction("Login", "Auth");
               }

               (IEnumerable<Card> cards, MetaData metaData) = await _cardRepository.GetCardsForUserAsync(decodedJwt["AppUserId"]);

               var viewModel = new CardsViewModel
               {
                   Cards = cards,
                   MetaData = metaData
               };

               return View("Dashboard", viewModel);

           }*/

        // GET: Card/forUser
        [Authorize(Roles = "Member")]
        [HttpGet("forUser")]
        public async Task<IActionResult> GetCardsForUser([FromQuery] CardParameters cardParameters, 
            string sortByString,string pageSize)
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();
            if (decodedJwt == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            (IEnumerable<Card> cards, MetaData metaData) = await _cardRepository.GetCardsForUserAsync(decodedJwt["AppUserId"], cardParameters);

            var viewModel = new CardsViewModel
            {
                Cards = cards,
                MetaData = metaData
            };

            return View("Dashboard", viewModel);
        }


        // GET: Card/d10caae9-c6c9-47d4-9b19-bec30b4a94f5
        [HttpGet("{cardId}")]
        public async Task<IActionResult> GetCardById(string cardId)
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();

            var cards = await _cardRepository.GetCardByIdAsync(decodedJwt["AppUserId"], cardId);

            return View("CardDetails", cards);
        }

        // GET: Card/CreateCard
        [HttpGet("CreateCard")]
        public IActionResult CreateCard()
        {
            return View();
        }

        // POST: Card/CreateCard
        [HttpPost("CreateCard")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(Card card)
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();

            Card createdCard = await _cardRepository.CreateCard(decodedJwt["AppUserId"], card);

            return RedirectToAction(nameof(GetCardById), new { cardId = createdCard.CardId });
        }

        // GET: Card/CardEdit/d10caae9-c6c9-47d4-9b19-bec30b4a94f5
        [HttpGet("CardEdit")]
        public async Task<ActionResult> UpdateCard(string cardId)
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();

            var cards = await _cardRepository.GetCardByIdForEditAsync(decodedJwt["AppUserId"], cardId);

            return View("EditCard", cards);
        }

        // POST: Card/CardEdit/d10caae9-c6c9-47d4-9b19-bec30b4a94f5
        [HttpPost("CardEdit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateCard(CardForUpdate card)
        {
            
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();

            await _cardRepository.UpdateCard(decodedJwt["AppUserId"], card);

            return RedirectToAction(nameof(GetCardById), new { cardId = card.CardId });
        }

        // POST: Card/DeleteCard/d10caae9-c6c9-47d4-9b19-bec30b4a94f5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteCard(string cardId)
        {
            Dictionary<string, string> decodedJwt = await RetrieveAndDecodeCachedJwt();

            await _cardRepository.DeleteCard(decodedJwt["AppUserId"], cardId);

            return RedirectToAction(nameof(GetCards));
        }
        private async Task<Dictionary<string, string>?> RetrieveAndDecodeCachedJwt()
        {
            string? cashedToken = await _tokenRepository.FetchToken();//Get Chached Token

            if (cashedToken != null)
            {
                JwtSecurityToken jwtSecurityToken = _tokenRepository.ConvertJwtStringToJwtSecurityToken(cashedToken);

                Dictionary<string, string> decodedJwt = _tokenRepository.DecodeJwt(jwtSecurityToken);

                return decodedJwt;
            }
            return null;
        }
    }
}
