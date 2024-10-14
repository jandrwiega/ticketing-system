using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;
using TicketingSystem.Core.Validators;
using TicketingSystem.Repositories;

namespace TicketingSystem.Services
{
    public class TicketsService(
        AppDbContext _dbContext,
        IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto> _ticketsDbRepository,
        ITagsRepository _ticketTagsDbRepository,
        IMetadataRepository _ticketMetadataDbRepository,
        ITicketsConfigurationRepository _ticketsConfigurationRepository,
        ITicketsDependenciesRepository _ticketsDependenciesRepository
        ) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            TicketConfigurationMapEntity? configuration = await _ticketsConfigurationRepository.GetConfigurationForType(body.Type);

            Collection<TagEntity> tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            Collection<TicketMetadata> metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);
            Collection<TicketDependenciesEntity> dependencies = await _ticketsDependenciesRepository.CreateDependecies(body.Dependencies ?? []);

            TicketSaveDto UpdatedBody = new()
            { 
                Title = body.Title,
                Type = body.Type,
                Tags = tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = metadata,
                Dependencies = dependencies
            };

            return await _ticketsDbRepository.Create(UpdatedBody, configuration.Id);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            TicketEntity entity = await _ticketsDbRepository.GetById(ticketId);

            DependeciesValidator validator = new(_dbContext);

            bool validationResult = await validator.ValidateDependecies(entity);

            if (validationResult == false) throw new Exception("Please resolve ticket dependencies first");

            Collection<TagEntity> tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            TicketConfigurationMapEntity configuration = await _ticketsConfigurationRepository.GetConfigurationForType(entity.Type);
            
            Collection<TicketMetadata> metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);

            TicketUpdateSaveDto updatedBody = new()
            {
                Title = body.Title,
                RelatedElements = body.RelatedElements,
                Tags = tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = metadata
            };

            return await _ticketsDbRepository.Update(entity, updatedBody);
        }
    }
}
