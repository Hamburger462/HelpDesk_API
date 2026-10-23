using HelpDesk.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(u => u.Id);
                e.Property(u => u.FullName).HasMaxLength(200).IsRequired();
                e.Property(u => u.Email).HasMaxLength(256).IsRequired();
                e.Property(u => u.PassHash).IsRequired();
                e.Property(u => u.Role).HasMaxLength(50).IsRequired();
                e.HasIndex(u => u.Email).IsUnique();

                // User 1 -> * Tickets
                e.HasMany(u => u.Tickets)
                 .WithOne(t => t.User)
                 .HasForeignKey(t => t.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ticket>(e =>
            {
                e.HasKey(t => t.Id);

                // Ticket 1 -> * Messages
                e.HasMany(t => t.Messages)
                 .WithOne(m => m.Ticket)
                 .HasForeignKey(m => m.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);

                // Ticket 1 -> * Attachments
                e.HasMany(t => t.Attachments)
                 .WithOne(a => a.Ticket)
                 .HasForeignKey(a => a.TicketId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(e =>
            {
                e.HasKey(m => m.Id);
                e.Property(m => m.Text).IsRequired();
                // non-unique index on TicketId is created by convention
            });

            modelBuilder.Entity<Attachment>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.FileName).HasMaxLength(255).IsRequired();
                e.Property(a => a.ContentType).HasMaxLength(127).IsRequired();
                e.Property(a => a.StoragePath).HasMaxLength(500).IsRequired();
            });
        }
    }
}