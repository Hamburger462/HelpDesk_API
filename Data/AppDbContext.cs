using HelpDesk.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HelpDesk.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Attachment> Attachments => Set<Attachment>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketAssignee> TicketAssignees => Set<TicketAssignee>();
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

                e.ToTable(t => t.HasCheckConstraint("CK_Users_Role", $"\"Role\" IN ({InList(Roles.All)})"));
            });

            modelBuilder.Entity<Ticket>(e =>
            {
                e.HasKey(t => t.Id);
                e.Property(t => t.Title).HasMaxLength(200).IsRequired();
                e.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
                e.Property(t => t.Urgency).HasConversion<string>().HasMaxLength(20);
                e.HasIndex(t => t.Status);

                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Tickets_Status", $"\"Status\" IN ({InList(Enum.GetNames<TicketStatus>())})");
                    t.HasCheckConstraint("CK_Tickets_Urgency", $"\"Urgency\" IN ({InList(Enum.GetNames<TicketUrgency>())})");
                });

                // The sender: User 1 -> * Tickets. Restrict so deleting a user can't wipe shared conversations.
                e.HasOne(t => t.CreatedBy)
                 .WithMany(u => u.CreatedTickets)
                 .HasForeignKey(t => t.CreatedById)
                 .OnDelete(DeleteBehavior.Restrict);

                // The responders: Tickets * <-> * Users through TicketAssignees
                e.HasMany(t => t.Assignees)
                 .WithMany(u => u.AssignedTickets)
                 .UsingEntity<TicketAssignee>(
                    j => j.HasOne(a => a.User)
                          .WithMany()
                          .HasForeignKey(a => a.UserId)
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(a => a.Ticket)
                          .WithMany()
                          .HasForeignKey(a => a.TicketId)
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasKey(a => new { a.TicketId, a.UserId }));

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

                e.HasOne(m => m.Author)
                 .WithMany()
                 .HasForeignKey(m => m.AuthorId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Attachment>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.FileName).HasMaxLength(255).IsRequired();
                e.Property(a => a.ContentType).HasMaxLength(127).IsRequired();
                e.Property(a => a.StoragePath).HasMaxLength(500).IsRequired();

                e.HasOne(a => a.UploadedBy)
                 .WithMany()
                 .HasForeignKey(a => a.UploadedById)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }

        // Keep Ticket.UpdatedAt current whenever a ticket row is modified.
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            TouchTickets();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            TouchTickets();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void TouchTickets()
        {
            foreach (var entry in ChangeTracker.Entries<Ticket>().Where(e => e.State == EntityState.Modified))
                entry.Entity.UpdatedAt = DateTimeOffset.UtcNow;
        }

        private static string InList(IEnumerable<string> values) =>
            string.Join(", ", values.Select(v => $"'{v}'"));
    }
}
