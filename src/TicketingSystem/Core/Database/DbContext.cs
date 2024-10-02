using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Entities;

namespace TicketingSystem.Core.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<TicketEntity> TicketEntities { get; set; }
        public DbSet<TagEntity> TagEntities { get; set; }

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
                modelBuilder.Entity<TagEntity>().ToTable("tags");
                
                modelBuilder.HasPostgresEnum<TicketTypeEnum>("TicketTypeEnum");
                modelBuilder.HasPostgresEnum<TicketStatusEnum>("TicketStatusEnum");

                modelBuilder.Entity<TicketEntity>()
                    .HasMany(s => s.Tags)
                    .WithMany(c => c.Tickets)
                    .UsingEntity<Dictionary<string, object>>(
                        "TagEntityTicketEntity",
                        j => j.HasOne<TagEntity>().WithMany().HasForeignKey("TagId"),
                        j => j.HasOne<TicketEntity>().WithMany().HasForeignKey("TicketId")
                    );

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
