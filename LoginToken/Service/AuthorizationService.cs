using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LoginToken.Models;
using LoginToken.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
                x.NombreUsuario == authorizationRequest.NombreUsuario &&
                x.Clave == authorizationRequest.Clave
            );
            if (usuario_registrado == null){
                return await Task.FromResult<AuthorizationResponse>(null);
            }

            string tokenCreado = CreateToken(usuario_registrado.IdUsuario.ToString());

            return new AuthorizationResponse() { Token = tokenCreado, Resultado = true, Msg ="OK" };
        }
    }
}
