using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsDependenciesDbRepository(AppDbContext _dbContext) : ITicketsDependenciesRepository
    {
        public async Task<Collection<TicketDependenciesEntity>> GetDependencies(GetTicketDependencyDto options)
        {
            var builder = _dbContext.TicketDependenciesEntities.AsQueryable();

            if (options.DependencyType is not null)
            {
                builder.Where(dependency => dependency.DependencyType == options.DependencyType);
            }

            if (options.TargetTicketId is not null)
            {
                builder.Where(dependency => dependency.TargetTicketId == options.TargetTicketId);
            }

            if (options.SourceTicketId is not null)
            {
                builder.Where(dependency => dependency.SourceTicketId == options.SourceTicketId);
            }

            return new Collection<TicketDependenciesEntity>(await builder.ToArrayAsync());
        }

        public async Task<Collection<TicketDependenciesEntity>> CreateDependecies(Collection<TicketDependencyDto> elements)
        {
            Collection<TicketDependenciesEntity> createdElements = [];

            foreach (TicketDependencyDto element in elements)
            {
                TicketDependenciesEntity result = await CreateDependency(element);

                createdElements.Add(result);
            }

            await _dbContext.SaveChangesAsync();

            return createdElements;
        }

        private async Task<TicketDependenciesEntity> CreateDependency(TicketDependencyDto body)
        {
            TicketDependenciesEntity element = new()
            {
                DependencyType = body.DependencyType,
                TargetTicketId = body.TargetTicketId
            };

            if (body.SourceTicketId is not null)
            {
                element.SourceTicketId = body.SourceTicketId ?? Guid.Empty;
            }

            var results = await _dbContext.TicketDependenciesEntities.AddAsync(element);

            return results.Entity;
        }

        public async Task DeleteDependency(Guid dependencyId)
        {
            TicketDependenciesEntity? EntityToRemove = await _dbContext.TicketDependenciesEntities.Where(dependency => dependency.Id == dependencyId).FirstOrDefaultAsync();

            if (EntityToRemove is not null)
            {
                _dbContext.Remove(EntityToRemove);

                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
