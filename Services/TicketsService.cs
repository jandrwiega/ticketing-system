using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Services
{
    public class TicketsService(IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto> _ticketsDbRepository) : ITicketsService
    {
        private readonly IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto> ticketsDbRepository = _ticketsDbRepository;

        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            return await ticketsDbRepository.Create(body);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            return await ticketsDbRepository.Update(ticketId, body);
        }
    }
}
