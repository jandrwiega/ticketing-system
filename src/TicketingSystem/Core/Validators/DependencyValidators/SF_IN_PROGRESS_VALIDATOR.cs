using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Core.Validators.DependencyValidators
{
    public class SF_IN_PROGRESS_VALIDATOR : IDependencyValidator<TicketUpdateDto>
    {
        public bool CanCreate()
        {
            throw new NotImplementedException();
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
