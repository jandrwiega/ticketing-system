using System.Collections.ObjectModel;
using TicketingSystem.Common.Interfaces;
using TicketingSystem.Common.Models.Dtos;
using TicketingSystem.Common.Models.Entities;
using TicketingSystem.Core.Database;
using TicketingSystem.Core.Validators;

namespace TicketingSystem.Services
{
    public class TicketsService(
        IRepository<TicketEntity, TicketSaveDto, TicketUpdateSaveDto> _ticketsDbRepository,
        ITagsRepository _ticketTagsDbRepository,
        IMetadataRepository _ticketMetadataDbRepository,
        ITicketsConfigurationRepository _ticketsConfigurationRepository,
        ITicketsDependenciesRepository _ticketsDependenciesRepository,
        DependeciesValidatorFactory _dependenciesValidatorFactory
        ) : ITicketsService
    {
        public async Task<IEnumerable<TicketEntity>> GetTickets(TicketFiltersDto filters)
        {
             return await _ticketsDbRepository.Get(filters);
        }

        public async Task<TicketEntity> CreateTicket(TicketCreateDto body)
        {
            TicketConfigurationMapEntity? configuration = await _ticketsConfigurationRepository.GetConfigurationForType(body.Type);

            Collection<TagEntity> tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            Collection<TicketMetadata> metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);
            Collection<TicketDependenciesEntity> dependencies = await _ticketsDependenciesRepository.CreateDependecies(body.Dependencies ?? []);

            TicketSaveDto UpdatedBody = new()
            { 
                Title = body.Title,
                Type = body.Type,
                Tags = tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = metadata,
                Dependencies = dependencies
            };

            return await _ticketsDbRepository.Create(UpdatedBody, configuration.Id);
        }

        public async Task<TicketEntity> UpdateTicket(Guid ticketId, TicketUpdateDto body)
        {
            TicketEntity entity = await _ticketsDbRepository.GetById(ticketId);

            if (body.Dependencies?.Count > 0)
            {
                Collection<TicketDependencyDto> dependeciesWithSource = new(body.Dependencies.Select(dependency => {
                    dependency.SourceTicketId = ticketId;

                    return dependency;
                }).ToList());

                Collection<TicketDependenciesEntity> ticketDependencies = await _ticketsDependenciesRepository.CreateDependecies(dependeciesWithSource ?? []);

                foreach (TicketDependenciesEntity dependency in ticketDependencies)
                {
                    IDependencyValidator<TicketUpdateDto> validator = _dependenciesValidatorFactory.GetValidator<TicketUpdateDto>(dependency.DependencyType);

                    try
                    {
                        await validator.CanCreate(ticketId, dependency);
                    }
                    catch
                    {
                        foreach (TicketDependenciesEntity revertDependencies in ticketDependencies)
                        {
                            await _ticketsDependenciesRepository.DeleteDependency(revertDependencies.Id);
                        }
                        throw;
                    }
                }

                foreach (TicketDependenciesEntity oldDependency in entity.Dependencies)
                {
                    await _ticketsDependenciesRepository.DeleteDependency(oldDependency.Id);
                }

                entity.Dependencies = ticketDependencies;
            }

            foreach (TicketDependenciesEntity dependency in entity.Dependencies)
            {
                TicketEntity targetEntity = await _ticketsDbRepository.GetById(dependency.TargetTicketId);
                IDependencyValidator<TicketUpdateDto> validator = _dependenciesValidatorFactory.GetValidator<TicketUpdateDto>(dependency.DependencyType);

                if (validator.ShouldValidate(body))
                {
                    validator.Validate(targetEntity);
                }
            }

            Collection<TagEntity> tags = await _ticketTagsDbRepository.GetOrCreateTags(body.Tags ?? []);
            TicketConfigurationMapEntity configuration = await _ticketsConfigurationRepository.GetConfigurationForType(entity.Type);
            
            Collection<TicketMetadata> metadata = await _ticketMetadataDbRepository.CreateMetadata(body.Metadata ?? [], configuration);

            TicketUpdateSaveDto updatedBody = new()
            {
                Title = body.Title,
                RelatedElements = body.RelatedElements,
                Tags = tags,
                AffectedVersion = body.AffectedVersion,
                Assignee = body.Assignee,
                Description = body.Description,
                Status = body.Status,
                Metadata = metadata
            };

            return await _ticketsDbRepository.Update(entity, updatedBody);
        }
    }
}
