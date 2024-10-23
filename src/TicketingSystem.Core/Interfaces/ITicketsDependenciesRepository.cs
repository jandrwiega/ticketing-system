using System.Collections.ObjectModel;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface ITicketsDependenciesRepository
    {
        Task<Collection<TicketDependenciesEntity>> GetDependencies(GetTicketDependencyDto options, Guid? sourceDependencyId = null);
        Task<Collection<TicketDependenciesEntity>> CreateDependecies(Collection<TicketDependencyDto> elements);
        Task DeleteDependency(Guid dependencyId);
    }
}
