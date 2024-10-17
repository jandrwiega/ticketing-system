using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Common.Interfaces
{
    public interface IDependencyValidator<T>
    {
        bool ShouldValidate(T body);
        bool Validate(TicketEntity targetTicket);
        void CanCreate(Guid sourceId, AppDbContext _dbContext, TicketDependenciesEntity dependency);
    }
}
