using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Core.Converters;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsDbRepository(AppDbContext _dbContext) : IRepository<TicketEntity, TicketCreateDto, TicketUpdateDto>
    {
        private readonly Mapper _mapper = new(new MapperConfiguration(config => config
            .CreateMap<TicketCreateDto, TicketEntity>()
            .ForMember(dest => dest.ReportedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
        ));

        public async Task<IEnumerable<TicketEntity>> Get(TicketFiltersDto filters) 
        {
            var builder = _dbContext.TicketEntities.AsQueryable();

            if (filters.Type is not null)
            {
                if (Enum.TryParse(filters.Type, true, out TicketTypeEnum enumValue))
                {
                    builder = builder.Where(prop => prop.Type == enumValue);
                }
                else
                {
                    throw new Exception("Type not allowed");
                }
            }

            if (filters.Assignee is not null)
            {
                builder = builder.Where(prop => prop.Assignee == filters.Assignee);
            }

            if (filters.Status is not null)
            {
                if (Enum.TryParse(filters.Status, true, out TicketStatusEnum enumValue))
                {
                    builder = builder.Where(prop => prop.Status == enumValue);
                }
                else
                {
                    throw new Exception("Status not allowed");
                }
            }

            if (filters.AffectedVersion is not null)
            {
                builder = builder.Where(prop => prop.AffectedVersion == filters.AffectedVersion);
            }

            return await builder.ToListAsync();
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
                    UpdateIfModified(body.AffectedVersion, (value) => entity.AffectedVersion = value?.ToString());
                }
                else
                {
                    throw new BadHttpRequestException("Affected version can be set only for a bug");
                }
            }
            UpdateIfModified(body.Title, (value) => entity.Title = value ?? throw new Exception("Title cannot be null"));
            UpdateIfModified(body.Description, (value) => entity.Description = value);
            UpdateIfModified(body.Assignee, (value) => entity.Assignee = value);
            UpdateIfModified(body.Status, (value) =>
            {
                entity.Status = value;

                if (value == TicketStatusEnum.Resolved)
                {
                    entity.ResolvedDate = DateTime.UtcNow;
                }
            });

            if (body.RelatedElements.IsPresent)
            {
                await UpdateRelatedElements(entity, body.RelatedElements.Value ?? []);
            }

            bool hasChanges = _dbContext.ChangeTracker.HasChanges();

            if (hasChanges)
            {
                entity.LastModifiedDate = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        private static void UpdateIfModified<T>(Optional<T> item, Action<T?> action)
        {
            if (item.IsPresent)
            {
                action(item.Value);
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

        public void DeleteAll()
        {
            var entities = _dbContext.TicketEntities.ToList();
            _dbContext.TicketEntities.RemoveRange(entities);
            _dbContext.SaveChanges();
        }
    }
}
