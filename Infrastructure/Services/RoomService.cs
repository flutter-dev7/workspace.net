using System;
using Dapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Services;

public class RoomService : IRoomService
{
    private readonly DataContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(DataContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        _logger.LogInformation("Start in GetAllAsync rooms");
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM Rooms";

                var rooms = await connection.QueryAsync<Room>(sql);

                _logger.LogInformation("Successfully get {Count} rooms", rooms.Count());

                return rooms;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync rooms");
            throw;
        }
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Start in GetByIdAsync with RoomId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT * FROM Rooms
                WHERE Id = @id";

                var room = await connection.QuerySingleOrDefaultAsync<Room>(sql, new { id });

                if (room == null)
                {
                    _logger.LogWarning("Room with Id = {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Successfully get RoomId = {Id}", id);
                }

                return room;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync wtih RoomId = {Id}", id);
            throw;
        }
    }

    public async Task<int> CreateAsync(Room request)
    {
        _logger.LogInformation("Start in CreateAsync with Room Name = {Name}", request.Name);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                INSERT INTO Rooms (Name, Capacity, PricePerHour)
                VALUES (@Name, @Capacity, @PricePerHour)";

                var res = await connection.ExecuteAsync(sql, request);

                _logger.LogInformation("Room created successfully with Id = {Id}", request.Id);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync with Room Name = {Name}", request.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Room request)
    {
        _logger.LogInformation("Start in UpdateAsync with RoomId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                UPDATE Rooms SET
                Name = @Name,
                Capacity = @Capacity,
                PricePerHour = @PricePerHour
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    request.Name,
                    request.Capacity,
                    request.PricePerHour
                });

                if(res == 0)
                {
                    _logger.LogWarning("Room with Id = {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Room updated successfully with id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateAsync with RoomId = {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Start in DeleteAsync with RoomId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                DELETE FROM Rooms
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new {id});

                if(res == 0)
                {
                    _logger.LogWarning("Room with Id = {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Room deleted successfully with Id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteAsync with RoomId = {Id}", id);
            throw;
        }
    }
}
