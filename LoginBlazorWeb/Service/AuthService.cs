using LoginBlazorWeb.Extensiones;
using LoginBlazorWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace LoginBlazorWeb.Service
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<SessionDTO> Login(LoginDTO loginModel)
        {
            var clave_encriptada = Utility.GetSHA256(loginModel.clave);
            var httpClient = _httpClientFactory.CreateClient("registerApi");
            //var loginResponse = await httpClient.PostAsJsonAsync("api/User/Authenticate", loginModel);
            var newLoginDto = new LoginDTO() { clave = clave_encriptada, nombreUsuario = loginModel.nombreUsuario };
            var loginResponse = await httpClient.PostAsJsonAsync("api/User/Authenticate", newLoginDto);
            var sesionUsuario = await loginResponse.Content.ReadFromJsonAsync<SessionDTO>();
            if (loginResponse.IsSuccessStatusCode)
            {
                var autenticacionExt = (AuthenticationExtension)_authenticationStateProvider;
                await autenticacionExt.ActualizarEstadoAutenticacion(sesionUsuario);
                return sesionUsuario!;
            }
            return sesionUsuario!;

        }
    }
}
