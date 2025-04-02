using BlueprintService.Data.Mappers;
using BlueprintService.Data.Repositories;
using BlueprintService.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlueprintService.Services
{
    public interface IBlueprintService
    {
        Task<BlueprintResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<BlueprintResponse>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
        Task<BlueprintResponse> CreateAsync(CreateBlueprintRequest request, CancellationToken cancellationToken = default);
        Task<BlueprintResponse?> UpdateAsync(UpdateBlueprintRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
    }

    public class BlueprintService(IBlueprintRepository repository) : IBlueprintService
    {
        private readonly IBlueprintRepository _repository = repository;

        public async Task<BlueprintResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.GetById");
            activity?.SetTag("blueprint.id", id);

            var blueprint = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            return blueprint != null ? BlueprintMapper.MapToResponse(blueprint) : null;
        }

        public async Task<IEnumerable<BlueprintResponse>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.GetAll");
            activity?.SetTag("limit", limit);
            activity?.SetTag("offset", offset);

            var blueprints = await _repository.GetAllAsync(limit, offset, cancellationToken).ConfigureAwait(false);

            var responses = new List<BlueprintResponse>();
            foreach (var blueprint in blueprints)
            {
                responses.Add(BlueprintMapper.MapToResponse(blueprint));
            }

            return responses;
        }

        public async Task<BlueprintResponse> CreateAsync(CreateBlueprintRequest request, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.Create");

            // Validate the request
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            var blueprint = BlueprintMapper.MapFromCreateRequest(request);
            activity?.SetTag("blueprint.id", blueprint.Id);

            var created = await _repository.CreateAsync(blueprint, cancellationToken).ConfigureAwait(false);
            return BlueprintMapper.MapToResponse(created);
        }

        public async Task<BlueprintResponse?> UpdateAsync(UpdateBlueprintRequest request, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.Update");
            activity?.SetTag("blueprint.id", request.Id);

            // Validate the request
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new ArgumentException("Name cannot be empty");
            }

            // Get the existing blueprint
            var existingBlueprint = await _repository.GetByIdAsync(request.Id, cancellationToken).ConfigureAwait(false);
            if (existingBlueprint == null)
            {
                return null;
            }

            // Map and update
            var updated = BlueprintMapper.MapFromUpdateRequest(request, existingBlueprint);
            var result = await _repository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);

            return result != null ? BlueprintMapper.MapToResponse(result) : null;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.Delete");
            activity?.SetTag("blueprint.id", id);

            return await _repository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Service.Count");

            return await _repository.CountAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}