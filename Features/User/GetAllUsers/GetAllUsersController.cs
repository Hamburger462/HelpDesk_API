using HelpDesk.Data.Dto;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Features.User.GetAllUsers
{
    [ApiController]
    public class GetAllUsersController(IMediator mediator): ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = mediator.Send(new GetAllUsersCommand());
            return Ok(users);
        }
    }
}