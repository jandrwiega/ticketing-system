using System.Collections.ObjectModel;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITicketsDependenciesRepository
    {
        Task<Collection<TicketDependenciesEntity>> CreateDependecies(Collection<TicketDependencyDto> elements);
        Task DeleteDependency(Guid dependencyId);
    }
}
