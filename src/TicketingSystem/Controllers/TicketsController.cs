﻿using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Controllers
{
    [ApiController]
    [Route("v1/tickets")]
    public class TicketsController(ITicketsService _ticketsService) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<TicketEntity>> GetTickets([FromQuery] TicketFiltersDto filters)
        {
            return await _ticketsService.GetTickets(filters);
        }

        [HttpPost]
        public async Task<TicketEntity> CreateTicket([FromBody] TicketCreateDto body)
        {
            return await _ticketsService.CreateTicket(body);
        }

        [HttpPut("{ticketId}")]
        public async Task<TicketEntity> UpdateTicket([FromRoute] Guid ticketId, [FromBody] TicketUpdateDto body)
        {
            return await _ticketsService.UpdateTicket(ticketId, body);
        }
    }
}
