using System;
using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Context;
using Microsoft.Extensions.Logging;
using Npgsql;
using Dapper;
namespace Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly DataContext _context;
    private readonly ILogger<CompanyService> _logger;

    public CompanyService(DataContext context, ILogger<CompanyService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        _logger.LogInformation("Start GetAllAsync companies");
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"SELECT * FROM Companies";

                var companies = await connection.QueryAsync<Company>(sql);

                _logger.LogInformation("Successfully get {Count} companies", companies.Count());

                return companies;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetAllAsync companies");
            throw;
        }
    }

    public async Task<Company?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Start GetByIdAsync with CompanyId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                SELECT * FROM Companies
                WHERE Id = @id";

                var company = await connection.QuerySingleOrDefaultAsync<Company>(sql, new {id});

                if(company == null)
                {
                    _logger.LogWarning("Company with Id = {Id} not found", id);
                }
                else
                {
                    _logger.LogInformation("Successfully get CompanyId = {Id}", id);
                }

                return company;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in GetByIdAsync with CompanyId = {Id}", id);
            throw;
        }
    }

    public async Task<int> CreateAsync(Company request)
    {
        _logger.LogInformation("Start CreateAsync with Company Name = {Name}", request.Name);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                INSERT INTO Companies (Name, Phone, Email)
                VALUES (@Name, @Phone, @Email)";

                var res = await connection.ExecuteAsync(sql, request);

                _logger.LogInformation("Company created successfully with Id = {Id}", request.Id);

                return res;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in CreateAsync with Company Name = {Name}", request.Name);
            throw;
        }
    }

    public async Task<bool> UpdateAsync(int id, Company request)
    {
        _logger.LogInformation("Start in UpdateAsync CompanyId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                UPDATE Companies SET
                Name = @Name,
                Phone = @Phone,
                Email = @Email
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    request.Name,
                    request.Phone,
                    request.Email
                });

                if(res == 0)
                {
                    _logger.LogWarning("Company not found with Id = {Id}", id);
                }
                else
                {
                    _logger.LogInformation("Company updated successfully with Id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in UpdateAsync with CompanyId = {Id}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Start in DeleteAsync with CompanyId = {Id}", id);
        try
        {
            using (NpgsqlConnection connection = _context.GetConnection())
            {
                connection.Open();

                string sql = @"
                DELETE FROM Companies
                WHERE Id = @id";

                var res = await connection.ExecuteAsync(sql, new {id});

                if(res == 0)
                {
                    _logger.LogWarning("Company not found with Id = {Id}", id);
                }
                else
                {
                    _logger.LogInformation("Company deleted successfully with Id = {Id}", id);
                }

                return res > 0;
            }
        }
        catch (System.Exception ex)
        {
            _logger.LogError(ex, "Error in DeleteAsync with Id = {Id}", id);   
            throw;
        }
    }
}
