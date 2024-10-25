using TicketingSystem.Core.Dtos;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database.Enums;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class FinishToFinishValidator(ITicketsDependenciesRepository _ticketsDependenciesRepository) : DependencyValidatorBase(_ticketsDependenciesRepository), IDependencyValidator<TicketUpdateDto>
    {
        public void Validate(TicketEntity sourceEntity, TicketEntity targetEntity)
        {
            bool validationResults = sourceEntity.Status == TicketStatusEnum.Resolved && targetEntity.Status == TicketStatusEnum.Resolved;

            if (!validationResults)
            {
                throw new InvalidOperationException("Some of dependencies conditions doesn't met");
            }
        }
    }
}
