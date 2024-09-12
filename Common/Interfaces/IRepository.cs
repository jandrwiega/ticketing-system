using TicketingSystem.Common.Models;

namespace TicketingSystem.Common.Interfaces
{
    public interface IRepository<TEntity, TCreateDto, TUpdateDto>
        where TEntity : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<IEnumerable<TEntity>> Get();
        Task<TEntity> Create(TCreateDto body);
        Task<TEntity> Update(TUpdateDto body);
    }
}
