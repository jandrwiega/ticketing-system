using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Core.Validators.DependencyValidators;

namespace TicketingSystem.Core.Validators
{
    public interface IDependenciesValidationFactory
    {
        IDependencyValidator<T> GetValidator<T>(TicketDependenciesEnum dependencyType);
    }

    public class DependeciesValidatorFactory(ITicketsDependenciesRepository _ticketsDependenciesRepository): IDependenciesValidationFactory
    {
        public IDependencyValidator<T> GetValidator<T>(TicketDependenciesEnum dependencyType)
        {
            return dependencyType switch
            {
                TicketDependenciesEnum.SF_IN_PROGRESS => (IDependencyValidator<T>)new StartFinishInProgressTicket(_ticketsDependenciesRepository),
                TicketDependenciesEnum.SF_RESOLVED => (IDependencyValidator<T>)new StartFinishResolvedTicket(_ticketsDependenciesRepository),
                _ => throw new Exception("Validator not implemented yet"),
            };
        }
    }
}
