using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using LoginBlazorWeb.Models;
using System.Net;
using System.Net.Http.Json;

namespace LoginBlazorWeb.Extensiones
{
    public class CustomHttpHandler: DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public CustomHttpHandler(
            ILocalStorageService localStorageService,
            IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider
            )
        {
            _localStorageService = localStorageService;
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }
        // Ejecutar este método antes de que se envíe la solicitud al servidor
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Indicamos que estas apis no necesitan una autorización
            if (request.RequestUri.AbsolutePath.ToLower().Contains("Authenticate") ||
                request.RequestUri.AbsolutePath.ToLower().Contains("GetRefreshToken"))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var originalResponse = await base.SendAsync(request, cancellationToken);
            
            return originalResponse;

        }
    }
}
