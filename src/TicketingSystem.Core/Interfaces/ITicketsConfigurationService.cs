using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Enums;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface ITicketsConfigurationService
    {
        Task<TicketConfigurationMapEntity?> GetConfigurationForType(TicketTypeEnum type);
        Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body);
        Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid MetadataId, TicketConfigurationUpdateDto body);
        Task DeleteConfigurationField(Guid MetadataId);
    }
}
