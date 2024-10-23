using System.Collections.ObjectModel;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class DependencyValidatorBase(ITicketsDependenciesRepository _ticketsDependenciesRepository) : IDependencyValidatorBase<TicketUpdateDto>
    {
        public async Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency)
        {
            Collection<TicketDependenciesEntity> targetTicketDependencies = await _ticketsDependenciesRepository.GetDependencies(new GetTicketDependencyDto { SourceTicketId = dependency.TargetTicketId }, dependency.Id);

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
            return body.Status.IsPresent;
        }
    }
}
