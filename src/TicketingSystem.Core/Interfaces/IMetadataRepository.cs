using System.Collections.ObjectModel;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface IMetadataRepository
    {
        Task<Collection<TicketMetadata>> CreateMetadata(Dictionary<string, string> metadataBody, TicketConfigurationMapEntity configuration);
    }
}
