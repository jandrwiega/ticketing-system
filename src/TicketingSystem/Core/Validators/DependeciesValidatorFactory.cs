using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Core.Validators.DependencyValidators;

namespace TicketingSystem.Core.Validators
{
    public class DependeciesValidatorFactory
    {
        public static IDependencyValidator GetValidator(TicketDependenciesEnum dependencyType)
        {
            return dependencyType switch
            {
                TicketDependenciesEnum.SF_RESOLVED => new SF_RESOLVED_VALIDATOR(),
                TicketDependenciesEnum.SF_IN_PROGRESS => new SF_IN_PROGRESS_VALIDATOR(),
                _ => throw new Exception("Validator not implemented yet")
            };
        }
    }
}
