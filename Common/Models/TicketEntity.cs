using TicketingSystem.Models.Enums;

namespace TicketingSystem.Common.Models
{
    public class TicketEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid? Assignee { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime ReportedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public TicketTypeEnum Type { get; set; }

    }
}
