using LoginBlazorWeb.Models;

namespace LoginBlazorWeb.Service
{
    public interface IAuthService
    {
        Task<SessionDTO> Login(LoginDTO loginModel);

    }
}
