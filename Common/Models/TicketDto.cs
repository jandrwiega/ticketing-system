using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using TicketingSystem.Models.Enums;

namespace TicketingSystem.Common.Models
{
    public class TicketCreateDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public required string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string? Description { get; set; }

        public Guid? Assignee { get; set; }

        public TicketStatusEnum? Status { get; set; }

        public TicketTypeEnum? Type { get; set; }

    }

    public class TicketUpdateDto
    {
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public string? Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string? Description { get; set; }

        public Guid? Assignee { get; set; }

        public TicketStatusEnum? Status { get; set; }
    }
}
