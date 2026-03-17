using System;
using Dapper;
using Domain.DTOs;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly DataContext _context;
    private readonly ILogger<CompanyService> _logger;

    public BookingService(DataContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Booking>> GetCompanyBookingsAsync(int companyId)
    {
        _logger.LogInformation("Start GetCompanyBookingsAsync with CompanyId = {Id}", companyId);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string companySql = @"
                SELECT 1 FROM Companies
                WHERE Id = @CompanyId";

                var companyExists = await connection.ExecuteScalarAsync<int>(companySql, new {companyId});

                if(companyExists == 0)
                {
                    throw new Exception($"Company with Id = {companyId} not found");
                }

                string sql = @"
                SELECT * FROM Bookings
                WHERE CompanyId = @companyId";

                var res = await connection.QueryAsync<Booking>(sql, new { companyId });

                _logger.LogInformation("Successfully get {Count} bookings with CompanyId = {Id}", res.Count(), companyId);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetCompanyBookingsAsync with CompanyId = {Id}", companyId);
            throw;
        }
    }

    public async Task<BookingStatisticsDto?> GetBookingStatisticsAsync()
    {
        _logger.LogInformation("Start in GetBookingStatisticsAsync");
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT COUNT(*) AS BookingCount,
                SUM(TotalPrice) AS TotalPrice,
                COUNT(DISTINCT CompanyId) AS CompanyCount
                FROM Bookings";

                var res = await connection.QuerySingleOrDefaultAsync<BookingStatisticsDto>(sql);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetBookingStatisticsAsync");
            throw;
        }
    }

    public async Task<int> CreateBookingAsync(CreateBookingRequest request)
    {
        _logger.LogInformation("Start CreateBookingAsync with WorkSpaceId = {Id}", request.WorkSpaceId);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                if (request.StartTime >= request.EndTime)
                {
                    throw new Exception("StartTime must be less than EndTime");
                }

                if(request.StartTime <= DateTime.Now.TimeOfDay)
                {
                    throw new Exception("StartTime must be less than Now");
                }

                string companySql = @"
                SELECT 1 FROM Companies
                WHERE Id = @CompanyId";

                var companyExists = await connection.ExecuteScalarAsync<int>(companySql, new {request.CompanyId});

                if(companyExists == 0)
                {
                    throw new Exception($"Company with Id = {request.CompanyId} not found");
                } 

                string workSpaceSql = @"
                SELECT 1 FROM WorkSpaces
                WHERE Id = @WorkSpaceId";

                var workSpaceExists = await connection.ExecuteScalarAsync<int>(workSpaceSql, new {request.WorkSpaceId});

                if(workSpaceExists == 0)
                {
                    throw new Exception($"Work Space with Id = {request.WorkSpaceId} not found");
                }

                string priceSql = @"
                SELECT r.PricePerHour FROM Rooms AS r
                JOIN WorkSpaces w ON w.RoomId = r.Id
                WHERE w.Id = @WorkSpaceId";

                var res1 = await connection.ExecuteScalarAsync<decimal>(priceSql, new { request.WorkSpaceId });

                var hours = (request.EndTime - request.StartTime).TotalHours;
                var totalPrice = (decimal)hours * res1;

                string sql = @"
                INSERT INTO Bookings (CompanyId, WorkSpaceId, BookingDate, StartTime, EndTime, TotalPrice, Status)
                VALUES (@CompanyId, @WorkSpaceId, @BookingDate, @StartTime, @EndTime, @TotalPrice, @Status)";

                var res = await connection.ExecuteAsync(sql, new
                {
                    request.CompanyId,
                    request.WorkSpaceId,
                    request.BookingDate,
                    request.StartTime,
                    request.EndTime,
                    TotalPrice = totalPrice,
                    Status = "pending"
                });

                _logger.LogInformation("Booking created successfully");

                return res;

            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in CreateBookingAsync with WorkSpaceId = {Id}", request.WorkSpaceId);
            throw;
        }
    }

    public async Task<bool> UpdateBookingStatusAsync(int id, string status)
    {
        _logger.LogInformation("Updating booking status Id = {Id} to {Status}", id, status);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                

                string sql = @"
                UPDATE Bookings SET
                Status = @Status
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new { id, status });

                if (res == 0)
                {
                    _logger.LogWarning("Booking not found with Id = {Id} to {Status}", id, status);
                }
                else
                {
                    _logger.LogInformation("Booking updated successfully with Id = {Id} to {Status}", id, status);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateBookingStatusAsync with Id = {Id} to {Status}", id, status);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByDateAsync(DateTime date)
    {
        _logger.LogInformation("Getting bookings for Date = {Date}", date);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT * FROM Bookings
                WHERE BookingDate = @date";

                var res = await connection.QueryAsync<Booking>(sql, new {date});

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetBookingsByDateAsync for Date = {Date}", date);
            throw;
        }
    }
}
