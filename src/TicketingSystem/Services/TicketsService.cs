using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Services
{
    public class TicketsService(IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto> _ticketsDbRepository) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            return await _ticketsDbRepository.Create(body);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            return await _ticketsDbRepository.Update(ticketId, body);
        }

        public void DeleteTickets()
        {
            _ticketsDbRepository.DeleteAll();
        }
    }
}
