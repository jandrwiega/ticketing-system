using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models;
using TicketingSystem.Models.Enums;

namespace TicketingSystem.Core
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public DbSet<TicketEntity> TicketEntities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DB");

            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var StatusConversion = new ValueConverter<TicketStatusEnum, int>(
                value => (int)value,
                value => (TicketStatusEnum)value
            );

            modelBuilder.Entity<TicketEntity>()
                .ToTable("tickets")
                .Property(column => column.Status).HasConversion(StatusConversion);

            base.OnModelCreating(modelBuilder);
        }
    }
}
