﻿using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using LoginBlazorWeb.Models;
using System.Security.Claims;

namespace LoginBlazorWeb.Extensiones
{
    public class AuthenticationExtension : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;

        private ClaimsPrincipal _sininformacion = new ClaimsPrincipal(new ClaimsIdentity());

        public AuthenticationExtension(ISessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }

        // actualizar el estado de autenticación cuando el usuario a iniciado sesión
        public async Task ActualizarEstadoAutenticacion(SessionDTO? sessionUsuario)
        {
            ClaimsPrincipal claimsPrincipal;
            if (sessionUsuario != null)
            {
                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, sessionUsuario.Nombre),
                    new Claim(ClaimTypes.Email, sessionUsuario.Correo),
                    new Claim(ClaimTypes.Role, sessionUsuario.Rol)
                },"JwtAuth"));
                // guarda la sesión del ususario
                await _sessionStorageService.GuardarStorage("sesionUsuario", sessionUsuario);
            }
            else
            {
                claimsPrincipal = _sininformacion;
                // para eliminar la sesión del usuario
                await _sessionStorageService.RemoveItemAsync("sesionUsuario");
            }
            // Notificar al servicio de autenticación que se ha cambiado el estado
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));

        }

        // devuelve la información del usuario autenticado
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // obtenemos la sesión del usuario
            var sesionUsuario = await _sessionStorageService.ObtenerStorage<SessionDTO>("sesionUsuario");
            if (sesionUsuario == null)
                return await Task.FromResult(new AuthenticationState(_sininformacion));
            
            // pasamos los datos del usuario logueado a la autenticaicón
            var claimPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, sesionUsuario.Nombre),
                    new Claim(ClaimTypes.Email, sesionUsuario.Correo),
                    new Claim(ClaimTypes.Role, sesionUsuario.Rol)
                }, "JwtAuth"));
            return await Task.FromResult(new AuthenticationState(claimPrincipal));
        
        }
    }
}
