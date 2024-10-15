using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface IDependencyValidator<T>
    {
        bool ShouldValidate(T body);
        bool Validate(TicketEntity targetTicket);
    }
}
