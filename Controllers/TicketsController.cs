using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Common.Models;
using TicketingSystem.Services;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("v1/tickets")]
    public class TicketsController(TicketsService ticketsService) : ControllerBase
    {
        // Add filters for type, assignment, status, affected version (bug) - filters should be optional
        [HttpGet]
        public async Task<IEnumerable<TicketEntity>> GetTickets()
        {
            return await ticketsService.GetTickets();
        }

        [HttpPost]
        public async Task<TicketEntity> CreateTicket([FromBody] TicketCreateDto body)
        {
            return await ticketsService.CreateTicket(body);
        }

        // Update assignment or ticket status
        [HttpPut]
        public async Task<TicketEntity> UpdateTicket([FromBody] TicketUpdateDto body)
        {
            return await ticketsService.UpdateTicket(body);
        }
    }
}
