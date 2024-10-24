﻿using System.ComponentModel.DataAnnotations;
using TicketingSystem.Database.Enums;
using System.Text.Json.Serialization;
using TicketingSystem.Core.Attributes;
using TicketingSystem.Core.Converters;
using TicketingSystem.Database.Entities;
using System.Collections.ObjectModel;

namespace TicketingSystem.Core.Dtos
{
    public class TicketBaseDto
    {
        [Required]
        [MaxLength(255, ErrorMessage = "Ticket title can't be longer than 255 characters")]
        public required string Title { get; set; }

        [MaxLength(2000, ErrorMessage = "Description can't be longer than 2000 characters")]
        public string? Description { get; set; }

        public Guid? Assignee { get; set; }

        public TicketStatusEnum? Status { get; set; }

        [Required]
        public required TicketTypeEnum Type { get; set; }

        [DependentValidation("type", "bug", ErrorMessage = "Affected version can be set only for a bug")]
        public Version? AffectedVersion { get; set; }
    }

    public class TicketCreateDto : TicketBaseDto
    {
        public string[]? Tags { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public Collection<TicketDependencyDto>? Dependencies { get; set; }
    }

    public class TicketSaveDto : TicketBaseDto
    {
        public Collection<TagEntity> Tags { get; set; } = [];
        public Collection<TicketMetadata> Metadata { get; set; } = [];
        public Collection<TicketDependenciesEntity> Dependencies { get; set; } = [];
    }

    public class TicketUpdateBaseDto
    {
        [ValidateOptionalMaxLength<string>(255)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Title { get; set; }

        [ValidateOptionalMaxLength<string>(2000)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<string> Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid> Assignee { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<TicketStatusEnum> Status { get; set; }

        [ValidateOptionalMinLengthArray<Guid>(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Guid[]> RelatedElements { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Optional<Version> AffectedVersion { get; set; }
    }

    public class TicketUpdateDto : TicketUpdateBaseDto
    {
        public string[]? Tags { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
        public Collection<TicketDependencyDto>? Dependencies { get; set; }
    }

    public class TicketUpdateSaveDto : TicketUpdateBaseDto
    {
        public Collection<TagEntity> Tags { get; set; } = [];
        public Collection<TicketMetadata> Metadata { get; set; } = [];
        public Collection<TicketDependenciesEntity> Dependencies { get; set; } = [];
    }
}
