using System.ComponentModel.DataAnnotations;
using TicketingSystem.Common.Enums;
using TicketingSystem.Core.Converters;

namespace TicketingSystem.Common.Models.Dtos
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
