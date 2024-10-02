using System.Collections.ObjectModel;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Common.Interfaces
{
    public interface ITagsRepository
    {
        Task<Collection<TagEntity>> GetOrCreateTags(string[] tags);
        Task DeleteTagsRelation(Guid ticketId, Collection<TagEntity> tags);
    }
}
