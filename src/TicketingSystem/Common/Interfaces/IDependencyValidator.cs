using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface IDependencyValidator<UpdateDto>
    {
        bool ShouldValidate(UpdateDto body);
        bool Validate(TicketEntity targetTicket);
        bool CanCreate();
    }
}
