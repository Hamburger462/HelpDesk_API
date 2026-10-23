using MediatR;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Features.User.GetAllUsers;
using HelpDesk.Features.User.GetUserById;
using HelpDesk.Features.User.CreateUser;
using HelpDesk.Features.User.UpdateUser;
using HelpDesk.Features.User.DeleteUser;

namespace HelpDesk.Features.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await mediator.Send(new GetAllUsersCommand());
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await mediator.Send(new GetUserByIdQuery(id));
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserDto userDto)
        {
            var userId = await mediator.Send(new CreateUserCommand(userDto));
            return Ok(userId);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
        {
            var user = await mediator.Send(new UpdateUserCommand(id, userDto));
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            var userId = await mediator.Send(new DeleteUserCommand(id));

            return Ok(userId);
        }
    }
}
