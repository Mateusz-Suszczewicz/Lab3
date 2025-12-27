using ProjectManager.Application.Models;

namespace ProjectManager.Application.Abstractions;

public interface IProjectService
{
    Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct);
    Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct);
    Task<ProjectDto?> GetByIdAsync(Guid projectId, CancellationToken ct);
    Task<bool> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid projectId, CancellationToken ct);
}
