using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("v1/tickets-configuration")]
    public class TicketsConfigurationController(ITicketsConfigurationService _ticketsConfigurationService) : ControllerBase
    {
        [HttpGet("{type}")]
        public async Task<TicketConfigurationMapEntity?> GetConfigurationForType([FromRoute] TicketTypeEnum type)
        {
            return await _ticketsConfigurationService.GetConfigurationForType(type);
        }

        [HttpPost("{type}")]
        public async Task<TicketMetadataFieldEntity?> CreateConfigurationField([FromRoute] TicketTypeEnum type, [FromBody] TicketConfigurationDto body)
        {
            return await _ticketsConfigurationService.CreateConfigurationField(type, body);
        }

        [HttpPut("{metadataId}")]
        public async Task<TicketMetadataFieldEntity?> UpdateConfigurationField([FromRoute] Guid metadataId, [FromBody] TicketConfigurationUpdateDto body)
        {
            return await _ticketsConfigurationService.UpdateConfigurationField(metadataId, body);
        }

        [HttpDelete("{metadataId}")]
        public async Task DeleteConfigurationField([FromRoute] Guid metadataId)
        {
            await _ticketsConfigurationService.DeleteConfigurationField(metadataId);
        }
    }
}
