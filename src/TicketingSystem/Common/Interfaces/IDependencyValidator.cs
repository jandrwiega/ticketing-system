using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Common.Interfaces
{
    public interface IDependencyValidator<T>
    {
        bool ShouldValidate(T body);
        void Validate(TicketEntity targetTicket);
        Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency);
    }
}
