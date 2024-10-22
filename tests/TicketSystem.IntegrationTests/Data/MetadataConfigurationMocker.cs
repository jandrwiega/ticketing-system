using TicketingSystem.Database.Enums;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database;

namespace TicketingSystem.IntegrationTests.Data
{
    public class MetadataConfigurationMocker(AppDbContext _dbContext)
    {
        public async Task<TicketConfigurationMapEntity> GenerateConfigurationMock(TicketConfigurationMapEntity value)
        {
            var results = await _dbContext.TicketConfigurationMapEntities.AddAsync(value);

            return results.Entity;
        }

        public async Task<TicketMetadataFieldEntity> GenerateConfigurationMetadataMock(TicketTypeEnum type)
        {
            var results = await _dbContext.TicketMetadataFieldEntities.AddAsync(new TicketMetadataFieldEntity() { PropertyName = $"metadata-{type.ToString().ToLower()}", PropertyType = TicketMetadataTypeEnum.String });

            return results.Entity;
        }
    }
}
