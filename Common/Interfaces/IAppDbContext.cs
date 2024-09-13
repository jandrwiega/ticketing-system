using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Common.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<TicketEntity> TicketEntities { get; set; }
    }
}
