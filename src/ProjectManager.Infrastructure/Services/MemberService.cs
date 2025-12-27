using Microsoft.EntityFrameworkCore;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;
using ProjectManager.Domain.Entities;
using ProjectManager.Infrastructure.Persistence;

namespace ProjectManager.Infrastructure.Services;

public class MemberService : IMemberService
{
    private readonly AppDbContext _db;

    public MemberService(AppDbContext db) => _db = db;

    public async Task<MemberDto> CreateMemberAsync(CreateMemberRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var entity = new Member
        {
            Name = request.Name.Trim(),
            Email = email
        };

        _db.Members.Add(entity);
        await _db.SaveChangesAsync(ct);

        return Map(entity);
    }

    public async Task<bool> AddToProjectAsync(Guid projectId, Guid memberId, CancellationToken ct)
    {
        var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId, ct);
        var memberExists = await _db.Members.AnyAsync(m => m.Id == memberId, ct);
        if (!projectExists || !memberExists) return false;

        var already = await _db.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId, ct);
        if (already) return true;

        _db.ProjectMembers.Add(new ProjectMember { ProjectId = projectId, MemberId = memberId });
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<IReadOnlyList<MemberDto>> GetProjectMembersAsync(Guid projectId, CancellationToken ct)
    {
        var members = await _db.ProjectMembers.AsNoTracking()
            .Where(pm => pm.ProjectId == projectId)
            .Include(pm => pm.Member)
            .Select(pm => pm.Member!)
            .OrderBy(m => m.Name)
            .ToListAsync(ct);

        return members.Select(Map).ToList();
    }

    public async Task<bool> RemoveFromProjectAsync(Guid projectId, Guid memberId, CancellationToken ct)
    {
        var entity = await _db.ProjectMembers.SingleOrDefaultAsync(pm => pm.ProjectId == projectId && pm.MemberId == memberId, ct);
        if (entity is null) return false;

        _db.ProjectMembers.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private static MemberDto Map(Member m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Email = m.Email
    };
}
