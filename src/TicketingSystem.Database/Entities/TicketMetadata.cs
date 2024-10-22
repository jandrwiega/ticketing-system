using System.Text.Json.Serialization;

namespace TicketingSystem.Database.Entities
{
    public class TicketMetadata
    {
        public Guid Id { get; set; }
        [JsonIgnore]
        public TicketMetadataFieldEntity MetadataConfiguration { get; set; }
        public Guid MetadataId { get; set; }
        public Guid TicketId { get; set; }
        [JsonIgnore]
        public TicketEntity Ticket { get; set; }
        public string Value { get; set; }
    }
}
