using Cards.Client.Repositories;
using Cards.Client.Services;
using System.Net.Http.Headers;

namespace Cards.Client.Handler
{
    public class LoginHandler : DelegatingHandler
    {
        private readonly ITokenRepository _tokenRepository;

        public LoginHandler(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenRepository.FetchToken();

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
