using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.Infrastructure.Services;

public class ProjectService : IProjectService
{
    private readonly AppDbContext _db;

    public ProjectService(AppDbContext db) => _db = db;

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request, CancellationToken ct)
    {
        var entity = new Project
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            StartDate = request.StartDate,
            PlannedEndDate = request.PlannedEndDate
        };

        _db.Projects.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<IReadOnlyList<ProjectDto>> GetAllAsync(CancellationToken ct)
    {
        var list = await _db.Projects.AsNoTracking()
            .OrderByDescending(p => p.StartDate)
            .ToListAsync(ct);

        return list.Select(Map).ToList();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid projectId, CancellationToken ct)
    {
        var entity = await _db.Projects.AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == projectId, ct);

        return entity is null ? null : Map(entity);
    }

    public async Task<bool> UpdateAsync(Guid projectId, UpdateProjectRequest request, CancellationToken ct)
    {
        var entity = await _db.Projects.SingleOrDefaultAsync(p => p.Id == projectId, ct);
        if (entity is null) return false;

        entity.Name = request.Name.Trim();
        entity.Description = request.Description?.Trim();
        entity.StartDate = request.StartDate;
        entity.PlannedEndDate = request.PlannedEndDate;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid projectId, CancellationToken ct)
    {
        var entity = await _db.Projects.SingleOrDefaultAsync(p => p.Id == projectId, ct);
        if (entity is null) return false;

        _db.Projects.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static ProjectDto Map(Project p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        StartDate = p.StartDate,
        PlannedEndDate = p.PlannedEndDate
    };
}
