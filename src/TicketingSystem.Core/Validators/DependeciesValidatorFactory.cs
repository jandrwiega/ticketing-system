using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Core.Validators.DependencyValidators;

namespace TicketingSystem.Core.Validators
{
    public interface IDependenciesValidationFactory
    {
        IDependencyValidator<T> GetValidator<T>(TicketDependenciesEnum dependencyType);
    }

    public class DependeciesValidatorFactory(ITicketsDependenciesRepository _ticketsDependenciesRepository) : IDependenciesValidationFactory
    {
        public IDependencyValidator<T> GetValidator<T>(TicketDependenciesEnum dependencyType)
        {
            return dependencyType switch
            {
                TicketDependenciesEnum.SS_DEPENDNECY => (IDependencyValidator<T>)new StartToStartValidator(_ticketsDependenciesRepository),
                TicketDependenciesEnum.SF_DEPENDENCY => (IDependencyValidator<T>)new StartToFinishValidator(_ticketsDependenciesRepository),
                TicketDependenciesEnum.FS_DEPENDENCY => (IDependencyValidator<T>)new FinishToStartValidator(_ticketsDependenciesRepository),
                TicketDependenciesEnum.FF_DEPENDENCY => (IDependencyValidator<T>)new FinishToFinishValidator(_ticketsDependenciesRepository),
                _ => throw new Exception("Validator not implemented yet"),
            };
        }
    }
}
