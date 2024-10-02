using System.Collections.ObjectModel;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITicketMetadataRepository
    {
        Task<Collection<TicketMetadataEntity>> CreateMetadata(Collection<TicketMetadata> Metadata);
        Task<Collection<TicketMetadataEntity>> UpdateMetadata(Collection<TicketMetadata> Metadata, Guid ticketId);
    }
}
