using Blazored.LocalStorage;
using LoginBlazorWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace LoginBlazorWeb.Extensiones
{
    public class CustomHttpHandler: DelegatingHandler
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IConfiguration _configuration;

        public CustomHttpHandler(
            ILocalStorageService localStorageService,
            IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider,
            IConfiguration configuration
            )
        {
            _localStorageService = localStorageService;
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
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

			// leer token del localStore
			var sesionUsuario = await _localStorageService.ObtenerLocalStorage<string>("sesionUsuario");
			if (sesionUsuario != null)
			{
				request.Headers.Add("Authorization", $"Bearer {sesionUsuario}");
			}
			var originalResponse = await base.SendAsync(request, cancellationToken);
			// Verificar si el Access Token ha expirado para usar el Refresh Token
			if (originalResponse.StatusCode == HttpStatusCode.Unauthorized)
			{
				return await InvokeRefreshTokenCall(originalResponse, request, cancellationToken);
			}
			return originalResponse;

		}
		private async Task<HttpResponseMessage> InvokeRefreshTokenCall(HttpResponseMessage originalResponse,
			HttpRequestMessage originalRequest, CancellationToken cancellationToken)
		{
			// obtener los datos almacenados
			var sesionUsuario = await _localStorageService.ObtenerLocalStorage<string>("sesionUsuario");
			var sesionUsuarioRefresh = await _localStorageService.ObtenerLocalStorage<string>("sesionUsuarioRefresh");
			// formato para el api
			var newTokenRequest = new RefreshTokenDTO()
			{
				RefreshToken = sesionUsuarioRefresh,
                TokenExpirado = sesionUsuario
			};

            var key = Utility.GetRequestUri(_configuration, "registrarHttp");
            var request_uri = Utility.GetRequestUri(_configuration, "getRefreshToken", 2);
            var httpClient = _httpClientFactory.CreateClient(key!);
			var refreshTokenResponse = await httpClient.PostAsJsonAsync<RefreshTokenDTO>(request_uri, newTokenRequest, cancellationToken: cancellationToken);
			if (refreshTokenResponse.StatusCode == HttpStatusCode.OK)
			{
				// obtenemos los tokens y los almacenamos en SessionStorage
				var tokensResponse = await refreshTokenResponse.Content.ReadFromJsonAsync<SessionDTO>();
				var autenticacionExt = (AuthenticationExtension)_authenticationStateProvider;
				await autenticacionExt.ActualizarEstadoAutenticacion(tokensResponse);

				// eliminamos el token expirado del encabezado
				originalRequest.Headers.Remove("Authorization");
                // agregamos el nuevo token al encabezado
                originalRequest.Headers.Add("Authorization", $"Bearer {tokensResponse.Token}");
				// volvemos hacer la petición fallida
				return await base.SendAsync(originalRequest, cancellationToken);
			}
			return originalResponse;
		}
	}
}
