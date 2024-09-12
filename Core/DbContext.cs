using Microsoft.EntityFrameworkCore;
using TicketingSystem.Common.Models;

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

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
