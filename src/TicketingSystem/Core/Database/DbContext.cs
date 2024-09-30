using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models;

namespace TicketingSystem.Core.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TicketEntity> TicketEntities { get; set; }
        public Logger logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/manual-logs-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<TicketEntity>().ToTable("tickets");
                modelBuilder.HasPostgresEnum<TicketTypeEnum>("TicketTypeEnum");
                modelBuilder.HasPostgresEnum<TicketStatusEnum>("TicketStatusEnum");
                base.OnModelCreating(modelBuilder);

                logger.Information("Database model created successfully");
            }
            catch (Exception exception)
            {
                logger.Fatal(exception, "Database model create failed");
            }

        }
    }
}
