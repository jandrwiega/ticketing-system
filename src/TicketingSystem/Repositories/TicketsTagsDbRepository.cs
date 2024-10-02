using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.Repositories
{
    public class TicketsTagsDbRepository(AppDbContext _dbContext) : ITagsRepository
    {
        public async Task<Collection<TagEntity>> GetOrCreateTags(string[] tags)
        {
            Collection<TagEntity> Tags = [];
            foreach (string tag in tags)
            {
                TagEntity? isExists = await _dbContext.TagEntities.FirstOrDefaultAsync(it => it.Content == tag);

                if (isExists is not null)
                {
                    Tags.Add(isExists);
                }
                else
                {
                    EntityEntry<TagEntity> result = await _dbContext.TagEntities.AddAsync(new TagEntity() { Content = tag });

                    Tags.Add(result.Entity);
                }
            }

            return Tags;
        }

        public async Task DeleteTagsRelation(Guid ticketId, Collection<TagEntity> tags)
        {
            var Set = _dbContext.Set<Dictionary<string, object>>("TagEntityTicketEntity")
               .AsNoTracking()
               .ToList();

            foreach (TagEntity tag in tags)
            {
                Set.Remove(new Dictionary<string, object>
                {
                    { "TicketId", ticketId },
                    { "TagId", tag.Id }
                });
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
