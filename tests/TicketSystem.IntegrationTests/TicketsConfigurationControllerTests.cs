using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using TicketingSystem.Database.Enums;
using TicketingSystem.Database.Entities;
using TicketingSystem.Database;
using TicketingSystem.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using TicketingSystem.IntegrationTests.Data;
using TicketingSystem.Core.Converters;
using TicketingSystem.Core.Dtos;

namespace TicketingSystem.IntegrationTests
{
    public class TicketsConfigurationControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

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
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    SetupTestData(db).Wait();
                });
            });
            _client = _factory.CreateClient();
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
        private static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
            };
        }

        #region Tests Helpers
        readonly TicketTypeEnum[] typeData = [TicketTypeEnum.Bug, TicketTypeEnum.Improvement, TicketTypeEnum.Epic];

        private async Task SetupTestData(AppDbContext _context)
        {
            MetadataConfigurationMocker metadataConfigurationMocker = new(_context);

            foreach (TicketTypeEnum type in typeData)
            {
                TicketMetadataFieldEntity metadata = await metadataConfigurationMocker.GenerateConfigurationMetadataMock(type);
                await metadataConfigurationMocker.GenerateConfigurationMock(new TicketConfigurationMapEntity() { TicketType = type, Metadata = [metadata] });
            }

            await _context.SaveChangesAsync();

            TicketConfigurationMapEntity[] configurations = [.. _context.TicketConfigurationMapEntities];

            if (configurations.Length > 0)
            {
                foreach (TicketConfigurationMapEntity configuration in configurations)
                {
                    _context.TicketMetadataFieldEntities.Add(
                        new TicketMetadataFieldEntity() { PropertyName = $"Metadata-{configuration.TicketType}", Configurations = [configuration] }
                    );
                }

                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Get Requests
        #region Get configurations
        public static IEnumerable<object[]> BugConfigurationTest()
        {
            yield return new object[] { TicketTypeEnum.Bug };
        }
        public static IEnumerable<object[]> ImprovementConfigurationTest()
        {
            yield return new object[] { TicketTypeEnum.Improvement };
        }
        public static IEnumerable<object[]> EpicConfigurationTest()
        {
            yield return new object[] { TicketTypeEnum.Epic };
        }

        [Theory]
        [MemberData(nameof(BugConfigurationTest))]
        [MemberData(nameof(ImprovementConfigurationTest))]
        [MemberData(nameof(EpicConfigurationTest))]
        public async Task GetConfiguration_ForValidSetup_RetunsConfigurationWithValidMetadataSet(TicketTypeEnum type)
        {
            string url = $"{baseUrl}/{type}";
            var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion
        #endregion

        #region Create Requests
        #region Try create existsing metadata
        [Fact]
        public async Task TryCreateMetadata_ForExistingMetadata_ReturnsExistsField()
        {
            TicketTypeEnum type = TicketTypeEnum.Bug;
            string url = $"{baseUrl}/{type}";
            TicketConfigurationDto dto = new() { PropertyName = $"metadata-{type}" };
            var response = await _client.PostAsJsonAsync(url, dto);

            string content = await response.Content.ReadAsStringAsync();
            TicketConfigurationMapEntity? apiResponse = JsonSerializer.Deserialize<TicketConfigurationMapEntity>(content);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(apiResponse?.TicketType == type);
        }
        #endregion

        #region Create new field
        [Fact]
        public async Task TryCreateMetadata_ForNonExistingMetadata_ReturnsValidField()
        {
            TicketTypeEnum type = TicketTypeEnum.Bug;
            string url = $"{baseUrl}/{type}";
            TicketConfigurationDto dto = new() { PropertyName = $"metadata-{type}-2" };
            var response = await _client.PostAsJsonAsync(url, dto);

            string content = await response.Content.ReadAsStringAsync();
            TicketMetadataFieldEntity? apiResponse = JsonSerializer.Deserialize<TicketMetadataFieldEntity>(content, GetOptions());

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            Assert.True(apiResponse?.PropertyName == $"metadata-{type}-2");
        }
        #endregion
        #endregion

        #region Update Requests
        #region Try update not existsing metadata
        [Fact]
        public async Task TryUpdateMetadata_OnNotExistingMetadata_ToThrowException()
        {
            string putUrl = $"{baseUrl}/{Guid.NewGuid()}";
            TicketConfigurationUpdateDto dto = new() { PropertyName = new Optional<string>("Updated-Metadata") };
            var response = await _client.PutAsJsonAsync(putUrl, dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
        #endregion

        #region Try update not existsing metadata
        [Fact]
        public async Task TryUpdateMetadata_OnExistingMetadata_RetunsUpdatedMetadataField()
        {
            TicketTypeEnum type = TicketTypeEnum.Bug;
            string getUrl = $"{baseUrl}/{type}";

            var getResponse = await _client.GetAsync(getUrl);
            string getContent = await getResponse.Content.ReadAsStringAsync();
            TicketConfigurationMapEntity? getApiResponse = JsonSerializer.Deserialize<TicketConfigurationMapEntity>(getContent, GetOptions());

            string putUrl = $"{baseUrl}/{getApiResponse?.Metadata.First().Id}";
            TicketConfigurationUpdateDto dto = new() { PropertyName = new Optional<string>("Updated-Metadata"), PropertyType = new Optional<TicketMetadataTypeEnum>(TicketMetadataTypeEnum.String) };
            var putResponse = await _client.PutAsJsonAsync(putUrl, dto);
            string putContent = await putResponse.Content.ReadAsStringAsync();
            TicketMetadataFieldEntity? putApiResponse = JsonSerializer.Deserialize<TicketMetadataFieldEntity>(putContent, GetOptions());

            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            Assert.True(putApiResponse?.PropertyName == "Updated-Metadata");
        }
        #endregion
        #endregion

        #region Delete Requests
        #region Delete valid metadata
        [Fact]
        public async Task TryDeleteMetadata_OnValidMetadata_MetadataShouldBeDeleted()
        {
            TicketTypeEnum type = TicketTypeEnum.Bug;
            string getUrl = $"{baseUrl}/{type}";

            var getResponse = await _client.GetAsync(getUrl);
            string getContent = await getResponse.Content.ReadAsStringAsync();
            TicketConfigurationMapEntity? getApiResponse = JsonSerializer.Deserialize<TicketConfigurationMapEntity>(getContent, GetOptions());

            string deleteUrl = $"{baseUrl}/{getApiResponse?.Metadata.First().Id}";
            var putResponse = await _client.DeleteAsync(deleteUrl);

            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion
        #endregion
    }
}
