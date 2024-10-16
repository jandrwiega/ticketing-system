using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Common.Interfaces
{
    public interface IDependencyValidator<UpdateDto>
    {
        bool ShouldValidate(UpdateDto body);
        bool Validate(TicketEntity targetTicket);
        void CanCreate(Guid sourceId, AppDbContext _dbContext, TicketDependenciesEntity dependency);
    }
}
