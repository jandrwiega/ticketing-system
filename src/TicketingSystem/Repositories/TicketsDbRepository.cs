using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Core.Converters;
using TicketingSystem.Database;
using TicketingSystem.Core.Validators;
using TicketingSystem.Core.Dtos;

namespace TicketingSystem.Repositories
{
    public class TicketsDbRepository(
        AppDbContext _dbContext,
        ITagsRepository _ticketTagsDbRepository,
        ITicketsConfigurationRepository _ticketsConfigurationRepository,
        ITicketsDependenciesRepository _ticketsDependenciesRepository,
        IDependenciesValidationFactory _dependenciesValidatorFactory
        ) : IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto>
    {
        private readonly Mapper _mapper = new(new MapperConfiguration(config => config
            .CreateMap<TicketSaveDto, TicketEntity>()
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

            return await builder
                .Include(prop => prop.Tags)
                .Include(prop => prop.MetadataConfiguration)
                .ThenInclude(metadataProp => metadataProp.Metadata)
                .Include(prop => prop.Metadata)
                .Include(prop => prop.Dependencies)
                .ToListAsync();
        }

        public async Task<TicketEntity> GetById(Guid ticketId)
        {
            var builder = _dbContext.TicketEntities.AsQueryable();

            return await builder
                .Include(it => it.Tags)
                .Include(it => it.MetadataConfiguration)
                .ThenInclude(it => it.Metadata)
                .Include(it => it.Dependencies) 
                .Where(it => it.Id == ticketId)
                .FirstAsync() ?? throw new KeyNotFoundException("Ticket not found");
        }

        public async Task<TicketEntity> Create(TicketSaveDto body, Guid configurationId)
        {
            TicketEntity ticket = _mapper.Map<TicketEntity>(body);
            ticket.ConfigurationId = configurationId;
            ticket.Dependencies = body.Dependencies;

            await _dbContext.TicketEntities.AddAsync(ticket);

            if (body.Dependencies.Count > 0)
            {
                foreach (TicketDependenciesEntity dependency in ticket.Dependencies)
                {
                    IDependencyValidator<TicketUpdateDto> validator = _dependenciesValidatorFactory.GetValidator<TicketUpdateDto>(dependency.DependencyType);
                    
                    try
                    {
                        await validator.CanCreateAsync(ticket.Id, dependency);
                    }
                    catch
                    {
                        foreach (TicketDependenciesEntity revertDependencies in ticket.Dependencies)
                        {
                            await _ticketsDependenciesRepository.DeleteDependency(revertDependencies.Id);
                        }
                        throw;
                    }

                    dependency.SourceTicketId = ticket.Id;
                    dependency.SourceTicket = ticket;
                }
            }

            await _dbContext.SaveChangesAsync();

            return ticket;
        }

        public async Task<TicketEntity> Update(TicketEntity entity, TicketUpdateSaveDto body)
        {
            if (body.AffectedVersion.isPresent)
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
            UpdateIfModified(body.Title, value => entity.Title = value ?? throw new Exception("Title cannot be null"));
            UpdateIfModified(body.Description, value => entity.Description = value);
            UpdateIfModified(body.Assignee, value => entity.Assignee = value);
            UpdateIfModified(body.Status, value =>
            {
                entity.Status = value;

                if (value == TicketStatusEnum.Resolved)
                {
                    entity.ResolvedDate = DateTime.UtcNow;
                }
            });

            if (body.RelatedElements.isPresent)
            {
                await UpdateRelatedElements(entity, body.RelatedElements.value ?? []);
            }

            if (body.Tags.Count > 0)
            {
                await _ticketTagsDbRepository.DeleteTagsRelation(entity.Id, entity.Tags);
                entity.Tags = body.Tags;
            }

            TicketConfigurationMapEntity? configuration = await _ticketsConfigurationRepository.GetConfigurationForType(entity.Type);

            if (body.Metadata.Count > 0 && configuration is not null)
            {
                entity.Metadata = body.Metadata;
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
            if (item.isPresent)
            {
                action(item.value);
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
