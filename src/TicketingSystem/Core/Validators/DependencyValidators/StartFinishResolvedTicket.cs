using System.Collections.ObjectModel;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Repositories;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class StartFinishResolvedTicket(ITicketsDependenciesRepository _ticketsDependenciesRepository) : IDependencyValidator<TicketUpdateDto>
    {
        public async Task CanCreate(Guid sourceId, TicketDependenciesEntity dependency)
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
                    await CanCreate(sourceId, targetDependency);
                }
            }
        }

        public bool ShouldValidate(TicketUpdateDto body)
        {
            return body.Status.isPresent;
        }

        public bool Validate(TicketEntity targetEntity)
        {
            return targetEntity.Status == TicketStatusEnum.Resolved;
        }
    }
}
