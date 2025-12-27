using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.Infrastructure.Services;

public class CommentService : ICommentService
{
    private readonly AppDbContext _db;

    public CommentService(AppDbContext db) => _db = db;

    public async Task<CommentDto?> CreateAsync(Guid taskId, CreateCommentRequest request, CancellationToken ct)
    {
        var taskExists = await _db.TaskItems.AnyAsync(t => t.Id == taskId, ct);
        if (!taskExists) return null;

        var entity = new Comment
        {
            TaskItemId = taskId,
            Content = request.Content.Trim()
        };

        _db.Comments.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<IReadOnlyList<CommentDto>> GetAllAsync(Guid taskId, CancellationToken ct)
    {
        var list = await _db.Comments.AsNoTracking()
            .Where(c => c.TaskItemId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(ct);

        return list.Select(Map).ToList();
    }

    public async Task<bool> DeleteAsync(Guid taskId, Guid commentId, CancellationToken ct)
    {
        var entity = await _db.Comments.SingleOrDefaultAsync(c => c.TaskItemId == taskId && c.Id == commentId, ct);
        if (entity is null) return false;

        _db.Comments.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static CommentDto Map(Comment c) => new()
    {
        Id = c.Id,
        TaskItemId = c.TaskItemId,
        Content = c.Content,
        CreatedAt = c.CreatedAt
    };
}
