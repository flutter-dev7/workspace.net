using System;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IWorkSpaceService
{
    Task<IEnumerable<WorkSpace>> GetAllAsync();
    Task<WorkSpace?> GetByIdAsync(int id);
    Task<IEnumerable<WorkSpace>> GetByRoomAsync(int roomId);
    Task<int> CreateAsync(WorkSpace request);
    Task<bool> UpdateAsync(int id, WorkSpace request);
    Task<bool> DeleteAsync(int id);
}
