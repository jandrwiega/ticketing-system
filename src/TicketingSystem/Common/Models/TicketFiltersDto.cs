namespace TicketingSystem.Common.Models
{
    public class TicketFiltersDto
    {
        public string? Type { get; set; }
        public Guid? Assignee { get; set; }
        public string? Status { get; set; }
        public string? AffectedVersion { get; set; }
    }
}
