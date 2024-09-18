using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core.Database
{
    public class AppDbContext(IConfiguration configuration) : DbContext
    {
        private readonly IConfiguration _configuration = configuration;

        public DbSet<TicketEntity> TicketEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DB");

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TicketEntity>().ToTable("tickets");
            modelBuilder.HasPostgresEnum<TicketTypeEnum>("TicketTypeEnum");
            modelBuilder.HasPostgresEnum<TicketStatusEnum>("TicketStatusEnum");
            base.OnModelCreating(modelBuilder);
        }
    }
}
