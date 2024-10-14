using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Core.Validators
{
    public class DependeciesValidator(AppDbContext _dbContext)
    {
        private bool ValidateFactory(TicketEntity targetEntity, TicketDependenciesEnum dependencyType)
        {
            switch (dependencyType)
            {
                case TicketDependenciesEnum.SF_IN_PROGRESS:
                    return targetEntity.Status != TicketStatusEnum.Open;
                case TicketDependenciesEnum.SF_RESOLVED:
                    return targetEntity.Status == TicketStatusEnum.Resolved;

                default: return false;
            }
        }

        public async Task<bool> ValidateDependecies(TicketEntity sourceTicket)
        {
            List<bool> dependenciesValidationResults = new();
            foreach (TicketDependenciesEntity dependency in sourceTicket.Dependencies)
            {
                TicketEntity? targetTicket = await _dbContext.TicketEntities.Where(ticket => ticket.Id == dependency.TargetTicketId).FirstOrDefaultAsync();

                if (targetTicket is null) throw new Exception("Target ticket not found");

                bool dependencyValidationResult = ValidateFactory(targetTicket, dependency.DependencyType);

                dependenciesValidationResults.Add(dependencyValidationResult);
            }

            return dependenciesValidationResults.All(it => it == true);
        }
    }
}
