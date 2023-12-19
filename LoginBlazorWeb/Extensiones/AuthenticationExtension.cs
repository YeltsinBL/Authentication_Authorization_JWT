using Blazored.LocalStorage;
using LoginBlazorWeb.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace LoginBlazorWeb.Extensiones
{
    public class AuthenticationExtension : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private ClaimsPrincipal _sininformacion = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthenticationExtension(
            ILocalStorageService localStorageService
            )
        {
            //_sessionStorageService = sessionStorageService;
            _localStorageService = localStorageService;
        }

        // actualizar el estado de autenticación cuando el usuario a iniciado sesión
        public async Task ActualizarEstadoAutenticacion(SessionDTO? sessionUsuario)
        {
            ClaimsPrincipal claimsPrincipal;
            if (sessionUsuario != null)
            {
                //claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                //{
                //    new Claim(ClaimTypes.Name, sessionUsuario.Nombre),
                //    //new Claim(ClaimTypes.Email, sessionUsuario.Correo),
                //    new Claim(ClaimTypes.Role, sessionUsuario.Rol),
                //    new Claim("Token", sessionUsuario.Token),
                //    new Claim("RefreshToken", sessionUsuario.RefreshToken)
                //},"JwtAuth"));

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(Utility.ParseClaimsFromJwt(sessionUsuario.Token), "JwtAuth"));

                // guarda la sesión del ususario
                await _localStorageService.GuardarLocalStorage("sesionUsuario", sessionUsuario.Token);
                await _localStorageService.GuardarLocalStorage("sesionUsuarioRefresh", sessionUsuario.RefreshToken);
            }
            else
            {
                claimsPrincipal = _sininformacion;
                // para eliminar la sesión del usuario
                await _localStorageService.RemoveItemAsync("sesionUsuario");
                await _localStorageService.RemoveItemAsync("sesionUsuarioRefresh");
            }
            // Notificar al servicio de autenticación que se ha cambiado el estado
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

        }

        // devuelve la información del usuario autenticado
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // obtenemos la sesión del usuario
            //var sesionUsuario = await _localStorageService.ObtenerLocalStorage<SessionDTO>("sesionUsuario");
            var sesionUsuario = await _localStorageService.ObtenerLocalStorage<string>("sesionUsuario");
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
            //var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(
            //    ParseClaimsFromJwt(sesionUsuario.Token), "JwtAuth"));
            //return await Task.FromResult(new AuthenticationState(claimPrincipal));
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(Utility.ParseClaimsFromJwt(sesionUsuario), "JwtAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
            return await Task.FromResult(new AuthenticationState(authenticatedUser));
        }

        public async Task RestaurarContrasena(SessionDTO? sessionUsuario)
        {
            if (sessionUsuario != null) await _localStorageService.GuardarLocalStorage("changepassword", sessionUsuario.Correo);
            else await _localStorageService.RemoveItemAsync("changepassword");
        }
    }
}
