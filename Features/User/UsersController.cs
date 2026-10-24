using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;
using HelpDesk.Features.User.GetAllUsers;
using HelpDesk.Features.User.GetUserById;
using HelpDesk.Features.User.CreateUser;
using HelpDesk.Features.User.UpdateUser;
using HelpDesk.Features.User.DeleteUser;

namespace HelpDesk.Features.User
{
    /// <summary>
    /// User accounts. Admin only.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Gets a page of users.
        /// </summary>
        /// <remarks>
        /// Sorted by full name. Filters can be combined.
        /// </remarks>
        /// <param name="filter">Paging and filter options, sent as query-string parameters.</param>
        /// <response code="200">A page of users.</response>
        [HttpGet]
        [ProducesResponseType<PagedResult<UserListItemDto>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<UserListItemDto>>> GetAllUsers([FromQuery] UserFilter filter, CancellationToken cancellationToken)
        {
            var users = await mediator.Send(new GetAllUsersCommand(filter), cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// Gets a user by id.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <response code="200">The user.</response>
        [HttpGet("{id}")]
        [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await mediator.Send(new GetUserByIdQuery(id));
            return Ok(user);
        }

        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="userDto">The user to create, including the plain-text password.</param>
        /// <response code="200">The id of the created user.</response>
        /// <response code="400">The request body is invalid.</response>
        [HttpPost]
        [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserDto userDto)
        {
            var userId = await mediator.Send(new CreateUserCommand(userDto));
            return Ok(userId);
        }

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <param name="userDto">The new user values.</param>
        /// <response code="200">The updated user.</response>
        /// <response code="400">The request body is invalid.</response>
        [HttpPost("{id}")]
        [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
        [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto userDto)
        {
            var user = await mediator.Send(new UpdateUserCommand(id, userDto));
            return Ok(user);
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <response code="200">The id of the deleted user.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType<Guid>(StatusCodes.Status200OK)]
        public async Task<ActionResult<Guid>> DeleteUser(Guid id)
        {
            var userId = await mediator.Send(new DeleteUserCommand(id));

            return Ok(userId);
        }
    }
}
