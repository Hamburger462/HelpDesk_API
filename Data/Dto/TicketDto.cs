using System.Text.Json.Serialization;
using HelpDesk.Data.Models;

namespace HelpDesk.Data.Dto
{
    public record TicketDto
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketStatus Status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TicketUrgency Urgency { get; set; } = TicketUrgency.Normal;

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }

        public Guid CreatedById { get; set; }
        public List<UserSummaryDto> Assignees { get; set; } = new();
        public List<MessageDto> Messages { get; set; } = new();
        public List<AttachmentDto> Attachments { get; set; } = new();
    }
}
