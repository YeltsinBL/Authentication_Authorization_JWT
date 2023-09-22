using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LoginToken.Models.Custom;
using LoginToken.Service;
using System.IdentityModel.Tokens.Jwt;

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
            if (result == null)
            {
                return Unauthorized();
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
            string idUsuario = tokenSupuestamenteExperido.Claims.First(x =>
                x.Type == JwtRegisteredClaimNames.NameId).Value.ToString();
            // Generar la Respuesta del Token
            var autorizacionResponse = await _authorizationService.RefreshTokenResponse(refreshTokenRequest, int.Parse(idUsuario));
            if (autorizacionResponse.Resultado)
            {
                return Ok(autorizacionResponse);
            } else
                return BadRequest(autorizacionResponse);
        }
    }
}
