using LoginToken.Models.Custom;

namespace LoginToken.Service
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse>TokenResponse(AuthorizationRequest authorizationRequest);

        Task<AuthorizationResponse> RefreshTokenResponse(RefreshTokenRequest refreshTokenRequest, int idUsuario, string nombre_usuario);
        Task<AuthorizationResponse> RegisterAccount(RegisterRequest registerRequest);
    }
}
