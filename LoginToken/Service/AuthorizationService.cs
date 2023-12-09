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
using System.Data;
using System;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;

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
        private string CreateToken(string idUsuario, string nombre_usuario)
        {
            // Accedemos a la clave secreta para el JWT
            var key = _configuration.GetValue<string>("JwtSetting:secretKey");
            // convetimos la clave en array
            var keyBytes = Encoding.ASCII.GetBytes(key);
            // Agregar la información del usuario al Token
            //var claims = new ClaimsIdentity();
            //claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, idUsuario));
            //claims.AddClaim(new Claim(ClaimTypes.Role, "Administrador"));
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario),
				new Claim(ClaimTypes.Name, nombre_usuario)
			};
            var roles = new[]
            {
                "Administrador", "Supervisor"
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Crear credencial para el Token
            var credencialesToken = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );
            // Descripción del Token
            var tok = new JwtSecurityToken(
                "", // identificar quién emitió el JWT
				"", // identificar el destinatario del JWT
				claims, // información del usuario
                expires: DateTime.UtcNow.AddMinutes(1), // tiempo de expiración del JWT
                signingCredentials: credencialesToken // credenciales del JWT
                );
            //var tokenDescripcion = new SecurityTokenDescriptor
            //{
            //    //Subject = claims,
            //    Claims= (IDictionary<string, object>)claims,
            //    Expires = DateTime.UtcNow.AddMinutes(1),
            //    SigningCredentials = credencialesToken
            //};

            // Crear los controladores del JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenConfig = tokenHandler.CreateToken(tokenDescripcion);

            // obtener el Token
            string tokenCreado = tokenHandler.WriteToken(tok);

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
            if (!VerifyPasswordHash(authorizationRequest.Clave, usuario_registrado.Clave, usuario_registrado.ClaveSalt))
            {
                return await Task.FromResult<AuthorizationResponse>(result: new AuthorizationResponse { Resultado = false, Msg = "Contraseña incorrecta" });
            }
            usuario_registrado = _sesionTokenContext.Usuarios.FirstOrDefault(x =>
                x.NombreUsuario == authorizationRequest.NombreUsuario &&
                x.Clave == usuario_registrado.Clave
            );
            //if (usuario_registrado == null){
            //    //return await Task.FromResult<AuthorizationResponse>(null);
            //    return await Task.FromResult<AuthorizationResponse>(result: new AuthorizationResponse { Resultado = false, Msg = "Contraseña incorrecta" });
            //}

            string tokenCreado = CreateToken(usuario_registrado.IdUsuario.ToString(),
                usuario_registrado.NombreUsuario.ToString());

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

        public async Task<AuthorizationResponse> RefreshTokenResponse(RefreshTokenRequest refreshTokenRequest, int idUsuario,
            string nombre_usuario)
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
            var tokenCreado = CreateToken(idUsuario.ToString(), nombre_usuario.ToString());

            return await SaveHistoryRefreshToken(idUsuario, tokenCreado, refreshTokenCreado);
        }

        #region Verificar Iniciar Sesión
        /// <summary>
        /// Verificar la contraseña encriptada
        /// </summary>
        /// <param name="password"></param>
        /// <param name="claveHash"></param>
        /// <param name="claveSalt"></param>
        private bool VerifyPasswordHash(string password, byte[] claveHash, byte[] claveSalt)
        {
            // indicamos a partir de cual contraseña calculamos el Hash
            using var hmac = new HMACSHA512(claveSalt); 
            var calcularHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return calcularHash.SequenceEqual(claveHash);
        }
        #endregion

        #region Crear Cuenta

        public async Task<AuthorizationResponse> RegisterAccount(RegisterRequest registerRequest)
        {
            if (_sesionTokenContext.Usuarios.Any(u => u.NombreUsuario == registerRequest.Usuario))
            {
                return new AuthorizationResponse { Resultado = false, Msg = "El usuario ya existe" }; ;
            }

            CreatePasswordHash(registerRequest.Password, out byte[] claveHash, out byte[] claveSalt);

            var validEmailToken = CreateRandomToken();
            var user = new Usuario
            {
                NombreUsuario = registerRequest.Usuario,
                Clave = claveHash,
                ClaveSalt = claveSalt,
                VerificarToken = validEmailToken
            };
            _sesionTokenContext.Usuarios.Add(user);
            await _sesionTokenContext.SaveChangesAsync();
                        
            SendEmail(registerRequest.Usuario, validEmailToken);
            return new AuthorizationResponse { Resultado = true, Msg = "Cuenta Creada" };
        }


        /// <summary>
        /// Crear las encriptaciones para la contraseña
        /// </summary>
        /// <param name="password"></param>
        /// <param name="claveHash"></param>
        /// <param name="claveSalt"></param>
        private void CreatePasswordHash(string password, out byte[] claveHash, out byte[] claveSalt)
        {
            using var hmac = new HMACSHA512();
            claveSalt = hmac.Key;// genera una clave aleatoria
            claveHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        /// <summary>
        /// Generar un token de verificación
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
        #endregion

        #region Enviar Correo a la Cuenta Creada
        /// <summary>
        /// Enviar mensaje de verificación al correo
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="validEmailToken"></param>
        private void SendEmail(string usuario, string validEmailToken)
        {
            string url = $"{_configuration["AppUrl"]}/api/User/ConfirmAccount?usuario={usuario}&token={validEmailToken}";
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.Subject = "Confirmar Cuenta";
            email.Body = new TextPart(TextFormat.Html) { Text = $"<p>Práctica de Confirmar Correo {usuario} </p> <p>Por favor, confirma tu correo electrónico haciendo <a href='{url}'>Click aquí</a></p>" };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        #endregion

        #region Verificar Cuenta Creada
        /// <summary>
        /// Verificar la cuenta por el usuario y token generado
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="token_verificar"></param>
        /// <returns>URL para el login de la web</returns>
        public async Task<AuthorizationResponse> VerifyRegisterAccount(string usuario, string token_verificar)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(token_verificar))
            {
                return new AuthorizationResponse { Resultado = false, Msg = "Formato de usuario o token inválidos" };
            }
            var user = _sesionTokenContext.Usuarios.FirstOrDefault(u => 
                        u.NombreUsuario== usuario &&
                        u.VerificarToken == token_verificar);
            if (user == null)
            {
                return new AuthorizationResponse { Resultado = false, Msg = "Usuario o token inválido" };
            }
            user.Verificar = DateTime.Now;
            await _sesionTokenContext.SaveChangesAsync();
            return new AuthorizationResponse { Resultado = true, Msg = $"{_configuration["AppUrlWeb"]}"! };
        }
        #endregion
    }
}
