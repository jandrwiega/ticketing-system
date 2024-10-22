using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Enums;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface ITicketsConfigurationRepository
    {
        Task<TicketConfigurationMapEntity> GetConfigurationForType(TicketTypeEnum type);
        Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body);
        Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid metadataId, TicketConfigurationUpdateDto body);
        Task DeleteConfigurationField(Guid metadataId);
    }
}
