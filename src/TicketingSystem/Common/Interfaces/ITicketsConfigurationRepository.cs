﻿using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITicketsConfigurationRepository
    {
        Task<TicketConfigurationMapEntity?> GetConfigurationForType(TicketTypeEnum type);
        Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body);
        Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid metadataId, TicketConfigurationUpdateDto body);
        Task DeleteConfigurationField(Guid metadataId);
    }
}
