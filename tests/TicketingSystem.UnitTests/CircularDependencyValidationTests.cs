﻿using Moq;
using System.Collections.ObjectModel;
using TicketingSystem.Common.Enums;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Validators.DependencyValidators;

namespace TicketingSystem.UnitTests
{
    public class CircularDependencyValidationTests
    {
        private readonly Mock<ITicketsDependenciesRepository> _ticketsDependenciesRepository;
        private readonly StartFinishResolvedTicket _startFinishResolvedTicket;

        public CircularDependencyValidationTests()
        {
            _ticketsDependenciesRepository = new Mock<ITicketsDependenciesRepository>();
            _startFinishResolvedTicket = new StartFinishResolvedTicket(_ticketsDependenciesRepository.Object);
        }

        class MockDependencyOptions
        {
            public Guid SourceId;
            public Guid TargetId;
        }

        private static Collection<TicketDependenciesEntity> GenerateMockedDependencies()
        {
            return [];
        }

        private static Collection<TicketDependenciesEntity> GenerateMockedDependencies(MockDependencyOptions[] options)
        {
            Collection<TicketDependenciesEntity> dependencies = [];

            foreach (MockDependencyOptions option in options)
            {
                dependencies.Add(
                    new TicketDependenciesEntity { DependencyType = TicketDependenciesEnum.SF_RESOLVED, SourceTicketId = option.SourceId, TargetTicketId = option.TargetId }
                );
            }

            return dependencies;
        }

        static readonly Guid baseTicketId = new("4eadb3ba-e83f-45ec-be3e-893cd4e6e849");
        static readonly Guid ticket1Id = new("deb2f777-5379-4d98-b6f3-43af8d1369e6");
        static readonly Guid ticket2Id = new("c7a9c3c3-5f6e-4f3e-ac02-0b2b574cb5b9");
        static readonly Guid ticket3Id = new("02895efa-cb50-4834-88c8-813e1f2fa720");
        static readonly Guid ticket4Id = new("2e3e88f4-8620-490b-8bc5-bdd0e2326df9");
        static readonly Guid ticket5Id = new("8a2b7541-f094-48fb-93e9-ae7bbda2b1f9");
        static readonly TicketDependenciesEntity baseTicketDependency = new() { DependencyType = TicketDependenciesEnum.SF_RESOLVED, SourceTicketId = baseTicketId, TargetTicketId = ticket1Id };

        [Fact]
        public async Task CheckValidation_For2Elements_ShouldThrowCircularDependenciesException()
        {
            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket1Id)))
                .ReturnsAsync(GenerateMockedDependencies([
                    new MockDependencyOptions { SourceId = ticket1Id, TargetId = baseTicketId }
                ]));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _startFinishResolvedTicket.CanCreateAsync(baseTicketId, baseTicketDependency));
        }

        [Fact]
        public async Task CheckValidation_For3Elements_ShouldThrowCircularDependenciesException()
        {
            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket1Id)))
                .ReturnsAsync(GenerateMockedDependencies([
                    new MockDependencyOptions { SourceId = ticket1Id, TargetId = ticket2Id }
                ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket2Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket2Id, TargetId = baseTicketId }
               ]));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _startFinishResolvedTicket.CanCreateAsync(baseTicketId, baseTicketDependency));
        }

        [Fact]
        public async Task CheckValidation_For4Elements_ShouldThrowCircularDependenciesException()
        {
            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket1Id)))
                .ReturnsAsync(GenerateMockedDependencies([
                    new MockDependencyOptions { SourceId = ticket1Id, TargetId = ticket2Id }
                ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket2Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket2Id, TargetId = ticket3Id }
               ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket2Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket3Id, TargetId = baseTicketId }
               ]));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _startFinishResolvedTicket.CanCreateAsync(baseTicketId, baseTicketDependency));
        }

        [Fact]
        public async Task CheckValidation_ForNestedElements_ShouldThrowCircularDependenciesException()
        {
            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket1Id)))
                .ReturnsAsync(GenerateMockedDependencies([
                    new MockDependencyOptions { SourceId = ticket1Id, TargetId = ticket2Id },
                    new MockDependencyOptions { SourceId = ticket1Id, TargetId = ticket3Id }
                ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket2Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket2Id, TargetId = ticket4Id }
               ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket4Id)))
               .ReturnsAsync(GenerateMockedDependencies());

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket3Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket3Id, TargetId = ticket5Id }
               ]));

            _ticketsDependenciesRepository.Setup(repo => repo.GetDependencies(It.Is<GetTicketDependencyDto>(dto => dto.DependencyType == TicketDependenciesEnum.SF_RESOLVED && dto.SourceTicketId == ticket5Id)))
               .ReturnsAsync(GenerateMockedDependencies([
                   new MockDependencyOptions { SourceId = ticket5Id, TargetId = baseTicketId }
               ]));

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _startFinishResolvedTicket.CanCreateAsync(baseTicketId, baseTicketDependency));
        }
    }
}
