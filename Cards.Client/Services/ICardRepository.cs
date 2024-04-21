using Cards.Client.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Cards.Client.Services
{
    public interface ICardRepository
    {
        Task<IEnumerable<Card>> GetAllCardsAsync(string appUserId);
        //Task<IEnumerable<Card>> GetCardsForUserAsync(string appUserId);
        Task<(IEnumerable<Card>, MetaData)> GetCardsForUserAsync(string appUserId, CardParameters cardParameters);
        public abstract Task<Card> GetCardByIdAsync(string appUserId, string cardId);
        public abstract Task<CardForUpdate> GetCardByIdForEditAsync(string appUserId, string cardId);
        Task<Card> CreateCard(string appUserId, Card card);
        Task UpdateCard(string appUserId, CardForUpdate card);
        Task DeleteCard(string appUserId, string cardId);

    }
}
