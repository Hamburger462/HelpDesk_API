using MediatR;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Features.Auth.Register;
using HelpDesk.Features.Auth.Login;

namespace HelpDesk.Features.Auth
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(IMediator mediator): ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] RegisterDto registerDto)
        {
            var token = await mediator.Send(new RegisterCommand(registerDto));

            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto loginDto)
        {
            var token = await mediator.Send(new LoginCommand(loginDto));

            return Ok(token);
        }
    }
}
