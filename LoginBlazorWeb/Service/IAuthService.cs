using LoginBlazorWeb.Models;

namespace LoginBlazorWeb.Service
{
    public interface IAuthService
    {
        Task<SessionDTO> Login(LoginDTO loginModel);
        Task<SessionDTO> Registrar(RegisterDTO registerModel);

    }
}
