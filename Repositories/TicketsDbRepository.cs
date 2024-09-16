﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Core;

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
            return await _dbContext.TicketEntities.ApplyFilter<TicketEntity>(filters).ToListAsync();
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
            TicketEntity? entity = await _dbContext.TicketEntities.FindAsync(ticketId) ?? throw new KeyNotFoundException("Ticket not found");
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

        public async Task<TicketEntity> TicketAddRelated(Guid ticketId, TicketAddRelatedDto body)
        {
            TicketEntity? entity = await _dbContext.TicketEntities.FindAsync(ticketId) ?? throw new KeyNotFoundException("Ticket not found");

            if (entity.Type != "Epic") throw new BadHttpRequestException("Tickets can be related only to Epics");

            Guid[]? relatedElementIds = body.RelatedElements;
            List<TicketEntity> relatedTickets = await _dbContext.TicketEntities.Where(p => relatedElementIds.Contains(p.Id)).ToListAsync();

            if (relatedTickets.Count != relatedElementIds.Length) throw new BadHttpRequestException("Some of tickets not found");

            bool isAllRelatedElementsValid = relatedTickets.All(item => item.Type != "Epic");

            if (!isAllRelatedElementsValid) throw new BadHttpRequestException("Epic cannot be related of other Epic");

            Guid[] updatedElements = [.. (entity.RelatedElements ?? []), .. body.RelatedElements];
            entity.RelatedElements = (Guid[]?)updatedElements.Distinct().ToArray();

            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
    }
}
