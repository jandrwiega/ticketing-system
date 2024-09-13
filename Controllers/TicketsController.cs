using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Services;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("v1/tickets")]
    public class TicketsController(ITicketsService ticketsService) : ControllerBase
    {
        // Add filters for type, assignment, status, affected version (bug) - filters should be optional
        [HttpGet]
        public async Task<IEnumerable<TicketEntity>> GetTickets([FromQuery] TicketFiltersDto filters)
        {
            return await ticketsService.GetTickets(filters);
        }

        [HttpPost]
        public async Task<TicketEntity> CreateTicket([FromBody] TicketCreateDto body)
        {
            return await ticketsService.CreateTicket(body);
        }

        // Update assignment or ticket status
        [HttpPut("{ticketId}")]
        public async Task<TicketEntity> UpdateTicket([FromRoute] Guid ticketId, [FromBody] TicketUpdateDto body)
        {
            return await ticketsService.UpdateTicket(ticketId, body);
        }
    }
}
