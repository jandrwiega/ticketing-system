using TicketingSystem.Core.Validators.DependencyValidators;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface IDependencyValidatorBase<T>
    {
        bool ShouldValidate(T body);
        Task CanCreateAsync(Guid sourceId, TicketDependenciesEntity dependency);
    }

    public interface IDependencyValidator<T> : IDependencyValidatorBase<T>
    {
        void Validate(TicketEntity targetTicket);
    }
}
