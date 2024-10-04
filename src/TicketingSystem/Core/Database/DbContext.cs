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
        public DbSet<TicketMetadataEntity> TicketMetadataEntities { get; set; }
        public DbSet<TicketConfigurationMapEntity> TicketConfigurationMapEntities { get; set; }
        public DbSet<TicketMetadataFieldEntity> TicketMetadataFieldEntities { get; set; }

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
                modelBuilder.Entity<TicketMetadataEntity>().ToTable("tickets_metadata");
                modelBuilder.Entity<TicketConfigurationMapEntity>().ToTable("tickets_configuration");
                modelBuilder.Entity<TicketMetadataFieldEntity>().ToTable("tickets_configuration_fields");

                modelBuilder.HasPostgresEnum<TicketTypeEnum>("TicketTypeEnum");
                modelBuilder.HasPostgresEnum<TicketStatusEnum>("TicketStatusEnum");
                modelBuilder.HasPostgresEnum<TicketMetadataTypeEnum>("TicketMetadataTypeEnum");

                modelBuilder.Entity<TicketEntity>()
                    .HasMany(s => s.Tags)
                    .WithMany(c => c.Tickets)
                    .UsingEntity<Dictionary<string, object>>(
                        "TagEntityTicketEntity",
                        j => j.HasOne<TagEntity>().WithMany().HasForeignKey("TagId"),
                        j => j.HasOne<TicketEntity>().WithMany().HasForeignKey("TicketId")
                    );

                modelBuilder.Entity<TicketEntity>()
                    .HasOne(t => t.MetadataConfiguration)
                    .WithMany(mc => mc.Tickets)
                    .HasForeignKey(t => t.ConfigurationId);

                modelBuilder.Entity<TicketMetadataEntity>()
                    .HasOne(m => m.TicketEntity)
                    .WithMany(t => t.Metadata)
                    .HasForeignKey(m => m.TicketId)
                    .IsRequired(false);

                modelBuilder.Entity<TicketConfigurationMapEntity>()
                    .HasMany(c => c.Metadata)
                    .WithMany(m => m.Configurations)
                    .UsingEntity<Dictionary<string, object>>(
                        "ConfigurationMapFieldsRelation",
                        j => j.HasOne<TicketMetadataFieldEntity>().WithMany().HasForeignKey("MetadataId"),
                        j => j.HasOne<TicketConfigurationMapEntity>().WithMany().HasForeignKey("ConfigurationId")
                    );

                modelBuilder.Entity<TicketConfigurationMapEntity>().HasData(
                    new TicketConfigurationMapEntity() { TicketType = TicketTypeEnum.Bug },
                    new TicketConfigurationMapEntity() { TicketType = TicketTypeEnum.Improvement },
                    new TicketConfigurationMapEntity() { TicketType = TicketTypeEnum.Epic }
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
