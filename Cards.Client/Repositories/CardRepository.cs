using Cards.Client.Models;
using Cards.Client.Services;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Cards.Client.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;

        public CardRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IEnumerable<Card>> GetAllCardsAsync(string appUserId)
        {
            var result = await _httpClient.GetAsync($"{appUserId}/cards/all");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<IEnumerable<Card>>(response, _serializerOptions);
        }

        // public async Task<IEnumerable<Card>> GetCardsForUserAsync(string appUserId)
        /*   public async Task<(IEnumerable<Card>, MetaData)> GetCardsForUserAsync(string appUserId)
           {
               var result = await _httpClient.GetAsync($"{appUserId}/cards/forUser");

               result.EnsureSuccessStatusCode();

               var response = await result.Content.ReadAsStringAsync();

               var cards = JsonSerializer.Deserialize<IEnumerable<Card>>(response, _serializerOptions);

               var paginationHeader = result.Headers.FirstOrDefault(h => h.Key == "X-Pagination");
               MetaData metaData = null;
               if (paginationHeader.Value != null && paginationHeader.Value.Any())
               {
                   var paginationJson = paginationHeader.Value.First();
                   metaData = JsonSerializer.Deserialize<MetaData>(paginationJson);
               }

               return (cards, metaData);
           }*/

        public async Task<(IEnumerable<Card>, MetaData)> GetCardsForUserAsync(string appUserId, CardParameters cardParameters)
        {
            // Construct query string with parameters
            var queryString = string.Join("&", cardParameters.GetType().GetProperties()
                .Where(prop => prop.GetValue(cardParameters, null) != null)
                .Select(prop => $"{prop.Name}={HttpUtility.UrlEncode(prop.GetValue(cardParameters, null).ToString())}"));

            var result = await _httpClient.GetAsync($"{appUserId}/cards/forUser?{queryString}");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();

            var cards = JsonSerializer.Deserialize<IEnumerable<Card>>(response, _serializerOptions);

            var paginationHeader = result.Headers.FirstOrDefault(h => h.Key == "X-Pagination");
            MetaData metaData = null;
            if (paginationHeader.Value != null && paginationHeader.Value.Any())
            {
                var paginationJson = paginationHeader.Value.First();
                metaData = JsonSerializer.Deserialize<MetaData>(paginationJson);
            }

            return (cards, metaData);
        }


        public async Task<Card> GetCardByIdAsync(string appUserId, string cardId)
        {
            var result = await _httpClient.GetAsync($"{appUserId}/cards/{cardId}");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Card>(response, _serializerOptions);
        }
        public async Task<CardForUpdate> GetCardByIdForEditAsync(string appUserId, string cardId)
        {
            var result = await _httpClient.GetAsync($"{appUserId}/cards/{cardId}");

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<CardForUpdate>(response, _serializerOptions);
        }

        public async Task<Card> CreateCard(string appUserId, Card cardForCreation)
        {
            var card = JsonSerializer.Serialize(cardForCreation);

            var requestContent = new StringContent(card, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{appUserId}/cards/", requestContent);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            var createdCard = JsonSerializer.Deserialize<Card>(content, _serializerOptions);

            return createdCard;
        }

        public async Task UpdateCard(string appUserId, CardForUpdate cardForUpdated)
        {
            var card = JsonSerializer.Serialize(cardForUpdated);

            var requestContent = new StringContent(card, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{appUserId}/cards/{cardForUpdated.CardId}", requestContent);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCard(string appUserId, string cardId)
        {
            var response = await _httpClient.DeleteAsync($"{appUserId}/cards/{cardId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
