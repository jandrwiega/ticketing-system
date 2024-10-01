using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.Common.Models.Entities
{
    public class TagEntity
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        [JsonIgnore]
        public Collection<TicketEntity> Tickets { get; set; } = new Collection<TicketEntity>();

        public TagEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
