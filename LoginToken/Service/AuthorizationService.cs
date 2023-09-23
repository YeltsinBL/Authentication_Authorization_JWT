using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginToken.Models;
using LoginToken.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace LoginToken.Service
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly SesionTokenContext _sesionTokenContext;
        private readonly IConfiguration _configuration;

        public AuthorizationService(SesionTokenContext sesionTokenContext, IConfiguration configuration)
        {
            _sesionTokenContext = sesionTokenContext;
            _configuration = configuration;
        }

        // Generar el Token
        private string CreateToken(string idUsuario)
        {
            // Accedemos a la clave secreta para el JWT
            var key = _configuration.GetValue<string>("JwtSetting:secretKey");
            // convetimos la clave en array
            var keyBytes = Encoding.ASCII.GetBytes(key);
            // Agregar la información del usuario al Token
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));
            // Crear credencial para el Token
            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );
            // Descripción del Token
            var tokenDescripcion = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = credencialesToken
            };

            // Crear los controladores del JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescripcion);

            // obtener el Token
            string tokenCreado = tokenHandler.WriteToken(tokenConfig);

            return tokenCreado;

        }

        // Devolver el Token
        public async Task<AuthorizationResponse> TokenResponse(AuthorizationRequest authorizationRequest)
        {
            var usuario_registrado = _sesionTokenContext.Usuarios.FirstOrDefault(x =>
                x.NombreUsuario == authorizationRequest.NombreUsuario
            );
            if (usuario_registrado == null)
            {
                return await Task.FromResult<AuthorizationResponse>(result: new AuthorizationResponse { Resultado = false, Msg = "Usuario incorrecto" });
            }
            usuario_registrado = _sesionTokenContext.Usuarios.FirstOrDefault(x =>
                x.NombreUsuario == authorizationRequest.NombreUsuario &&
                x.Clave == authorizationRequest.Clave
            );
            if (usuario_registrado == null){
                //return await Task.FromResult<AuthorizationResponse>(null);
                return await Task.FromResult<AuthorizationResponse>(result: new AuthorizationResponse { Resultado = false, Msg = "Contraseña incorrecta" });
            }

            string tokenCreado = CreateToken(usuario_registrado.IdUsuario.ToString());

            // devolvemos el Refresh Token
            string refreshToquenCreado = CreateRefreshToken();

            //return new AuthorizationResponse() { Token = tokenCreado, Resultado = true, Msg ="OK" };
            return await SaveHistoryRefreshToken(usuario_registrado.IdUsuario,tokenCreado,refreshToquenCreado); 
        }

        // Generar RefreshToken
        private string CreateRefreshToken()
        {
            var byteArray = new byte[64];
            var refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray);
                refreshToken = Convert.ToBase64String(byteArray);
            }
            return refreshToken;
        }

        // Guardar el Refresh Token en la BD
        private async Task<AuthorizationResponse> SaveHistoryRefreshToken(int idUsuario,
            string token, string refreshToken)
        {
            var historyRefreshToken = new HistorialRefreshToken
            {
                IdUsuario = idUsuario,
                Token = token,
                RefreshToken = refreshToken,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(2)
            };

            await _sesionTokenContext.HistorialRefreshTokens.AddAsync(historyRefreshToken);
            await _sesionTokenContext.SaveChangesAsync();

            return new AuthorizationResponse() { Token = token, RefreshToken = refreshToken,
                Resultado = true, Msg ="OK", Nombre="Prueba", Correo="prueba@prueba.com", Rol="Administrador" };
        
        }

        public async Task<AuthorizationResponse> RefreshTokenResponse(RefreshTokenRequest refreshTokenRequest, int idUsuario)
        {
            var refreshTokenRegistrado = _sesionTokenContext.HistorialRefreshTokens.FirstOrDefault(X =>
                X.Token == refreshTokenRequest.TokenExpirado &&
                X.RefreshToken == refreshTokenRequest.RefreshToken &&
                X.IdUsuario == idUsuario);
            if (refreshTokenRegistrado == null)
            {
                return new AuthorizationResponse{ Resultado = false, Msg = "No existe refreshToken" };
            }
            // Generar ambos Tokens
            var refreshTokenCreado = CreateRefreshToken();
            var tokenCreado = CreateToken(idUsuario.ToString());

            return await SaveHistoryRefreshToken(idUsuario, tokenCreado, refreshTokenCreado);
        }
    }
}
