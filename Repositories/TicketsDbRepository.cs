using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Core.Converters;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsDbRepository(AppDbContext context) : IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto>
    {
        private readonly AppDbContext _dbContext = context;
        private readonly Mapper _mapper = new(new MapperConfiguration(config => config
            .CreateMap<TicketCreateDto, TicketEntity>()
            .ForMember(dest => dest.ReportedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
        ));

        public async Task<IEnumerable<TicketEntity>> Get(TicketFiltersDto filters) 
        {
            return await _dbContext.TicketEntities.ApplyFilter<TicketEntity>(filters).ToListAsync();
        }

        public async Task<TicketEntity> Create(TicketCreateDto body)
        {
            var ticket = _mapper.Map<TicketEntity>(body);

            await _dbContext.TicketEntities.AddAsync(ticket);
            await _dbContext.SaveChangesAsync();

            return ticket;
        }

        public async Task<TicketEntity> Update(Guid ticketId, TicketUpdateDto body)
        {
            TicketEntity entity = await _dbContext.TicketEntities.FindAsync(ticketId) ?? throw new KeyNotFoundException("Ticket not found");

            if (body.AffectedVersion.IsPresent)
            {
                if (entity.Type == TicketTypeEnum.Bug)
                {
                    UpdateIfModified<Version>(body.AffectedVersion, entity, "AffectedVersion");
                }
                else
                {
                    throw new BadHttpRequestException("Affected version can be set only for a bug");
                }
            }
            UpdateIfModified<string>(body.Title, entity, "Title");
            UpdateIfModified<string>(body.Description, entity, "Description");
            UpdateIfModified<Guid>(body.Assignee, entity, "Assignee");
            UpdateIfModified<TicketStatusEnum>(body.Status, entity, "Status");

            if (body.RelatedElements.IsPresent)
            {
                await UpdateRelatedElements(entity, body.RelatedElements.Value ?? []);
            }

            bool hasChanges = _dbContext.ChangeTracker.HasChanges();

            if (hasChanges)
            {
                PropertyEntry lastModifiedProps = _dbContext.Entry(entity).Property("LastModifiedDate");

                lastModifiedProps.IsModified = true;
                lastModifiedProps.CurrentValue = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        private void UpdateIfModified<T>(Optional<T> item, TicketEntity entity, string propertyName)
        {
            PropertyEntry prevProp = _dbContext.Entry(entity).Property(propertyName);

            if (item.IsPresent)
            {
                prevProp.IsModified = true;
                prevProp.CurrentValue = item.Value?.GetType() == typeof(Version) ? item.Value.ToString() : item.Value;
            }

            if (item.IsPresent && propertyName == "Status" && item.Value?.ToString() == "Resolved")
            {
                PropertyEntry prevResolvedDateProp = _dbContext.Entry(entity).Property("ResolvedDate");
                prevResolvedDateProp.IsModified = true;
                prevResolvedDateProp.CurrentValue = DateTime.UtcNow;
            }
        }

        private async Task UpdateRelatedElements(TicketEntity entity, Guid[] relatedElements)
        {
            if (entity.Type != TicketTypeEnum.Epic) throw new BadHttpRequestException("Tickets can be related only to Epics");

            List<TicketEntity> relatedTickets = await _dbContext.TicketEntities.Where(p => relatedElements.Contains(p.Id)).ToListAsync();

            if (relatedTickets.Count != relatedElements.Length) throw new BadHttpRequestException("Some of tickets not found");

            bool isAllRelatedElementsValid = relatedTickets.All(item => item.Type != TicketTypeEnum.Epic);

            if (!isAllRelatedElementsValid) throw new BadHttpRequestException("Epic cannot be related of other Epic");

            Guid[] updatedElements = [.. (entity.RelatedElements ?? []), .. relatedElements];
            entity.RelatedElements = (Guid[]?)updatedElements.Distinct().ToArray();
        }
    }
}
