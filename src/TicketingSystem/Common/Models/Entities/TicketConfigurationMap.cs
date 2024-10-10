using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using TicketingSystem.Common.Enums;

namespace TicketingSystem.Common.Models.Entities
{
    public class TicketConfigurationMapEntity
    {
        public Guid Id { get; set; }
        public TicketTypeEnum TicketType { get; set; }
        [JsonPropertyName("metadata")]
        public Collection<TicketMetadataFieldEntity> Metadata { get; set; } = [];
        [JsonIgnore]
        public Collection<TicketEntity> Tickets { get; set; } = [];

        public TicketConfigurationMapEntity() => Id = Guid.NewGuid();
    }

    public class TicketMetadataFieldEntity
    {
        public Guid Id { get; set; }
        public required string PropertyName { get; set; }
        public TicketMetadataTypeEnum PropertyType { get; set; }
        [JsonIgnore]
        public Collection<TicketConfigurationMapEntity> Configurations { get; set; } = [];
        [JsonIgnore]
        public Collection<TicketMetadata> MetadataValues { get; set; } = [];

        public TicketMetadataFieldEntity() => Id = Guid.NewGuid();
    }
}
