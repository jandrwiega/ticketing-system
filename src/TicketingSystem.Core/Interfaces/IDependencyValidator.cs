using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface IDependencyValidator<T>
    {
        bool ShouldValidate(T body);
        void Validate(TicketEntity targetTicket);
        Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency);
    }
}
