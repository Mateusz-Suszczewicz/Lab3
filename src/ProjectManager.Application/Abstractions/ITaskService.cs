using ProjectManager.Application.Models;

namespace ProjectManager.Application.Abstractions;

public interface ITaskService
{
    Task<TaskDto?> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct);
    Task<IReadOnlyList<TaskDto>> GetAllAsync(Guid projectId, CancellationToken ct);
    Task<TaskDto?> GetByIdAsync(Guid projectId, Guid taskId, CancellationToken ct);
    Task<bool> UpdateAsync(Guid projectId, Guid taskId, UpdateTaskRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid projectId, Guid taskId, CancellationToken ct);
}
