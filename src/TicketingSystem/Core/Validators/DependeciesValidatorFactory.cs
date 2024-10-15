using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Core.Validators.DependencyValidators;

namespace TicketingSystem.Core.Validators
{
    public class DependeciesValidatorFactory
    {
        public static IDependencyValidator<T> GetValidator<T>(TicketDependenciesEnum dependencyType)
        {
            return dependencyType switch
            {
                TicketDependenciesEnum.SF_IN_PROGRESS => (IDependencyValidator<T>)new SF_IN_PROGRESS_VALIDATOR(),
                TicketDependenciesEnum.SF_RESOLVED => (IDependencyValidator<T>)new SF_RESOLVED_VALIDATOR(),
                _ => throw new Exception("Validator not implemented yet"),
            };
        }
    }
}
