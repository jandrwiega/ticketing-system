using TicketingSystem.Models.Enums;

namespace TicketingSystem.Common.Models
{
    public class TicketEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public Guid? Assignee { get; set; }
        public string Status { get; set; }
        public DateTime ReportedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Type { get; set; }
        public string? AffectedVersion { get; set; }
        public Guid[]? RelatedElements { get; set; }
    }
}
