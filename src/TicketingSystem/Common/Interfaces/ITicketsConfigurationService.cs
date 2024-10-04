using System.Reflection.Metadata;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITicketsConfigurationService
    {
        Task<TicketConfigurationMapEntity?> GetConfigurationForType(TicketTypeEnum type);
        Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body);
        Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid MetadataId, TicketConfigurationUpdateDto body);
        Task DeleteConfigurationField(Guid MetadataId);
    }
}
