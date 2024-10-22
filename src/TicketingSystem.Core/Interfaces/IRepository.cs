using TicketingSystem.Core.Dtos;

namespace TicketingSystem.Core.Interfaces
{
    public interface IRepository<TEntity, TCreateDto, TUpdateDto>
        where TEntity : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<TEntity> GetById(Guid ticketId);
        Task<IEnumerable<TEntity>> Get(TicketFiltersDto filters);
        Task<TEntity> Create(TCreateDto body, Guid configurationId);
        Task<TEntity> Update(TEntity entity, TUpdateDto body);
    }
}
