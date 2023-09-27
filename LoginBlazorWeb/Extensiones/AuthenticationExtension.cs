using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using LoginBlazorWeb.Models;
using System.Security.Claims;
using Blazored.LocalStorage;
using System.Text.Json;

namespace LoginBlazorWeb.Extensiones
{
    public class AuthenticationExtension : AuthenticationStateProvider
    {
        //private readonly ISessionStorageService _sessionStorageService;
        private readonly ILocalStorageService _localStorageService;

        public AuthenticationExtension(//ISessionStorageService sessionStorageService,
            ILocalStorageService localStorageService
            )
        {
            //_sessionStorageService = sessionStorageService;
            _localStorageService = localStorageService;
        }

        private ClaimsPrincipal _sininformacion = new ClaimsPrincipal(new ClaimsIdentity());


        // actualizar el estado de autenticación cuando el usuario a iniciado sesión
        public async Task ActualizarEstadoAutenticacion(SessionDTO? sessionUsuario)
        {
            ClaimsPrincipal claimsPrincipal;
            if (sessionUsuario != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, sessionUsuario.Nombre),
                    //new Claim(ClaimTypes.Email, sessionUsuario.Correo),
                    new Claim(ClaimTypes.Role, sessionUsuario.Rol),
                    new Claim("Token", sessionUsuario.Token),
                    new Claim("RefreshToken", sessionUsuario.RefreshToken)
                },"JwtAuth"));
                // guarda la sesión del ususario
                //await _sessionStorageService.GuardarStorage("sesionUsuario", sessionUsuario);
                await _localStorageService.GuardarLocalStorage("sesionUsuario", sessionUsuario);
            }
            else
            {
                claimsPrincipal = _sininformacion;
                // para eliminar la sesión del usuario
                //await _sessionStorageService.RemoveItemAsync("sesionUsuario");
                await _localStorageService.RemoveItemAsync("sesionUsuario");
            }
            // Notificar al servicio de autenticación que se ha cambiado el estado
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

        }

        // devuelve la información del usuario autenticado
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // obtenemos la sesión del usuario
            //var sesionUsuario = await _sessionStorageService.ObtenerStorage<SessionDTO>("sesionUsuario");
            var sesionUsuario = await _localStorageService.ObtenerLocalStorage<SessionDTO>("sesionUsuario");
            if (sesionUsuario == null)
                return await Task.FromResult(new AuthenticationState(_sininformacion));

            // pasamos los datos del usuario logueado a la autenticaicón
            //var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            //    {
            //        //new Claim(ClaimTypes.Name, sesionUsuario.Nombre),
            //        //new Claim(ClaimTypes.Email, sesionUsuario.Correo),
            //        new Claim(ClaimTypes.Role, sesionUsuario.Rol),
            //        new Claim("Token", sesionUsuario.Token),
            //        new Claim("RefreshToken", sesionUsuario.RefreshToken)
            //    }, "JwtAuth"));
            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
                ParseClaimsFromJwt(sesionUsuario.Token), "JwtAuth"));
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        
        }

        // Leer el Token
        private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        //public void NotifyAuthState()
        //{
        //    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        //}
    }
}
