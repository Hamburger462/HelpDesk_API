using System.Text.Json.Serialization;
using HelpDesk.Data.Models;

namespace HelpDesk.Data.Dto
{
    // List shape for tickets: counts instead of the full messages and attachments.
    public record TicketSummaryDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketStatus Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketUrgency Urgency { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }

        public Guid CreatedById { get; set; }
        public string? CreatedByFullName { get; set; }

        public List<UserSummaryDto> Assignees { get; set; } = new();
        public int MessageCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}
