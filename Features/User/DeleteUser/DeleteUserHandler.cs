using HelpDesk.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Features.User.DeleteUser
{
    public class DeleteUserHandler(AppDbContext context) : IRequestHandler<DeleteUserCommand, Guid?>
    {
        public async Task<Guid?> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userId = request.id;
            var user = await context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null) {
                return null;
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return userId;
        }
    }
}
