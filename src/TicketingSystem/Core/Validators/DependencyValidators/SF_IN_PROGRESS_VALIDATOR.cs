using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class SF_IN_PROGRESS_VALIDATOR : IDependencyValidator<TicketUpdateDto>
    {
        public void CanCreate(Guid sourceId, AppDbContext _dbContext, TicketDependenciesEntity dependency)
        {
            TicketDependenciesEntity[] targetTicketDependencies = [.. _dbContext.TicketDependenciesEntities.Where(it => it.DependencyType == dependency.DependencyType && it.SourceTicketId == dependency.TargetTicketId)];

            if (targetTicketDependencies.Any(targetDependency => targetDependency.TargetTicketId == sourceId))
            {
                throw new Exception("Found dependency error");
            }

            if (targetTicketDependencies.Length > 0)
            {
                foreach (TicketDependenciesEntity targetDependency in targetTicketDependencies)
                {
                    CanCreate(sourceId, _dbContext, targetDependency);
                }
            }
        }

        public bool ShouldValidate(TicketUpdateDto body)
        {
            return body.Status.isPresent;
        }

        public bool Validate(TicketEntity targetEntity)
        {
            return targetEntity.Status != TicketStatusEnum.Open;
        }
    }
}
