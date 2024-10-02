using System.Text.Json.Serialization;
using System.Collections.ObjectModel;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.Common.Models.Entities
{
    public class TicketMetadataEntity
    {
        public Guid Id { get; set; }
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
        public TicketMetadataTypeEnum PropertyType { get; set; }
        [JsonIgnore]
        public TicketEntity TicketEntity { get; set; }
        public Guid? TicketId { get; set; }

        public TicketMetadataEntity() => Id = Guid.NewGuid();
    }
}
