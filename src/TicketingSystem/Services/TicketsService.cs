using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Repositories;

namespace TicketingSystem.Services
{
    public class TicketsService(
        IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto> _ticketsDbRepository,
        ITagsRepository _ticketTagsDbRepository,
        IMetadataRepository _ticketMetadataDbRepository,
        ITicketsConfigurationRepository _ticketsConfigurationRepository
        ) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            TicketConfigurationMapEntity? configuration = await _ticketsConfigurationRepository.GetConfigurationForType(body.Type);

            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            Collection<TicketMetadata> Metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);

            TicketSaveDto UpdatedBody = new()
            { 
                Title = body.Title,
                Type = body.Type,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = Metadata
            };

            return await _ticketsDbRepository.Create(UpdatedBody, configuration.Id);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            TicketEntity entity = await _ticketsDbRepository.GetById(ticketId);

            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            TicketConfigurationMapEntity configuration = await _ticketsConfigurationRepository.GetConfigurationForType(entity.Type);

            Collection<TicketMetadata> Metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);

            TicketUpdateSaveDto UpdatedBody = new()
            {
                Title = body.Title,
                RelatedElements = body.RelatedElements,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = Metadata
            };

            return await _ticketsDbRepository.Update(entity, UpdatedBody);
        }
    }
}
