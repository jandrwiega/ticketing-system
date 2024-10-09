using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsMetadataDbRepository(AppDbContext _dbContext) : IMetadataRepository
    {
        public async Task<Collection<TicketMetadata>> CreateMetadata(Dictionary<string, string> metadataBody, TicketConfigurationMapEntity configuration)
        {
            Collection<TicketMetadata> createdElements = [];

            foreach (var metadata in metadataBody)
            {
                TicketMetadataFieldEntity? MetadataField = configuration.Metadata.Where(it => it.PropertyName.Equals(metadata.Key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

                if (MetadataField is not null)
                {
                    EntityEntry<TicketMetadata> results = await _dbContext.TicketMetadata.AddAsync(new TicketMetadata() { Value = metadata.Value, MetadataId = MetadataField.Id });

                    createdElements.Add(results.Entity);
                }
                else
                {
                    throw new Exception($"Metadata {metadata.Key} not defined in configuration");
                }
            }

            return createdElements;
        }
    }
}
