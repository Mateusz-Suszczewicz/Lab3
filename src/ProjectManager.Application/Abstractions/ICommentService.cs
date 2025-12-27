using ProjectManager.Application.Models;

namespace ProjectManager.Application.Abstractions;

public interface ICommentService
{
    Task<CommentDto?> CreateAsync(Guid taskId, CreateCommentRequest request, CancellationToken ct);
    Task<IReadOnlyList<CommentDto>> GetAllAsync(Guid taskId, CancellationToken ct);
    Task<bool> DeleteAsync(Guid taskId, Guid commentId, CancellationToken ct);
}
