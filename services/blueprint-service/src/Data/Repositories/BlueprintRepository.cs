using BlueprintService.Configuration;
using BlueprintService.Data.Mappers;
using BlueprintService.Data.Queries;
using BlueprintService.Models;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlueprintService.Data.Repositories
{
    public interface IBlueprintRepository
    {
        Task<Blueprint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Blueprint>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
        Task<Blueprint> CreateAsync(Blueprint blueprint, CancellationToken cancellationToken = default);
        Task<Blueprint?> UpdateAsync(Blueprint blueprint, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
    }

    public class BlueprintRepository(DatabaseConfiguration config) : IBlueprintRepository
    {
        private readonly DatabaseConfiguration _config = config;

        private NpgsqlConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_config.ConnectionString);
            return connection;
        }

        public async Task<Blueprint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.GetById");
            activity?.SetTag("blueprint.id", id);

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var entity = await connection.QuerySingleOrDefaultAsync<BlueprintEntity>(
                    BlueprintQueries.GetById,
                    new { Id = id },
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return entity != null ? BlueprintMapper.MapToDomainModel(entity) : null;
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<IEnumerable<Blueprint>> GetAllAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.GetAll");
            activity?.SetTag("limit", limit);
            activity?.SetTag("offset", offset);

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var entities = await connection.QueryAsync<BlueprintEntity>(
                    BlueprintQueries.GetAll,
                    new { Limit = limit, Offset = offset },
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return entities.Select(BlueprintMapper.MapToDomainModel);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<Blueprint> CreateAsync(Blueprint blueprint, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.Create");
            activity?.SetTag("blueprint.id", blueprint.Id);

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var entity = BlueprintMapper.MapToEntity(blueprint);

                var createdEntity = await connection.QuerySingleAsync<BlueprintEntity>(
                    BlueprintQueries.Insert,
                    entity,
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return BlueprintMapper.MapToDomainModel(createdEntity);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<Blueprint?> UpdateAsync(Blueprint blueprint, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.Update");
            activity?.SetTag("blueprint.id", blueprint.Id);

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                var entity = BlueprintMapper.MapToEntity(blueprint);

                var updatedEntity = await connection.QuerySingleOrDefaultAsync<BlueprintEntity>(
                    BlueprintQueries.Update,
                    entity,
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return updatedEntity != null ? BlueprintMapper.MapToDomainModel(updatedEntity) : null;
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.Delete");
            activity?.SetTag("blueprint.id", id);

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                int rowsAffected = await connection.ExecuteAsync(
                    BlueprintQueries.Delete,
                    new { Id = id },
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            using var activity = Telemetry.ActivitySource.Source.StartActivity("Repository.Count");

            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                return await connection.ExecuteScalarAsync<int>(
                    BlueprintQueries.Count,
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                throw;
            }
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                int result = await connection.ExecuteScalarAsync<int>(
                    BlueprintQueries.HealthCheck,
                    commandTimeout: _config.CommandTimeout).ConfigureAwait(false);

                return result == 1;
            }
            catch
            {
                return false;
            }
        }
    }
}