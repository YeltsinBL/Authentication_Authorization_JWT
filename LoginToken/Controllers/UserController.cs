using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LoginToken.Models.Custom;
using LoginToken.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LoginToken.Models;

namespace LoginToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public UserController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthorizationRequest authorization)
        {
            var result = await _authorizationService.TokenResponse(authorization);
            if (result.Resultado == false)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenSupuestamenteExperido = tokenHandler.ReadJwtToken(refreshTokenRequest.TokenExpirado);

            if(tokenSupuestamenteExperido.ValidTo >DateTime.UtcNow) {
                return BadRequest(new AuthorizationResponse { Resultado = false, Msg="Token no ha expirado"});
            }
            // Obtenemos el IdUsuario que está dentro del JWT
            //string idUsuario = tokenSupuestamenteExperido.Claims.First(x =>
            //    x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();
            string idUsuario = tokenSupuestamenteExperido.Claims.FirstOrDefault(x =>
                x.Type == ClaimTypes.NameIdentifier).Value.ToString();
			string nombre_Usuario = tokenSupuestamenteExperido.Claims.FirstOrDefault(x =>
				x.Type == ClaimTypes.Name).Value.ToString();
			// Generar la Respuesta del Token
			var autorizacionResponse = await _authorizationService.RefreshTokenResponse(refreshTokenRequest, int.Parse(idUsuario), nombre_Usuario);
            if (autorizacionResponse.Resultado)
            {
                return Ok(autorizacionResponse);
            } else
                return BadRequest(autorizacionResponse);
        }

        [HttpPost]
        [Route("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(RegisterRequest registerRequest)
        {
            var result = await _authorizationService.RegisterAccount(registerRequest);
            if (!result.Resultado)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("ConfirmAccount")]
        public async Task<IActionResult> ConfirmAccount(string usuario, string token)
        {
            var result = await _authorizationService.VerifyRegisterAccount(usuario, token);
            if (!result.Resultado)
            {
                return BadRequest(result);
            }
            return Redirect(result.Msg);
        }
        [HttpPost]
        [Route("ForgotAccount")]
        public async Task<IActionResult> ForgotAccount(string usuario)
        {
            var result = await _authorizationService.ForgotAccount(usuario);
            if (!result.Resultado)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpGet]
        [Route("ConfirmForgotAccount")]
        public IActionResult ConfirmForgotAccount(string usuario, string codigo_verificacion)
        {
            var result = _authorizationService.VerifyForgotAccount(usuario, codigo_verificacion);
            if (!result.Resultado)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost]
        [Route("RegisterNewPassword")]
        public async Task<IActionResult> RegisterNewPassword(ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authorizationService.RegisterNewPassword(resetPasswordRequest);
            if (!result.Resultado)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
