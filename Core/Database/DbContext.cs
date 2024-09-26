using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TicketEntity> TicketEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketEntity>().ToTable("tickets");
            modelBuilder.HasPostgresEnum<TicketTypeEnum>("TicketTypeEnum");
            modelBuilder.HasPostgresEnum<TicketStatusEnum>("TicketStatusEnum");
            base.OnModelCreating(modelBuilder);
        }
    }
}
