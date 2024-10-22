using System.Collections.ObjectModel;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class StartFinishInProgressTicket(ITicketsDependenciesRepository _ticketsDependenciesRepository) : IDependencyValidator<TicketUpdateDto>
    {
        public async Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency)
        {
            Collection<TicketDependenciesEntity> targetTicketDependencies = await _ticketsDependenciesRepository.GetDependencies(new GetTicketDependencyDto { DependencyType = dependency.DependencyType, SourceTicketId = dependency.TargetTicketId });

            if (targetTicketDependencies.Any(targetDependency => targetDependency.TargetTicketId == sourceId))
            {
                throw new InvalidOperationException("A circular dependency was detect");
            }

            if (targetTicketDependencies.Count > 0)
            {
                foreach (TicketDependenciesEntity targetDependency in targetTicketDependencies)
                {
                    await CanCreateAsync(sourceId, targetDependency);
                }
            }
        }

        public bool ShouldValidate(TicketUpdateDto body)
        {
            return body.Status.isPresent;
        }

        public void Validate(TicketEntity targetEntity)
        {
            bool validationResults = targetEntity.Status != TicketStatusEnum.Open;

            if (!validationResults)
            {
                throw new InvalidOperationException("Some of dependencies conditions doesn't meet");
            }
        }
    }
}
