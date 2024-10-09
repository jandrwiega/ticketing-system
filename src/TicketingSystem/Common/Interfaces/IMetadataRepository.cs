using System.Collections.ObjectModel;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface IMetadataRepository
    {
        Task<Collection<TicketMetadata>> CreateMetadata(Dictionary<string, string> metadataBody, TicketConfigurationMapEntity configuration);
    }
}
