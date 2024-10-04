using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Services
{
    public class TicketsService(
        IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto> _ticketsDbRepository,
        ITagsRepository _ticketTagsDbRepository
        ) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);

            TicketSaveDto UpdatedBody = new()
            { 
                Title = body.Title,
                Type = body.Type,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = body.Metadata
            };

            return await _ticketsDbRepository.Create(UpdatedBody);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            Collection<TagEntity> Tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);

            TicketUpdateSaveDto UpdatedBody = new()
            {
                Title = body.Title,
                RelatedElements = body.RelatedElements,
                Tags = Tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = body.Metadata
            };

            return await _ticketsDbRepository.Update(ticketId, UpdatedBody);
        }
    }
}
