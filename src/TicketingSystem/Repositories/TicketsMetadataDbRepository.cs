using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsMetadataDbRepository(AppDbContext _dbContext) : ITicketMetadataRepository
    {
        public async Task<Collection<TicketMetadataEntity>> CreateMetadata(Collection<TicketMetadata> Metadata)
        {
            Collection<TicketMetadataEntity> CreatedMetadata = [];

            foreach (TicketMetadata item in Metadata)
            {
                var results = _dbContext.TicketMetadataEntities.Add(new TicketMetadataEntity()
                {
                    PropertyName = item.PropertyName,
                    PropertyType = item.PropertyType,
                    PropertyValue = item.PropertyValue,
                    TicketId = item?.TicketId
                });

                CreatedMetadata.Add(results.Entity);
            };

            await _dbContext.SaveChangesAsync();

            return CreatedMetadata;
        }

        public async Task<Collection<TicketMetadataEntity>> UpdateMetadata(Collection<TicketMetadata> Metadata, Guid ticketId)
        {
            var EntitiesToDelete = _dbContext.TicketMetadataEntities.Where(it => it.TicketId == ticketId);
            _dbContext.TicketMetadataEntities.RemoveRange(EntitiesToDelete);

            foreach (TicketMetadata item in Metadata)
            {
                item.TicketId = ticketId;
            }

            return await this.CreateMetadata(Metadata);
        }
    }
}
