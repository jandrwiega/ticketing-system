using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TicketingSystem.Database.Enums;

namespace TicketingSystem.Database.Entities
{
    public class TicketDependenciesEntity
    {
        public Guid Id { get; set; }
        public TicketDependenciesEnum DependencyType { get; set; }
        [JsonIgnore]
        [NotMapped]
        public TicketEntity SourceTicket { get; set; }
        public Guid SourceTicketId { get; set; }
        [JsonIgnore]
        [NotMapped]
        public TicketEntity TargetTicket { get; set; }
        public Guid TargetTicketId { get; set; }
        public TicketDependenciesEntity() => Id = Guid.NewGuid();
    }
}
