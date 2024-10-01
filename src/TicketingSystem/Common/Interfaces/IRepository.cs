using TicketingSystem.Common.Models.Dtos;

namespace TicketingSystem.Common.Interfaces
{
    public interface IRepository<TEntity, TCreateDto, TUpdateDto>
        where TEntity : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<IEnumerable<TEntity>> Get(TicketFiltersDto filters);
        Task<TEntity> Create(TCreateDto body);
        Task<TEntity> Update(Guid itemId, TUpdateDto body);
    }
}
