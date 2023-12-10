using LoginToken.Models.Custom;

namespace LoginToken.Service
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse>TokenResponse(AuthorizationRequest authorizationRequest);

        Task<AuthorizationResponse> RefreshTokenResponse(RefreshTokenRequest refreshTokenRequest, int idUsuario, string nombre_usuario);
        Task<AuthorizationResponse> RegisterAccount(RegisterRequest registerRequest);
        Task<AuthorizationResponse> VerifyRegisterAccount(string usuario, string token_verificar);
        Task<AuthorizationResponse> ForgotAccount(string usuario);
        AuthorizationResponse VerifyForgotAccount(string usuario, string codigo_verificacion);
        Task<AuthorizationResponse> RegisterNewPassword(ResetPasswordRequest resetPasswordRequest);
    }
}
