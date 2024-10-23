using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class StartFinishResolvedTicket(ITicketsDependenciesRepository _ticketsDependenciesRepository) : DependencyValidatorBase(_ticketsDependenciesRepository), IDependencyValidator<TicketUpdateDto>
    {
        public void Validate(TicketEntity targetEntity)
        {
            bool validationResults = targetEntity.Status == TicketStatusEnum.Resolved;

            if (!validationResults)
            {
                throw new InvalidOperationException("Some of dependencies conditions doesn't met");
            }
        }
    }
}
