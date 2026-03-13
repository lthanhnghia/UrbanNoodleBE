using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrbanNoodle.Dto;
using UrbanNoodle.Service.Interface;

namespace UrbanNoodle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<ActionResult<ResponseLoginDTO>> Login(LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result;
        }

        [Authorize(Roles = "staff")]
        [HttpGet("staff-only")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("Phân quyền của staff");
        }
    }
}
