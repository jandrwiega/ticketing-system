using System.ComponentModel.DataAnnotations;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Core.Dtos
{
    public class TicketConfigurationDto
    {
        [Required]
        public required string PropertyName { get; set; }

        [Required]
        public TicketMetadataTypeEnum PropertyType { get; set; }
    }

    public class TicketConfigurationUpdateDto
    {
        public Optional<string> PropertyName { get; set; }

        public Optional<TicketMetadataTypeEnum> PropertyType { get; set; }
    }
}
