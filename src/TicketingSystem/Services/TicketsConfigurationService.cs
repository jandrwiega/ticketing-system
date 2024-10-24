﻿using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Services
{
    public class TicketsConfigurationService(ITicketsConfigurationRepository _ticketsConfigurationRepository) : ITicketsConfigurationService
    {
        public async Task<TicketConfigurationMapEntity?> GetConfigurationForType(TicketTypeEnum type)
        {
            return await _ticketsConfigurationRepository.GetConfigurationForType(type);
        }

        public async Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body)
        {
            return await _ticketsConfigurationRepository.CreateConfigurationField(type, body);
        }

        public async Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid metadataId, TicketConfigurationUpdateDto body)
        {
            return await _ticketsConfigurationRepository.UpdateConfigurationField(metadataId, body);
        }

        public async Task DeleteConfigurationField(Guid metadataId)
        {
            await _ticketsConfigurationRepository.DeleteConfigurationField(metadataId);
        }
    }
}
