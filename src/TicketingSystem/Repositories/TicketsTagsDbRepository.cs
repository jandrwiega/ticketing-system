using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsTagsDbRepository(AppDbContext _dbContext) : ITagsRepository
    {
        public async Task<Collection<TagEntity>> GetOrCreateTags(string[] tags)
        {
            Collection<TagEntity> createdTags = [];
            foreach (string tag in tags)
            {
                TagEntity? tagEntity = await _dbContext.TagEntities.FirstOrDefaultAsync(it => it.Content == tag);

                if (tagEntity is not null)
                {
                    createdTags.Add(tagEntity);
                }
                else
                {
                    EntityEntry<TagEntity> result = await _dbContext.TagEntities.AddAsync(new TagEntity() { Content = tag });

                    createdTags.Add(result.Entity);
                }
            }

            return createdTags;
        }

        public async Task DeleteTagsRelation(Guid ticketId, Collection<TagEntity> tags)
        {
            var set = _dbContext.Set<Dictionary<string, object>>("TagEntityTicketEntity")
               .AsNoTracking()
               .ToList();

            foreach (TagEntity tag in tags)
            {
                set.Remove(new Dictionary<string, object>
                {
                    { "TicketId", ticketId },
                    { "TagId", tag.Id }
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
