using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace TicketingSystem.Common.Models.Entities
{
    public class TagEntity
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        [JsonIgnore]
        public Collection<TicketEntity> Tickets { get; set; } = [];

        public TagEntity()
        {
            Id = Guid.NewGuid();
        }
    }
}
