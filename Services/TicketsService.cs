using TicketingSystem.Common.Models;
using TicketingSystem.Repositories;

namespace TicketingSystem.Services
{
    public class TicketsService
    {
        private readonly TicketsDbRepository ticketsDbRepository;

        public TicketsService(TicketsDbRepository _ticketsDbRepository)
        {
            ticketsDbRepository = _ticketsDbRepository;
        }

        public async Task<IEnumerable<TicketEntity>> GetTickets()
        {
            return await ticketsDbRepository.Get();
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            return await ticketsDbRepository.Create(body);
        }

        public async Task<TicketEntity> UpdateTicket(TicketUpdateDto body)
        {
            return await ticketsDbRepository.Update(body);
        }
    }
}
