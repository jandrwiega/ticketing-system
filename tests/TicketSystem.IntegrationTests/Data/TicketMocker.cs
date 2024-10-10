using Microsoft.AspNetCore.Mvc.Testing;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;

namespace TicketingSystem.IntegrationTests.Data
{
    public class TicketMocker(AppDbContext _dbContext)
    {
        public async Task<TicketEntity> GenerateTicketMock(TicketEntity value)
        {
            var results = await _dbContext.TicketEntities.AddAsync(value);

            await _dbContext.SaveChangesAsync();

            return results.Entity;
        }
    }
}
