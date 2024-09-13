using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Core;
using TicketingSystem.Models.Enums;

namespace TicketingSystem.Repositories
{
    public class TicketsDbRepository : IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Mapper _mapper;

        public TicketsDbRepository(AppDbContext context)
        {
            _dbContext = context;
            _mapper = new Mapper(new MapperConfiguration(config => config.CreateMap<TicketCreateDto, TicketEntity>()));
        }

        public async Task<IEnumerable<TicketEntity>> Get(TicketFiltersDto filters) 
        {
            var filterExpression = FilterBuilder.Build(filters);

            //Where(product => product.Type == filters.Type)

            return await _dbContext.TicketEntities.ToListAsync();
        }

        public async Task<TicketEntity> Create(TicketCreateDto body)
        {
            TicketEntity ticket = _mapper.Map<TicketEntity>(body);

            await _dbContext.TicketEntities.AddAsync(ticket);
            await _dbContext.SaveChangesAsync();

            return ticket;
        }

        public async Task<TicketEntity> Update(Guid ticketId, TicketUpdateDto body)
        {
            TicketEntity entity = await _dbContext.TicketEntities.FindAsync(ticketId);

            foreach (var property in body.GetType().GetProperties())
            {
                string key = property.Name;
                var value = property.GetValue(body);

                var entry = _dbContext.Entry<TicketEntity>(entity);
                var propertyEntry = entry.Property(key);

                if (propertyEntry != null && propertyEntry.Metadata != null && value != null)
                {
                    propertyEntry.CurrentValue = value;
                    propertyEntry.IsModified = true;
                }
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
