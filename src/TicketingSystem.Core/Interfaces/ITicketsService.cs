using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface ITicketsService
    {
        Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters);
        Task<TicketEntity> CreateTicket(TicketCreateDto body);
        Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body);
    }
}
