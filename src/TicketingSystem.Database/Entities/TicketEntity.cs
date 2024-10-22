using System.Collections.ObjectModel;
using TicketingSystem.Database.Enums;

namespace TicketingSystem.Database.Entities
{
    public class TicketEntity
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public Guid? Assignee { get; set; }
        public TicketStatusEnum Status { get; set; }
        public DateTime ReportedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public TicketTypeEnum Type { get; set; }
        public string? AffectedVersion { get; set; }
        public Guid[]? RelatedElements { get; set; }
        public Collection<TagEntity> Tags { get; set; } = [];
        public Collection<TicketMetadata> Metadata { get; set; } = [];
        public required TicketConfigurationMapEntity MetadataConfiguration { get; set; }
        public Guid ConfigurationId { get; set; }
        public Collection<TicketDependenciesEntity> Dependencies { get; set; } = [];
    }
}
