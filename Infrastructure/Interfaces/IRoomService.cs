using System;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IRoomService
{
    Task<IEnumerable<Room>> GetAllAsync();
    Task<Room?> GetByIdAsync(int id);
    Task<int> CreateAsync(Room request);
    Task<bool> UpdateAsync(int id, Room request);
    Task<bool> DeleteAsync(int id);
}
