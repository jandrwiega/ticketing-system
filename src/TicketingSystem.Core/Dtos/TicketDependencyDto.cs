using System.ComponentModel.DataAnnotations;
using TicketingSystem.Database.Enums;

namespace TicketingSystem.Core.Dtos
{
    public class TicketDependencyDto
    {
        [Required]
        public required TicketDependenciesEnum DependencyType { get; set; }

        public Guid? SourceTicketId { get; set; }

        [Required]
        public required Guid TargetTicketId { get; set; }
    }

    public class GetTicketDependencyDto
    {
        public TicketDependenciesEnum? DependencyType { get; set; }
        public Guid? TargetTicketId { get; set; }
        public Guid? SourceTicketId { get; set; }
    }
}
