using System;
using System.Text.Json;
using BlueprintService.Models;

namespace BlueprintService.Data.Mappers
    {
    public static class BlueprintMapper
        {
        // Map from database entity to domain model
        public static Blueprint MapToDomainModel(BlueprintEntity entity)
            {
            BlueprintMetadata metadata;

            try
                {
                metadata = string.IsNullOrEmpty(entity.Metadata)
                    ? new BlueprintMetadata()
                    : JsonSerializer.Deserialize<BlueprintMetadata>(entity.Metadata) ?? new BlueprintMetadata();
                }
            catch (JsonException ex)
                {
                // Log error and fallback to default
                Console.Error.WriteLine($"Failed to deserialize metadata: {ex.Message}");
                metadata = new BlueprintMetadata();
                }

            return new Blueprint
                {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Metadata = metadata
                };
            }

        // Map from domain model to database entity
        public static BlueprintEntity MapToEntity(Blueprint model)
            {
            string serializedMetadata;

            try
                {
                serializedMetadata = JsonSerializer.Serialize(model.Metadata);
                }
            catch (JsonException ex)
                {
                // Log error and fallback to empty JSON
                Console.Error.WriteLine($"Failed to serialize metadata: {ex.Message}");
                serializedMetadata = "{}";
                }

            return new BlueprintEntity
                {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                Metadata = serializedMetadata
                };
            }

        // Map from domain model to response model
        public static BlueprintResponse MapToResponse(Blueprint model)
            {
            return new BlueprintResponse
                {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt,
                Metadata = model.Metadata
                };
            }

        // Map from request model to domain model
        public static Blueprint MapFromCreateRequest(CreateBlueprintRequest request)
            {
            return new Blueprint
                {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Metadata = request.Metadata
                };
            }

        // Map from update request to domain model
        public static Blueprint MapFromUpdateRequest(UpdateBlueprintRequest request, Blueprint existingBlueprint)
            {
            return existingBlueprint with
                {
                Name = request.Name,
                Description = request.Description,
                UpdatedAt = DateTime.UtcNow,
                Metadata = request.Metadata
                };
            }
        }
    }
