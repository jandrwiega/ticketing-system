using System.ComponentModel.DataAnnotations;
using TicketingSystem.Models.Enums;

namespace TicketingSystem.Common.Models
{
    public class TicketFiltersDto
    {
        public TicketTypeEnum? Type { get; set; }
    }
}
