using System.Collections.ObjectModel;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITagsRepository<TEntity>
    {
        Task<Collection<TEntity>> GetOrCreateTags(string[] tags);
        Task DeleteTagsRelation(Guid ticketId, Collection<TagEntity> tags);
    }
}
