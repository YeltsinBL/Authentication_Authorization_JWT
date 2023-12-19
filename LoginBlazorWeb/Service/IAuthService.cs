using LoginBlazorWeb.Models;

namespace LoginBlazorWeb.Service
{
    public interface IAuthService
    {
        Task<SessionDTO> Login(LoginDTO loginModel);
        Task<SessionDTO> Registrar(RegisterDTO registerModel);
        Task<SessionDTO> OlvidaContrasena(string correo);
        Task<SessionDTO> ConfirmarOlvidaContrasena(string correo, string codigo);
        Task<SessionDTO> CambiarContrasena(RegisterDTO registerModel);

    }
}
