using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsDependenciesDbRepository(AppDbContext _dbContext) : ITicketsDependenciesRepository
    {

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

            var results = await _dbContext.TicketDependenciesEntities.AddAsync(element);

            return results.Entity;
        }

        public async Task DeleteDependency(Guid dependencyId)
        {
            _dbContext.Remove(dependencyId);

            await _dbContext.SaveChangesAsync();
        }
    }
}
