using System.Collections.ObjectModel;
using TicketingSystem.Database.Entities;

namespace TicketingSystem.Core.Interfaces
{
    public interface ITagsRepository
    {
        Task<Collection<TagEntity>> GetOrCreateTags(string[] tags);
        Task DeleteTagsRelation(Guid ticketId, Collection<TagEntity> tags);
    }
}
