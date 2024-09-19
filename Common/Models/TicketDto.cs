using System.ComponentModel.DataAnnotations;
using TicketingSystem.Common.Enums;
using System.Text.Json.Serialization;
using TicketingSystem.Core.Attributes;
using TicketingSystem.Core.Converters;

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

        [Required]
        public required TicketTypeEnum Type { get; set; }

    }

    public class TicketUpdateDto
    {
        [ValidateOptionalMaxLength<string>(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Title { get; set; }

        [ValidateOptionalMaxLength<string>(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid> Assignee { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<TicketStatusEnum> Status { get; set; }

        [ValidateOptionalMinLengthArray<Guid>(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid[]> RelatedElements { get; set; }
    }
}
