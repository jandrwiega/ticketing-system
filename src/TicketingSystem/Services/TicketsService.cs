using System.Collections.ObjectModel;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Services
{
    public class TicketsService(
        IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto> _ticketsDbRepository,
        ITagsRepository _ticketTagsDbRepository,
        ITicketMetadataRepository _ticketMapDbRepository
        ) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            Collection<TicketMetadataEntity> TicketMetadata = await _ticketMapDbRepository.CreateMetadata(body.Metadata ?? []);

            TicketSaveDto UpdatedBody = new()
            { 
                Title = body.Title,
                Type = body.Type,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = TicketMetadata
            };

            return await _ticketsDbRepository.Create(UpdatedBody);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            Collection<TicketMetadataEntity> TicketMetadata = await _ticketMapDbRepository.UpdateMetadata(body.Metadata ?? [], ticketId);

            TicketUpdateSaveDto UpdatedBody = new()
            {
                Title = body.Title,
                RelatedElements = body.RelatedElements,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = TicketMetadata
            };

            return await _ticketsDbRepository.Update(ticketId, UpdatedBody);
        }
    }
}
