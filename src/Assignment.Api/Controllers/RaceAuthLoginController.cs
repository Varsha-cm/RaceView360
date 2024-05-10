using Assignment.Service.Model;
using Assignment.Service.Services;
using Assignment.Service.Services.RaceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class RaceAuthLoginController : ControllerBase
    {
        private readonly RaceAuthService _raceAuthService;
        public RaceAuthLoginController(RaceAuthService raceAuthService)
        {
            _raceAuthService = raceAuthService;
        }

        [HttpPost("race/auth/login")]
        public async Task<IActionResult> Login([FromBody] AuthRQ authRequest)
        {
            var result = await _raceAuthService.AuthenticateAsync(authRequest);
            if (result <= 0)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = await _raceAuthService.GenerateF1AdminJweToken(authRequest.Email);
            return Ok(new { Token = accessToken });
        }
    }
}
