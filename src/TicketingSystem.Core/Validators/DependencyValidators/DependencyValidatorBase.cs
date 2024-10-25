using System.Collections.ObjectModel;
using TicketingSystem.Core.Dtos;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database.Enums;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class DependencyValidatorBase(ITicketsDependenciesRepository _ticketsDependenciesRepository) : IDependencyValidatorBase<TicketUpdateDto>
    {
        private readonly Dictionary<TicketDependenciesEnum, List<TicketDependenciesEnum>> allowedDependencies = new()
        {
            [TicketDependenciesEnum.SS_DEPENDNECY] = [TicketDependenciesEnum.SF_DEPENDENCY],
            [TicketDependenciesEnum.SF_DEPENDENCY] = [TicketDependenciesEnum.FS_DEPENDENCY, TicketDependenciesEnum.FF_DEPENDENCY],
            [TicketDependenciesEnum.FS_DEPENDENCY] = [TicketDependenciesEnum.SF_DEPENDENCY, TicketDependenciesEnum.SS_DEPENDNECY],
            [TicketDependenciesEnum.FF_DEPENDENCY] = [TicketDependenciesEnum.FS_DEPENDENCY]
        };

        public async Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency)
        {
            Collection<TicketDependenciesEntity> targetTicketDependencies = await _ticketsDependenciesRepository.GetDependencies(new GetTicketDependencyDto { SourceTicketId = dependency.TargetTicketId }, dependency.Id);

            List<TicketDependenciesEntity> ticketsWithCircluarDependencies = targetTicketDependencies.Where(targetDependency => targetDependency.TargetTicketId == sourceId).ToList();
            foreach (TicketDependenciesEntity ticketDependency in ticketsWithCircluarDependencies)
            {
                allowedDependencies.TryGetValue(ticketDependency.DependencyType, out List<TicketDependenciesEnum>? value);

                if (!value.Contains(ticketDependency.DependencyType))
                {
                    throw new InvalidOperationException("A circular dependency was detect");
                }
            }

            if (targetTicketDependencies.Any(targetTicketDependency => targetTicketDependency.SourceTicketId != sourceId) && targetTicketDependencies.Count > 0)
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
