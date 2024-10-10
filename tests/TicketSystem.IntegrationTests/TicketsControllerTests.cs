using Microsoft.AspNetCore.Mvc.Testing;
using TicketingSystem;
using FluentAssertions;
using TicketingSystem.Common.Enums;
using TicketSystem.IntegrationTests.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketingSystem.Core.Converters;
using System.Net;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Core.Database;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Common.Models.Dtos;
using System.Collections.ObjectModel;
using TicketingSystem.IntegrationTests.Data;

namespace TicketingSystem.IntegrationTests
{
    public class TicketsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        private readonly string baseUrl = "/v1/tickets";
        private readonly QueryParamsBuilder _queryParamsBuilder = new();
        private bool disposedValue;

        public TicketsControllerTests(WebApplicationFactory<Program> factory)
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

                    services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TicketsDb"));
                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<AppDbContext>();
                        db.Database.EnsureCreated();

                        SetupTestData(db).Wait();
                    }
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

        #region Tests Helpers
        private class SetupData
        {
            public TicketTypeEnum Type { get; set; }
            public TicketStatusEnum Status {  get; set; }
        }

        readonly SetupData[] data = [
            new SetupData() { Type = TicketTypeEnum.Bug, Status = TicketStatusEnum.Open },
            new SetupData() { Type = TicketTypeEnum.Bug, Status = TicketStatusEnum.Resolved },
            new SetupData() { Type = TicketTypeEnum.Bug, Status = TicketStatusEnum.In_Progress },
            new SetupData() { Type = TicketTypeEnum.Improvement, Status = TicketStatusEnum.Open },
            new SetupData() { Type = TicketTypeEnum.Improvement, Status = TicketStatusEnum.Resolved },
            new SetupData() { Type = TicketTypeEnum.Improvement, Status = TicketStatusEnum.In_Progress },
            new SetupData() { Type = TicketTypeEnum.Epic, Status = TicketStatusEnum.Open },
            new SetupData() { Type = TicketTypeEnum.Epic, Status = TicketStatusEnum.Resolved },
            new SetupData() { Type = TicketTypeEnum.Epic, Status = TicketStatusEnum.In_Progress }
        ];
        readonly TicketTypeEnum[] typeData = [TicketTypeEnum.Bug, TicketTypeEnum.Improvement, TicketTypeEnum.Epic];

        private async Task SetupTestData(AppDbContext _context)
        {
            MetadataConfigurationMocker metadataConfigurationMocker = new MetadataConfigurationMocker(_context);
            TicketMocker ticketMocker = new TicketMocker(_context);

            Collection<TicketConfigurationMapEntity> Configurations = [];

            foreach (TicketTypeEnum type in typeData)
            {
                TicketMetadataFieldEntity metadata = await metadataConfigurationMocker.GenerateConfigurationMetadataMock(type);
                TicketConfigurationMapEntity configuration = await metadataConfigurationMocker.GenerateConfigurationMock(new TicketConfigurationMapEntity() { TicketType = type, Metadata = [metadata] });

                Configurations.Add(configuration);
            }

            foreach (SetupData it in data)
            {
                TicketConfigurationMapEntity? MetadataConfiguration = Configurations.Where(config => config.TicketType == it.Type).FirstOrDefault();
                TicketEntity ticket = await ticketMocker.GenerateTicketMock(new TicketEntity() { Title = $"Test-Data-{it.Type}", Type = it.Type, MetadataConfiguration = MetadataConfiguration });
            }
        }

        private static JsonSerializerOptions GetOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
            };
        }

        private async Task<TicketEntity?> GetTicket(TicketFiltersDto dto)
        {
            string filters = _queryParamsBuilder.BuildQueryParams<TicketFiltersDto>(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                GetOptions()
            );

            return apiResponse?.FirstOrDefault();
        }
        #endregion

        #region Get Requests
        #region Get all tickets
        [Fact]
        public async Task GetTickets_WithoutParameters_RetunsAllTickets()
        {
            var response = await _client.GetAsync(baseUrl);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region Get tickets for correct status
        public static IEnumerable<object[]> OpenStatusFilters()
        {
            yield return new object[] { new TicketFiltersDto { Status = "Open" }, TicketStatusEnum.Open };
        }
        public static IEnumerable<object[]> InProgressStatusFilters()
        {
            yield return new object[] { new TicketFiltersDto { Status = "In_Progress" }, TicketStatusEnum.In_Progress };
        }
        public static IEnumerable<object[]> ResolvedStatusFilters()
        {
            yield return new object[] { new TicketFiltersDto { Status = "Resolved" }, TicketStatusEnum.Resolved };
        }

        [Theory]
        [MemberData(nameof(OpenStatusFilters))]
        [MemberData(nameof(InProgressStatusFilters))]
        [MemberData(nameof(ResolvedStatusFilters))]
        public async Task GetTickets_WithStatusFilter_ReturnsAllWithCorrectStatus(TicketFiltersDto dto, TicketStatusEnum expectedStatus)
        {
            string filters = _queryParamsBuilder.BuildQueryParams(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                GetOptions()
            );

            Assert.All(apiResponse ?? [], it => Assert.Equal(expectedStatus, it.Status));
        }
        #endregion

        #region Get tickets with correct type
        public static IEnumerable<object[]> BugTypeFilters()
        {
            yield return new object[] { new TicketFiltersDto { Type = "Bug" }, TicketTypeEnum.Bug };
        }
        public static IEnumerable<object[]> ImprovementFilters()
        {
            yield return new object[] { new TicketFiltersDto { Type = "Improvement" }, TicketTypeEnum.Improvement };
        }
        public static IEnumerable<object[]> EpicFilters()
        {
            yield return new object[] { new TicketFiltersDto { Type = "Epic" }, TicketTypeEnum.Epic };
        }

        [Theory]
        [MemberData(nameof(BugTypeFilters))]
        [MemberData(nameof(ImprovementFilters))]
        [MemberData(nameof(EpicFilters))]
        public async Task GetTickets_WithTypeFilter_ReturnsAllWithCorrectType(TicketFiltersDto dto, TicketTypeEnum expectedType)
        {
            string filters = _queryParamsBuilder.BuildQueryParams(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                GetOptions()
            );

            Assert.All(apiResponse ?? [], it => Assert.Equal(expectedType, it.Type));
        }
        #endregion
        #endregion

        #region Post Requests
        #region Create Valid Tickets
        public static IEnumerable<object[]> CreateBugBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Bug", Type = TicketTypeEnum.Bug },
            };
        }
        public static IEnumerable<object[]> CreateInProgressBugBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Bug", Type = TicketTypeEnum.Bug, Status = TicketStatusEnum.In_Progress },
            };
        }
        public static IEnumerable<object[]> CreateResolvedBugBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Bug", Type = TicketTypeEnum.Bug, Status = TicketStatusEnum.Resolved },
            };
        }
        public static IEnumerable<object[]> CreateImprovementBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Improvement", Type = TicketTypeEnum.Improvement }
            };
        }
        public static IEnumerable<object[]> CreateInProgressImprovementBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Improvement", Type = TicketTypeEnum.Improvement, Status = TicketStatusEnum.In_Progress }
            };
        }
        public static IEnumerable<object[]> CreateResolvedImprovementBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Improvement", Type = TicketTypeEnum.Improvement, Status = TicketStatusEnum.Resolved }
            };
        }
        public static IEnumerable<object[]> CreateEpicBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Epic", Type = TicketTypeEnum.Epic }
            };
        }
        public static IEnumerable<object[]> CreateInProgressEpicBody()
        {
            yield return new object[] {
                new TicketCreateDto { Title = "Integration-Tests-Epic", Type = TicketTypeEnum.Epic, Status = TicketStatusEnum.In_Progress }
            };
        }

        [Theory]
        [MemberData(nameof(CreateBugBody))]
        [MemberData(nameof(CreateInProgressBugBody))]
        [MemberData(nameof(CreateResolvedBugBody))]
        [MemberData(nameof(CreateImprovementBody))]
        [MemberData(nameof(CreateInProgressImprovementBody))]
        [MemberData(nameof(CreateResolvedImprovementBody))]
        [MemberData(nameof(CreateEpicBody))]
        [MemberData(nameof(CreateInProgressEpicBody))]
        public async Task CreateTickets_ForBody_ReturnsValidTicket(TicketCreateDto dto)
        {
            var response = await _client.PostAsJsonAsync(baseUrl, dto);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity? apiResponse = JsonSerializer.Deserialize<TicketEntity>(
                content,
                GetOptions()
            );

            apiResponse.Should().BeEquivalentTo(dto, options => options
                .Including(p => p.Type)
                .Including(p => p.Title)
            );
        }
        #endregion
        #endregion

        #region Put Requests
        #region Update Tickets - Expect Exceptions
        public static IEnumerable<object[]> ExpectNotFoundTicketId()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Epic", Status = "Resolved" },
                new TicketUpdateDto { Title = new Optional<string>("Updated item") },
                HttpStatusCode.InternalServerError
            };
        }
        public static IEnumerable<object[]> ExpectBadRequestOnAffectedVersionUpdateOnEpicUpdate()
        {
            yield return new object[]
            {
                new TicketFiltersDto() { Type = "Epic" },
                new TicketUpdateDto { AffectedVersion = new Optional<Version>(new Version()) },
                HttpStatusCode.BadRequest
            };
        }
        public static IEnumerable<object[]> ExpectBadRequestOnAffectedVersionUpdateOnImprovementUpdate()
        {
            yield return new object[]
            {
                new TicketFiltersDto() { Type = "Improvement" },
                new TicketUpdateDto { AffectedVersion = new Optional<Version>(new Version()) },
                HttpStatusCode.BadRequest
            };
        }
        public static IEnumerable<object[]> ExpectBadRequestOnTitleUpdateToNullValue()
        {
            yield return new object[]
            {
                new TicketFiltersDto() { Type = "Epic" },
                new TicketUpdateDto { Title = new Optional<string>(null) },
                HttpStatusCode.InternalServerError
            };
        }

        [Theory]
        [MemberData(nameof(ExpectNotFoundTicketId))]
        [MemberData(nameof(ExpectBadRequestOnAffectedVersionUpdateOnEpicUpdate))]
        [MemberData(nameof(ExpectBadRequestOnAffectedVersionUpdateOnImprovementUpdate))]
        [MemberData(nameof(ExpectBadRequestOnTitleUpdateToNullValue))]
        public async Task UpdateTicket_ForNotValidParameters_ExpectException(TicketFiltersDto ticketIdDto, TicketUpdateDto dto, HttpStatusCode errorCode)
        {
            TicketEntity? ticket = await GetTicket(ticketIdDto);
            var putUrl = $"{baseUrl}/{ticket?.Id ?? new Guid()}";

            var response = await _client.PutAsJsonAsync(putUrl, dto);
            response.StatusCode.Should().Be(errorCode);
        }
        #endregion

        #region Update Tickets - Expect Success
        public static IEnumerable<object[]> TryUpdateBugAffectedVersion()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug" },
                new TicketUpdateDto { AffectedVersion = new Optional<Version>(new Version(1, 9)) }
            };
        }
        public static IEnumerable<object[]> TryUpdateTitle()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug" },
                new TicketUpdateDto { Title = new Optional<string>("Updated title") }
            };
        }
        public static IEnumerable<object[]> TryUpdateDescription()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug" },
                new TicketUpdateDto { Description = new Optional<string>("Updated description") }
            };
        }
        public static IEnumerable<object[]> TryUpdateAssignee()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug" },
                new TicketUpdateDto { Assignee = new Optional<Guid>(Guid.NewGuid()) }
            };
        }
        public static IEnumerable<object[]> TryUpdateStatusFromOpenToInProgress()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Improvement" },
                new TicketUpdateDto { Status = new Optional<TicketStatusEnum>(TicketStatusEnum.In_Progress) }
            };
        }

        [Theory]
        [MemberData(nameof(TryUpdateBugAffectedVersion))]
        [MemberData(nameof(TryUpdateTitle))]
        [MemberData(nameof(TryUpdateDescription))]
        [MemberData(nameof(TryUpdateAssignee))]
        [MemberData(nameof(TryUpdateStatusFromOpenToInProgress))]
        public async Task UpdateTicket_ForValidParameters_ExpectSuccess(TicketFiltersDto ticketIdDto, TicketUpdateDto dto)
        {
            TicketEntity? ticket = await GetTicket(ticketIdDto);
            var putUrl = $"{baseUrl}/{ticket?.Id}";

            var response = await _client.PutAsJsonAsync(putUrl, dto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        #endregion

        #region Update Tickets - Expect resolve date update
        [Fact]
        public async Task UpdateTicket_ForValidParameters_ExpectSuccessAndRelatedStatusUpdate()
        {
            DateTime beforeUpdateTime = DateTime.UtcNow;
            TicketEntity? ticket = await GetTicket(new TicketFiltersDto() { });
            var putUrl = $"{baseUrl}/{ticket?.Id}";
            TicketUpdateDto body = new() { Status = new Optional<TicketStatusEnum>(TicketStatusEnum.Resolved) };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                GetOptions()
            );

            Assert.True(updatedTicket?.ResolvedDate > beforeUpdateTime);
        }
        #endregion

        #region Update Tickets - Expect related elements update
        [Fact]
        public async Task UpdateTicket_ForValidParameters_ExpectAddRelatedElement()
        {
            TicketEntity? oldEntity = await GetTicket(new TicketFiltersDto() { Type = "Epic" });
            TicketEntity? elementToAdd = await GetTicket(new TicketFiltersDto() { Type = "Bug" });

            var putUrl = $"{baseUrl}/{oldEntity?.Id}";
            TicketUpdateDto body = new() { RelatedElements = new Optional<Guid[]>([(elementToAdd?.Id ?? new Guid())]) };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                GetOptions()
            );

            Assert.True(updatedTicket?.RelatedElements?.Length == (oldEntity?.RelatedElements?.Length ?? 0) + 1);
        }
        #endregion

        #region Update Tickets - Expect add tag
        [Fact]
        public async Task UpdateTicket_ForValidParameters_ExpectAddTag()
        {
            TicketEntity? oldEntity = await GetTicket(new TicketFiltersDto() { Type = "Epic" });

            var putUrl = $"{baseUrl}/{oldEntity?.Id}";
            TicketUpdateDto body = new() { Tags = ["Tag1", "Tag2"] };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                GetOptions()
            );

            Assert.True(updatedTicket?.Tags?.Count == body.Tags.Length);
        }
        #endregion

        #region Update Tickets - Expect handle correct metadata update
        [Fact]
        public async Task UpdateTicket_ForValidParameters_ExpectAddMetadata()
        {
            TicketEntity? oldEntity = await GetTicket(new TicketFiltersDto() { Type = "Epic" });

            var putUrl = $"{baseUrl}/{oldEntity?.Id}";

            Dictionary<string, string> UpdatedMetadata = new Dictionary<string, string>();

            UpdatedMetadata.Add("metadata-epic", "test");

            TicketUpdateDto body = new() { Metadata = UpdatedMetadata };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                GetOptions()
            );

            Assert.True(updatedTicket?.Metadata?.Count == body.Metadata.Count);
        }
        #endregion
        #endregion
    }
}
