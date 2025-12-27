using ProjectManager.Application.Models;

namespace ProjectManager.Application.Abstractions;

public interface IMemberService
{
    Task<MemberDto> CreateMemberAsync(CreateMemberRequest request, CancellationToken ct);
    Task<bool> AddToProjectAsync(Guid projectId, Guid memberId, CancellationToken ct);
    Task<IReadOnlyList<MemberDto>> GetProjectMembersAsync(Guid projectId, CancellationToken ct);
    Task<bool> RemoveFromProjectAsync(Guid projectId, Guid memberId, CancellationToken ct);
}
