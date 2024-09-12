using Microsoft.EntityFrameworkCore;
using System.Collections;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Repositories
{
    public class TicketsDbRepository : IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto>
    {
        private readonly DbContext dbContext;

        public TicketsDbRepository(DbContext context)
        {
            dbContext = context;
        }

        public async Task<IEnumerable<TicketEntity>> Get() 
        {
            return new List<TicketEntity>();
        }

        public async Task<TicketEntity> Create(TicketCreateDto body)
        {
            return new TicketEntity();
        }

        public async Task<TicketEntity> Update(TicketUpdateDto body)
        {
            return new TicketEntity();
        }
    }
}
