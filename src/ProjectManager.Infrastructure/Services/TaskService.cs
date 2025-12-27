using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    public async Task<TaskDto?> CreateAsync(Guid projectId, CreateTaskRequest request, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, ct);
        if (!projectExists) return null;

        var entity = new TaskItem
        {
            ProjectId = projectId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Priority = request.Priority,
            DueDate = request.DueDate
        };

        _db.TaskItems.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<IReadOnlyList<TaskDto>> GetAllAsync(Guid projectId, CancellationToken ct)
    {
        var list = await _db.TaskItems.AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate)
            .ToListAsync(ct);

        return list.Select(Map).ToList();
    }

    public async Task<TaskDto?> GetByIdAsync(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var entity = await _db.TaskItems.AsNoTracking()
            .SingleOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId, ct);

        return entity is null ? null : Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid projectId, Guid taskId, UpdateTaskRequest request, CancellationToken ct)
    {
        var entity = await _db.TaskItems.SingleOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId, ct);
        if (entity is null) return false;

        entity.Title = request.Title.Trim();
        entity.Description = request.Description?.Trim();
        entity.Priority = request.Priority;
        entity.DueDate = request.DueDate;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var entity = await _db.TaskItems.SingleOrDefaultAsync(t => t.ProjectId == projectId && t.Id == taskId, ct);
        if (entity is null) return false;

        _db.TaskItems.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static TaskDto Map(TaskItem t) => new()
    {
        Id = t.Id,
        ProjectId = t.ProjectId,
        Title = t.Title,
        Description = t.Description,
        Priority = t.Priority,
        DueDate = t.DueDate
    };
}
