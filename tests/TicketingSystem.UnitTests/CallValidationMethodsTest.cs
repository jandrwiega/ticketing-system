using Moq;
using TicketingSystem.Database.Enums;
using TicketingSystem.Core.Interfaces;
using TicketingSystem.Database.Entities;
using TicketingSystem.Core.Converters;
using TicketingSystem.Core.Validators;
using TicketingSystem.Services;
using TicketingSystem.Core.Dtos;

namespace TicketingSystem.UnitTests
{
    public class CallValidationMethodsTest
    {
        #region Services - mocks
        private readonly Mock<ITicketsDependenciesRepository> _ticketsDependenciesRepositoryMock = new();
        private readonly Mock<ITicketsConfigurationRepository> _ticketsConfigurationRepositoryMock = new();

        private readonly Mock<IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto>> _ticketsDbRepositoryMock = new();
        private readonly Mock<ITagsRepository> _ticketsTagsDbRepositoryMock = new();
        private readonly Mock<IMetadataRepository> _ticketsMetadataDbRepositoryMock = new();

        private readonly Mock<IDependenciesValidationFactory> _ticketDependenciesFactoryMock = new();
        private readonly Mock<IDependencyValidator<TicketUpdateDto>> _startFinishResolvedTicketMock = new();
        #endregion

        #region Setup Services
        private readonly ITicketsService _ticketsService;
        #endregion

        public CallValidationMethodsTest()
        {
            #region Setup Services
            _ticketDependenciesFactoryMock.Setup(repo => repo.GetValidator<TicketUpdateDto>(It.IsAny<TicketDependenciesEnum>())).Returns(_startFinishResolvedTicketMock.Object);
            _startFinishResolvedTicketMock.Setup(repo => repo.ShouldValidate(It.IsAny<TicketUpdateDto>())).Returns(true);

            _ticketsService = new TicketsService(_ticketsDbRepositoryMock.Object, _ticketsTagsDbRepositoryMock.Object, _ticketsMetadataDbRepositoryMock.Object, _ticketsConfigurationRepositoryMock.Object, _ticketsDependenciesRepositoryMock.Object, _ticketDependenciesFactoryMock.Object);
            #endregion
        }

        static readonly Guid baseTicketId = new("4eadb3ba-e83f-45ec-be3e-893cd4e6e849");
        static readonly Guid ticket1Id = new("deb2f777-5379-4d98-b6f3-43af8d1369e6");

        [Fact]
        public async Task CheckValidation_ForElements_ShouldExecuteValidationAndCallUpdate()
        {
            _startFinishResolvedTicketMock.Setup(repo => repo.Validate(It.IsAny<TicketEntity>()));

            TicketConfigurationMapEntity bugConfiguration = new() { Id = Guid.NewGuid(), TicketType = TicketTypeEnum.Bug, Tickets = [], Metadata = [] };
            TicketUpdateDto dto = new() { Status = new Optional<TicketStatusEnum>(TicketStatusEnum.Resolved) };

            TicketEntity ticket = new()
            {
                Id = baseTicketId,
                MetadataConfiguration = bugConfiguration,
                Title = "Base Ticket",
                Dependencies = [
                    new TicketDependenciesEntity { DependencyType = TicketDependenciesEnum.SF_RESOLVED, SourceTicketId = baseTicketId, TargetTicketId = ticket1Id }
                ]
            };
            TicketEntity ticket1 = new()
            {
                Id = ticket1Id,
                MetadataConfiguration = bugConfiguration,
                Title = "Ticket 1",
                Status = TicketStatusEnum.Resolved,
                Dependencies = []
            };

            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket.Id)).ReturnsAsync(ticket);
            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket1Id)).ReturnsAsync(ticket1);

            await _ticketsService.UpdateTicket(ticket.Id, dto);

            _startFinishResolvedTicketMock.Verify(it => it.ShouldValidate(dto), Times.Once());
            _startFinishResolvedTicketMock.Verify(it => it.Validate(It.IsAny<TicketEntity>()), Times.Once());
            _ticketsDbRepositoryMock.Verify(it => it.Update(ticket, It.IsAny<TicketUpdateSaveDto>()), Times.Once());
        }

        [Fact]
        public async Task CheckValidation_ForElements_ShouldSkipValidationAndCallUpdate()
        {
            _startFinishResolvedTicketMock.Setup(repo => repo.ShouldValidate(It.IsAny<TicketUpdateDto>())).Returns(false);

            TicketConfigurationMapEntity bugConfiguration = new() { Id = Guid.NewGuid(), TicketType = TicketTypeEnum.Bug, Tickets = [], Metadata = [] };
            TicketUpdateDto dto = new() { Description = new Optional<string>("Test description") };

            TicketEntity ticket = new()
            {
                Id = baseTicketId,
                MetadataConfiguration = bugConfiguration,
                Title = "Base Ticket",
                Dependencies = [
                    new TicketDependenciesEntity { DependencyType = TicketDependenciesEnum.SF_RESOLVED, SourceTicketId = baseTicketId, TargetTicketId = ticket1Id }
                ]
            };
            TicketEntity ticket1 = new()
            {
                Id = ticket1Id,
                MetadataConfiguration = bugConfiguration,
                Title = "Ticket 1",
                Dependencies = []
            };

            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket.Id)).ReturnsAsync(ticket);
            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket1Id)).ReturnsAsync(ticket1);

            await _ticketsService.UpdateTicket(ticket.Id, dto);

            _startFinishResolvedTicketMock.Verify(it => it.ShouldValidate(dto), Times.Once());
            _startFinishResolvedTicketMock.Verify(it => it.Validate(It.IsAny<TicketEntity>()), Times.Never());
            _ticketsDbRepositoryMock.Verify(it => it.Update(ticket, It.IsAny<TicketUpdateSaveDto>()), Times.Once());
        }

        [Fact]
        public async Task CheckValidation_ForElements_ShouldExecuteValidationAndFail()
        {
            _startFinishResolvedTicketMock.Setup(repo => repo.Validate(It.IsAny<TicketEntity>())).Throws(new InvalidOperationException("Some of dependencies conditions doesn't meet"));

            TicketConfigurationMapEntity bugConfiguration = new() { Id = Guid.NewGuid(), TicketType = TicketTypeEnum.Bug, Tickets = [], Metadata = [] };
            TicketUpdateDto dto = new() { Status = new Optional<TicketStatusEnum>(TicketStatusEnum.Resolved) };

            TicketEntity ticket = new()
            {
                Id = baseTicketId,
                MetadataConfiguration = bugConfiguration,
                Title = "Base Ticket",
                Dependencies = [
                    new TicketDependenciesEntity { DependencyType = TicketDependenciesEnum.SF_RESOLVED, SourceTicketId = baseTicketId, TargetTicketId = ticket1Id }
                ]
            };
            TicketEntity ticket1 = new()
            {
                Id = ticket1Id,
                MetadataConfiguration = bugConfiguration,
                Title = "Ticket 1",
                Dependencies = []
            };

            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket.Id)).ReturnsAsync(ticket);
            _ticketsDbRepositoryMock.Setup(repo => repo.GetById(ticket1Id)).ReturnsAsync(ticket1);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _ticketsService.UpdateTicket(ticket.Id, dto));
            _startFinishResolvedTicketMock.Verify(it => it.Validate(It.IsAny<TicketEntity>()), Times.Once());
            _startFinishResolvedTicketMock.Verify(it => it.ShouldValidate(dto), Times.Once());
        }
    }
}
