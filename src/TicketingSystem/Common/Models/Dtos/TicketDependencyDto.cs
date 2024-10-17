using System.ComponentModel.DataAnnotations;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Models.Dtos
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
