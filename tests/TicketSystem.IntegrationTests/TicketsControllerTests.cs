using Microsoft.AspNetCore.Mvc.Testing;
using TicketingSystem;
using FluentAssertions;
using TicketingSystem.Common.Models;
using TicketingSystem.Common.Enums;
using TicketSystem.IntegrationTests.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;
using TicketingSystem.Core.Converters;
using System.Net;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Core.Database;

namespace TicketSystem.IntegrationTests
{
    public class TicketsControllerTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private HttpClient _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbContextOptions != null)
                {
                    services.Remove(dbContextOptions);
                }

                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TicketsDB"));
            });
        }).CreateClient();
        private readonly string baseUrl = "/v1/tickets";
        private readonly QueryParamsBuilder _queryParamsBuilder = new QueryParamsBuilder();

        #region Tests Helpers
        TicketCreateDto[] testData = [
            new TicketCreateDto() { Title = "Test-Data-Bug", Type = TicketTypeEnum.Bug },
            new TicketCreateDto() { Title = "Test-Data-Improvement", Type = TicketTypeEnum.Improvement },
            new TicketCreateDto() { Title = "Test-Data-Epic", Type = TicketTypeEnum.Epic },
        ];
        private async Task SetupTestData()
        {
            foreach (var dto in testData)
            {
                await _client.PostAsJsonAsync(baseUrl, dto);
            }
        }
        private async Task CleanDatabase()
        {
            await _client.DeleteAsync(baseUrl);
        }
        private async Task<TicketEntity?> GetTicket(TicketFiltersDto dto)
        {
            string filters = _queryParamsBuilder.BuildQueryParams<TicketFiltersDto>(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            return apiResponse?.FirstOrDefault();
        }
        #endregion

        #region Get Requests
        #region Get all tickets
        [Fact]
        public async Task GetTickets_WithoutParameters_RetunsAllTickets()
        {
            await SetupTestData();
            var response = await _client.GetAsync(baseUrl);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            await CleanDatabase();
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
            await SetupTestData();
            string filters = _queryParamsBuilder.BuildQueryParams<TicketFiltersDto>(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            Assert.All<TicketEntity>((apiResponse ?? []), it => Assert.Equal(expectedStatus, it.Status));
            await CleanDatabase();
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
            await SetupTestData();
            string filters = _queryParamsBuilder.BuildQueryParams<TicketFiltersDto>(dto);
            string concatedUrlWithParams = $"{baseUrl}?" + filters;

            var response = await _client.GetAsync(concatedUrlWithParams);

            string content = await response.Content.ReadAsStringAsync();
            TicketEntity[]? apiResponse = JsonSerializer.Deserialize<TicketEntity[]>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            Assert.All<TicketEntity>((apiResponse ?? []), it => Assert.Equal(expectedType, it.Type));
            await CleanDatabase();
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
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            apiResponse.Should().BeEquivalentTo(dto, options => options
                .Including(p => p.Type)
                .Including(p => p.Title)
            );
            await CleanDatabase();
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
                new TicketFiltersDto() { Type = "Epic", Status = "In_Progress" },
                new TicketUpdateDto { Title = new Optional<string>("Updated item"), AffectedVersion = new Optional<Version>(new Version()) },
                HttpStatusCode.BadRequest
            };
        }
        public static IEnumerable<object[]> ExpectBadRequestOnAffectedVersionUpdateOnImprovementUpdate()
        {
            yield return new object[]
            {
                new TicketFiltersDto() { Type = "Improvement", Status = "In_Progress" },
                new TicketUpdateDto { Title = new Optional<string>("Updated item"), AffectedVersion = new Optional<Version>(new Version()) },
                HttpStatusCode.BadRequest
            };
        }
        public static IEnumerable<object[]> ExpectBadRequestOnTitleUpdateToNullValue()
        {
            yield return new object[]
            {
                new TicketFiltersDto() { Type = "Epic", Status = "In_Progress" },
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
                new TicketFiltersDto() { Type = "Bug", Status = "In_Progress" },
                new TicketUpdateDto { AffectedVersion = new Optional<Version>(new Version(1, 9)) }
            };
        }
        public static IEnumerable<object[]> TryUpdateTitle()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug", Status = "In_Progress" },
                new TicketUpdateDto { Title = new Optional<string>("Updated title") }
            };
        }
        public static IEnumerable<object[]> TryUpdateDescription()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug", Status = "In_Progress" },
                new TicketUpdateDto { Description = new Optional<string>("Updated description") }
            };
        }
        public static IEnumerable<object[]> TryUpdateAssignee()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Bug", Status = "In_Progress" },
                new TicketUpdateDto { Assignee = new Optional<Guid>(Guid.NewGuid()) }
            };
        }
        public static IEnumerable<object[]> TryUpdateStatusFromOpenToInProgress()
        {
            yield return new object[] {
                new TicketFiltersDto() { Type = "Improvement", Status = "Open" },
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
            await SetupTestData();
            DateTime beforeUpdateTime = DateTime.UtcNow;
            TicketEntity? ticket = await GetTicket(new TicketFiltersDto() {});
            var putUrl = $"{baseUrl}/{ticket?.Id}";
            TicketUpdateDto body = new TicketUpdateDto { Status = new Optional<TicketStatusEnum>(TicketStatusEnum.Resolved) };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            Assert.True(updatedTicket.ResolvedDate > beforeUpdateTime);
            await CleanDatabase();
        }
        #endregion

        #region Update Tickets - Expect related elements update
        [Fact]
        public async Task UpdateTicket_ForValidParameters_ExpectAddRelatedElement()
        {
            await SetupTestData();
            TicketEntity? oldEntity = await GetTicket(new TicketFiltersDto() { Type = "Epic" });
            TicketEntity? elementToAdd = await GetTicket(new TicketFiltersDto() { Type = "Bug" });

            var putUrl = $"{baseUrl}/{oldEntity?.Id}";
            TicketUpdateDto body = new TicketUpdateDto { RelatedElements = new Optional<Guid[]>([(elementToAdd?.Id ?? new Guid())]) };

            var response = await _client.PutAsJsonAsync(putUrl, body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            TicketEntity? updatedTicket = JsonSerializer.Deserialize<TicketEntity>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) }
                }
            );

            Assert.True(updatedTicket?.RelatedElements?.Length == (oldEntity?.RelatedElements?.Length ?? 0) + 1);
            await CleanDatabase();
        }
        #endregion
        #endregion
    }
}
