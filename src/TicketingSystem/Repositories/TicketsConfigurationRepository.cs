using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Converters;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsConfigurationRepository(AppDbContext _dbContext) : ITicketsConfigurationRepository
    {
        public async Task<TicketConfigurationMapEntity> GetConfigurationForType(TicketTypeEnum type)
        {
            return await _dbContext
                .TicketConfigurationMapEntities
                .Include(config => config.Metadata)
                .Where(config => config.TicketType == type)
                .FirstOrDefaultAsync() ?? throw new Exception($"Not found metadata configuration for {type}");
        }

        public async Task<TicketMetadataFieldEntity?> GetMetadataField(Guid metadataId)
        {
            return await _dbContext
                .TicketMetadataFieldEntities
                .Where(metadata => metadata.Id == metadataId)
                .FirstOrDefaultAsync();
        }

        public async Task<TicketMetadataFieldEntity?> GetMetadataFieldByName(string metadataName)
        {
            return await _dbContext
                .TicketMetadataFieldEntities
                .Where(metadata => metadata.PropertyName == metadataName)
                .FirstOrDefaultAsync();
        }

        public async Task<TicketMetadataFieldEntity?> CreateConfigurationField(TicketTypeEnum type, TicketConfigurationDto body)
        {
            TicketConfigurationMapEntity? Configuration = await GetConfigurationForType(type);
            TicketMetadataFieldEntity? fieldExists = await GetMetadataFieldByName(body.PropertyName);

            if (fieldExists is not null)
            {
                bool isFieldDefined = fieldExists.Configurations.Contains(Configuration);

                if (!isFieldDefined)
                {
                    fieldExists.Configurations.Add(Configuration);

                    await _dbContext.SaveChangesAsync();
                }

                return fieldExists;
            }
            else
            {
                TicketMetadataFieldEntity elementToAdd = new()
                {
                    PropertyName = body.PropertyName,
                    PropertyType = body.PropertyType,
                    Configurations = [Configuration]
                };

                var results = await _dbContext.TicketMetadataFieldEntities.AddAsync(elementToAdd);

                await _dbContext.SaveChangesAsync();

                return results.Entity;
            }
        }

        public async Task<TicketMetadataFieldEntity?> UpdateConfigurationField(Guid metadataId, TicketConfigurationUpdateDto body)
        {
            TicketMetadataFieldEntity? oldEntity = await GetMetadataField(metadataId);

            if (oldEntity is not null)
            {
                UpdateIfModified(body.PropertyName, (value) => oldEntity.PropertyName = value ?? "");
                UpdateIfModified(body.PropertyType, (value) => oldEntity.PropertyType = value);

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Metadata not found");
            }

            return oldEntity;
        }

        public async Task DeleteConfigurationField(Guid metadataId)
        {
            TicketMetadataFieldEntity? elementToDelete = await GetMetadataField(metadataId);

            if (elementToDelete is not null)
            {
                _dbContext.TicketMetadataFieldEntities.Remove(elementToDelete);

                await _dbContext.SaveChangesAsync();
            }
        }

        private static void UpdateIfModified<T>(Optional<T> item, Action<T?> action)
        {
            if (item.IsPresent)
            {
                action(item.Value);
            }
        }
    }
}
