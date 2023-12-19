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
        private readonly IConfiguration _configuration;

        public AuthService(IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _authenticationStateProvider = authenticationStateProvider;
            _configuration = configuration;
        }

        public async Task<SessionDTO> Login(LoginDTO loginModel)
        {
            var clave_encriptada = Utility.GetSHA256(loginModel.clave);//loginModel.clave;//
            var key = Utility.GetRequestUri(_configuration, "registrarHttp");
            var requestUri = Utility.GetRequestUri(_configuration, "authentication",2);
            var httpClient = _httpClientFactory.CreateClient(key!);
            var newLoginDto = new LoginDTO() { clave = clave_encriptada, nombreUsuario = loginModel.nombreUsuario };
            var loginResponse = await httpClient.PostAsJsonAsync(requestUri!, newLoginDto);
            var sesionUsuario = await loginResponse.Content.ReadFromJsonAsync<SessionDTO>();
            if (loginResponse.IsSuccessStatusCode)
            {
                var autenticacionExt = (AuthenticationExtension)_authenticationStateProvider;
                await autenticacionExt.ActualizarEstadoAutenticacion(sesionUsuario);
                return sesionUsuario!;
            }
            return sesionUsuario!;

        }

        public async Task<SessionDTO> Registrar(RegisterDTO registerModel)
        {
            var clave_encriptada = Utility.GetSHA256(registerModel.password);//loginModel.clave;//
            var key = Utility.GetRequestUri(_configuration, "registrarHttp");
            var requestUri = Utility.GetRequestUri(_configuration, "getRegisterUser", 2);
            var httpClient = _httpClientFactory.CreateClient(key!);
            var newRegisterDTO = new RegisterDTO() { password = clave_encriptada, usuario = registerModel.usuario,confirmPassword=clave_encriptada };
            var loginResponse = await httpClient.PostAsJsonAsync(requestUri!, newRegisterDTO);
            var sesionUsuario = await loginResponse.Content.ReadFromJsonAsync<SessionDTO>();            
            return sesionUsuario!;
        }
    }
}
