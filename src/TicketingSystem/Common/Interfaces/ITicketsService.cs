﻿using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITicketsService
    {
        Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters);
        Task<TicketEntity> CreateTicket(TicketCreateDto body);
        Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body);
    }
}
