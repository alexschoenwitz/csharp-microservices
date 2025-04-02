using System;

namespace BlueprintService.Models
{
    // Database entity model
    public record BlueprintEntity
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public string Metadata { get; init; } = string.Empty; // JSON column
    }

    // Domain model
    public record Blueprint
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public BlueprintMetadata Metadata { get; init; } = new BlueprintMetadata();
    }

    // Complex type stored as JSON
    public record BlueprintMetadata
    {
        public string Owner { get; init; } = string.Empty;
        public string Version { get; init; } = "1.0.0";
        public string[] Tags { get; init; } = Array.Empty<string>();
    }

    // Request/response models for the service API
    public record CreateBlueprintRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public BlueprintMetadata Metadata { get; init; } = new BlueprintMetadata();
    }

    public record UpdateBlueprintRequest
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public BlueprintMetadata Metadata { get; init; } = new BlueprintMetadata();
    }

    public record BlueprintResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public BlueprintMetadata Metadata { get; init; } = new BlueprintMetadata();
    }
}
