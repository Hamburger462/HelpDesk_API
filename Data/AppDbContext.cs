using HelpDesk.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}

        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<User> Users => Set<User>();
    }
}