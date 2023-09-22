using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LoginToken.Models.Custom;
using LoginToken.Service;

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
        public async Task<IActionResult> Authenticate([FromBody]AuthorizationRequest authorization)
        {
            var result = await _authorizationService.TokenResponse(authorization);
            if (result == null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
