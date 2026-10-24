using HelpDesk.Data.Models;

namespace HelpDesk.Data.Dto
{
    public record UserFilter : PagedRequest
    {
        /// <summary>Case-insensitive match on full name or email.</summary>
        public string? Search { get; set; }

        /// <summary>Exact role: User, Operator or Admin.</summary>
        public string? Role { get; set; }
    }

    public record TicketFilter : PagedRequest
    {
        public TicketStatus? Status { get; set; }
        public TicketUrgency? Urgency { get; set; }

        /// <summary>Tickets sent by this user.</summary>
        public Guid? CreatedById { get; set; }

        /// <summary>Tickets this operator is assigned to.</summary>
        public Guid? AssigneeId { get; set; }

        /// <summary>True returns only tickets nobody is assigned to (the queue). Cannot be combined with AssigneeId.</summary>
        public bool? Unassigned { get; set; }

        /// <summary>Case-insensitive match on the title.</summary>
        public string? Search { get; set; }

        public DateTimeOffset? CreatedFrom { get; set; }
        public DateTimeOffset? CreatedTo { get; set; }
    }

    public record MessageFilter : PagedRequest
    {
        /// <summary>Case-insensitive match on the message text.</summary>
        public string? Search { get; set; }

        public Guid? AuthorId { get; set; }
        public DateTimeOffset? CreatedFrom { get; set; }
        public DateTimeOffset? CreatedTo { get; set; }
    }

    public record AttachmentFilter : PagedRequest
    {
        /// <summary>Case-insensitive match on the file name.</summary>
        public string? Search { get; set; }

        public Guid? UploadedById { get; set; }

        /// <summary>Content type or prefix, e.g. "application/pdf" or "image/".</summary>
        public string? ContentType { get; set; }

        public DateTimeOffset? CreatedFrom { get; set; }
        public DateTimeOffset? CreatedTo { get; set; }
    }
}
