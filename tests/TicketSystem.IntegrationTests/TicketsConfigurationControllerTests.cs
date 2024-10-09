using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;
using TicketSystem.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Sockets;

namespace TicketingSystem.IntegrationTests
{
    public class TicketsConfigurationControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly AppDbContext _dbContext;

        private readonly string baseUrl = "/v1/tickets-configuration";
        private readonly QueryParamsBuilder _queryParamsBuilder = new();
        private bool disposedValue;

        public TicketsConfigurationControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (dbContextOptions != null)
                    {
                        services.Remove(dbContextOptions);
                    }

                    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TicketsConfigturationDb"));
                });
            });
            _client = _factory.CreateClient();

            SetupTestData().Wait();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using var scope = _factory.Services.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region Tests Helpers
        private async Task SetupTestData()
        {
            var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //TicketMetadataFieldEntity generateMetadata(string propertyName, TicketMetadataTypeEnum propertyType)
            //{
            //    return new TicketMetadataFieldEntity { Id = Guid.NewGuid(), PropertyName = propertyName, PropertyType = propertyType };
            //}

            //context.TicketConfigurationMapEntities.AddRange(
            //    new TicketConfigurationMapEntity {
            //        Id = Guid.NewGuid(),
            //        Metadata = [generateMetadata("Metadata1", TicketMetadataTypeEnum.String)],
            //        TicketType = TicketTypeEnum.Bug
            //    },
            //    new TicketConfigurationMapEntity {
            //        Id = Guid.NewGuid(),
            //        Metadata = [
            //            generateMetadata("Metadata1", TicketMetadataTypeEnum.String),
            //            generateMetadata("Metadata2", TicketMetadataTypeEnum.String)
            //        ],
            //        TicketType = TicketTypeEnum.Improvement
            //    },
            //    new TicketConfigurationMapEntity {
            //        Id = Guid.NewGuid(),
            //        Metadata = [
            //            generateMetadata("Metadata1", TicketMetadataTypeEnum.String),
            //            generateMetadata("Metadata2", TicketMetadataTypeEnum.String),
            //            generateMetadata("Metadata3", TicketMetadataTypeEnum.String)
            //        ],
            //        TicketType = TicketTypeEnum.Epic
            //    }
            //);

            //await context.SaveChangesAsync();

            TicketConfigurationMapEntity[] configurations = context.TicketConfigurationMapEntities.ToArray();

            Console.WriteLine(configurations);
            if (configurations.Length > 0)
            {
                foreach (TicketConfigurationMapEntity configuration in configurations)
                {
                    context.TicketMetadataFieldEntities.AddRange(
                        new TicketMetadataFieldEntity() { PropertyName = "Metadata", Configurations = [configuration] }
                    );
                }

                await context.SaveChangesAsync();
            }
        }

        #endregion

        #region Get Requests
        #region Get configurations
        //public static IEnumerable<object[]> BugConfigurationTest()
        //{
        //    yield return new object[] { TicketTypeEnum.Bug };
        //}
        //public static IEnumerable<object[]> ImprovementConfigurationTest()
        //{
        //    yield return new object[] { TicketTypeEnum.Improvement };
        //}
        //public static IEnumerable<object[]> EpicConfigurationTest()
        //{
        //    yield return new object[] { TicketTypeEnum.Epic };
        //}

        //[Theory]
        //[MemberData(nameof(BugConfigurationTest))]
        //[MemberData(nameof(ImprovementConfigurationTest))]
        //[MemberData(nameof(EpicConfigurationTest))]
        //public async Task GetConfiguration_ForValidSetup_RetunsConfigurationWithValidMetadataSet(TicketTypeEnum type)
        //{
        //    string url = $"{baseUrl}/{type}";
        //    var response = await _client.GetAsync(url);

        //    string content = await response.Content.ReadAsStringAsync();
        //    TicketConfigurationMapEntity? apiResponse = JsonSerializer.Deserialize<TicketConfigurationMapEntity>(content);

        //    Console.Write(apiResponse);
        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}
        #endregion
        #endregion
    }
}
