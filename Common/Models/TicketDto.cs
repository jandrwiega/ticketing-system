using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using TicketingSystem.Models.Enums;

namespace TicketingSystem.Common.Models
{
    public class TicketCreateDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string Description { get; set; }

        public string? Assignee { get; set; }

        [Required]
        public TicketStatusEnum Status { get; set; }

        [Required]
        public TicketTypeEnum Type { get; set; }

    }

    public class TicketUpdateDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public string Title { get; set; }

        [Required]
        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string Description { get; set; }

        public string? Assignee { get; set; }

        [Required]
        public TicketStatusEnum Status { get; set; }
    }
}
