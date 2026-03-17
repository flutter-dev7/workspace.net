using System;
using Dapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Services;

public class WorkSpaceService : IWorkSpaceService
{
    private readonly DataContext _context;
    private readonly ILogger<RoomService> _logger;

    public WorkSpaceService(DataContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<WorkSpace>> GetAllAsync()
    {
        _logger.LogInformation("Start GetAllAsync workSpaces");
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM WorkSpaces";

                var workSpaces = await connection.QueryAsync<WorkSpace>(sql);

                _logger.LogInformation("Successfully get {Count} workSpaces", workSpaces.Count());

                return workSpaces;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync workSpaces");
            throw;
        }
    }

    public async Task<WorkSpace?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Start GetByIdAsync with WorkSpaceId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT * FROM WorkSpaces
                WHERE Id = @id";

                var workSpace = await connection.QuerySingleOrDefaultAsync<WorkSpace>(sql, new { id });

                if (workSpace == null)
                {
                    _logger.LogWarning("WorkSpace with Id = {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Successfully get WorkSpaceId = {Id}", id);
                }

                return workSpace;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync with WorkSpaceId = {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<WorkSpace>> GetByRoomAsync(int roomId)
    {
        _logger.LogInformation("Start in GetByRoomAsync workSpaces with RoomId = {Id}", roomId);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT * FROM WorkSpaces
                WHERE RoomId = @roomId";

                var res = await connection.QueryAsync<WorkSpace>(sql, new { roomId });

                _logger.LogInformation("Successfully get {Count} work spaces with RoomId = {Id}", res.Count(), roomId);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetByRoomAsync workSpaces with RoomId = {Id}", roomId);
            throw;
        }
    }

    public async Task<int> CreateAsync(WorkSpace request)
    {
        _logger.LogInformation("Start CreateAsync with WorkSpace Name = {Name}", request.Name);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string roomSql = @"
                SELECT 1 FROM Rooms
                WHERE Id = @RoomId";

                if(string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new Exception("Name cannot be Empty");
                }

                if(string.IsNullOrWhiteSpace(request.Type))
                {
                    throw new Exception("Type cannot be Empty");
                }

                var roomExists = await connection.ExecuteScalarAsync<int>(roomSql, new {request.RoomId});

                if(roomExists == 0)
                {
                    throw new Exception($"Room with Id = {request.RoomId} not found");
                }

                string sql = @"
                INSERT INTO WorkSpaces (RoomId, Name, Type)
                VALUES (@RoomId, @Name, @Type)";

                var res = await connection.ExecuteAsync(sql, request);

                _logger.LogInformation("WorkSpace created successfully with Id = {Id}", request.Id);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync with WorkSpace Name = {Name}", request.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, WorkSpace request)
    {
        _logger.LogInformation("Start in UpdateAsync with WorkSpaceId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string roomSql = @"
                SELECT 1 FROM Rooms
                WHERE Id = @RoomId";

                if(string.IsNullOrWhiteSpace(request.Name))
                {
                    throw new Exception("Name cannot be Empty");
                }

                if(string.IsNullOrWhiteSpace(request.Type))
                {
                    throw new Exception("Type cannot be Empty");
                }

                var roomExists = await connection.ExecuteScalarAsync<int>(roomSql, new {request.RoomId});

                if(roomExists == 0)
                {
                    throw new Exception($"Room with Id = {request.RoomId} not found");
                }

                string sql = @"
                UPDATE WorkSpaces SET
                RoomId = @RoomId,
                Name = @Name,
                Type = @Type
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    request.RoomId,
                    request.Name,
                    request.Type
                });

                if (res == 0)
                {
                    _logger.LogWarning("WorkSpace not found with Id = {Id}", id);
                }
                else
                {
                    _logger.LogInformation("WorkSpace updated successfully with Id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateAsync with WorkSpaceId = {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Start in DeleteAsync with WorkSpaceId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                DELETE FROM WorkSpaces
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new { id });

                if (res == 0)
                {
                    _logger.LogWarning("WorkSpace not found with Id = {Id}", id);
                }
                else
                {
                    _logger.LogInformation("WorkSpace deleted successfully with Id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteAsync with WorkSpaceId = {Id}", id);
            throw;
        }
    }
}
