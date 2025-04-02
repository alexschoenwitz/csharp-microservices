using BlueprintService.Services;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlueprintService.GrpcServices
{
    public class BlueprintGrpcService(IBlueprintService blueprintService, ILogger<BlueprintGrpcService> logger) : BlueprintProto.BlueprintService.BlueprintServiceBase
    {
        private readonly IBlueprintService _blueprintService = blueprintService;
        private readonly ILogger<BlueprintGrpcService> _logger = logger;

        public override async Task<BlueprintProto.GetBlueprintResponse> GetBlueprint(
            BlueprintProto.GetBlueprintRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);
                var blueprint = await _blueprintService.GetByIdAsync(id, context.CancellationToken).ConfigureAwait(false) ?? throw new RpcException(new Status(StatusCode.NotFound, $"Blueprint with ID {request.Id} not found"));
                var response = new BlueprintProto.GetBlueprintResponse
                {
                    Blueprint = new BlueprintProto.Blueprint
                    {
                        Id = blueprint.Id.ToString(),
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                        CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.CreatedAt, DateTimeKind.Utc)),
                        UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.UpdatedAt, DateTimeKind.Utc)),
                        Metadata = new BlueprintProto.BlueprintMetadata
                        {
                            Owner = blueprint.Metadata.Owner,
                            Version = blueprint.Metadata.Version
                        }
                    }
                };

                // Add tags
                foreach (string tag in blueprint.Metadata.Tags)
                {
                    response.Blueprint.Metadata.Tags.Add(tag);
                }

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid blueprint ID format: {request.Id}"));
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blueprint with ID {BlueprintId}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<BlueprintProto.ListBlueprintsResponse> ListBlueprints(
            BlueprintProto.ListBlueprintsRequest request, ServerCallContext context)
        {
            try
            {
                var blueprints = await _blueprintService.GetAllAsync(
                    limit: request.PageSize,
                    offset: (request.PageNumber - 1) * request.PageSize,
                    cancellationToken: context.CancellationToken).ConfigureAwait(false);

                var response = new BlueprintProto.ListBlueprintsResponse();

                foreach (var blueprint in blueprints)
                {
                    var protoBlueprint = new BlueprintProto.Blueprint
                    {
                        Id = blueprint.Id.ToString(),
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                        CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.CreatedAt, DateTimeKind.Utc)),
                        UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.UpdatedAt, DateTimeKind.Utc)),
                        Metadata = new BlueprintProto.BlueprintMetadata
                        {
                            Owner = blueprint.Metadata.Owner,
                            Version = blueprint.Metadata.Version
                        }
                    };

                    // Add tags
                    foreach (string tag in blueprint.Metadata.Tags)
                    {
                        protoBlueprint.Metadata.Tags.Add(tag);
                    }

                    response.Blueprints.Add(protoBlueprint);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing blueprints");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<BlueprintProto.CreateBlueprintResponse> CreateBlueprint(
            BlueprintProto.CreateBlueprintRequest request, ServerCallContext context)
        {
            try
            {
                var createRequest = new Models.CreateBlueprintRequest
                {
                    Name = request.Name,
                    Description = request.Description,
                    Metadata = new Models.BlueprintMetadata
                    {
                        Owner = request.Metadata?.Owner ?? string.Empty,
                        Version = request.Metadata?.Version ?? "1.0.0",
                        Tags = request.Metadata?.Tags.ToArray() ?? Array.Empty<string>()
                    }
                };

                var blueprint = await _blueprintService.CreateAsync(createRequest, context.CancellationToken).ConfigureAwait(false);

                return new BlueprintProto.CreateBlueprintResponse
                {
                    Blueprint = new BlueprintProto.Blueprint
                    {
                        Id = blueprint.Id.ToString(),
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                        CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.CreatedAt, DateTimeKind.Utc)),
                        UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.UpdatedAt, DateTimeKind.Utc)),
                        Metadata = new BlueprintProto.BlueprintMetadata
                        {
                            Owner = blueprint.Metadata.Owner,
                            Version = blueprint.Metadata.Version
                        }
                    }
                };
            }
            catch (ArgumentException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blueprint");
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<BlueprintProto.UpdateBlueprintResponse> UpdateBlueprint(
            BlueprintProto.UpdateBlueprintRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);

                var updateRequest = new Models.UpdateBlueprintRequest
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Metadata = new Models.BlueprintMetadata
                    {
                        Owner = request.Metadata?.Owner ?? string.Empty,
                        Version = request.Metadata?.Version ?? "1.0.0",
                        Tags = request.Metadata?.Tags.ToArray() ?? Array.Empty<string>()
                    }
                };

                var blueprint = await _blueprintService.UpdateAsync(updateRequest, context.CancellationToken).ConfigureAwait(false) ?? throw new RpcException(new Status(StatusCode.NotFound, $"Blueprint with ID {request.Id} not found"));
                var response = new BlueprintProto.UpdateBlueprintResponse
                {
                    Blueprint = new BlueprintProto.Blueprint
                    {
                        Id = blueprint.Id.ToString(),
                        Name = blueprint.Name,
                        Description = blueprint.Description,
                        CreatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.CreatedAt, DateTimeKind.Utc)),
                        UpdatedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(blueprint.UpdatedAt, DateTimeKind.Utc)),
                        Metadata = new BlueprintProto.BlueprintMetadata
                        {
                            Owner = blueprint.Metadata.Owner,
                            Version = blueprint.Metadata.Version
                        }
                    }
                };

                // Add tags
                foreach (string tag in blueprint.Metadata.Tags)
                {
                    response.Blueprint.Metadata.Tags.Add(tag);
                }

                return response;
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid blueprint ID format: {request.Id}"));
            }
            catch (ArgumentException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blueprint with ID {BlueprintId}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }

        public override async Task<BlueprintProto.DeleteBlueprintResponse> DeleteBlueprint(
            BlueprintProto.DeleteBlueprintRequest request, ServerCallContext context)
        {
            try
            {
                var id = Guid.Parse(request.Id);
                bool success = await _blueprintService.DeleteAsync(id, context.CancellationToken).ConfigureAwait(false);

                return !success
                    ? throw new RpcException(new Status(StatusCode.NotFound, $"Blueprint with ID {request.Id} not found"))
                    : new BlueprintProto.DeleteBlueprintResponse();
            }
            catch (FormatException)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid blueprint ID format: {request.Id}"));
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blueprint with ID {BlueprintId}", request.Id);
                throw new RpcException(new Status(StatusCode.Internal, "An internal error occurred"));
            }
        }
    }
}