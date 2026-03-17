using System;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface ICompanyService
{
    Task<IEnumerable<Company>> GetAllAsync();
    Task<Company?> GetByIdAsync(int id);
    Task<int> CreateAsync(Company request);
    Task<bool> UpdateAsync(int id, Company request);
    Task<bool> DeleteAsync(int id);
}
