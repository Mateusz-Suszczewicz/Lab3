using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Hateoas;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;

namespace ProjectManager.Api.Controllers;

[ApiController]
public class MembersController : ControllerBase
{
    private readonly IMemberService _members;

    public MembersController(IMemberService members) => _members = members;

    [HttpPost("api/members", Name = "CreateMember")]
    public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberRequest request, CancellationToken ct)
    {
        var created = await _members.CreateMemberAsync(request, ct);
        AddLinks(created, projectId: null);
        return CreatedAtRoute("GetMemberByIdStub", new { id = created.Id }, created);
    }

    [HttpGet("api/members/{id:guid}", Name = "GetMemberByIdStub")]
    public IActionResult GetMemberByIdStub(Guid id) => NoContent();

    [HttpPost("api/projects/{projectId:guid}/members", Name = "AddProjectMember")]
    public async Task<IActionResult> AddToProject(Guid projectId, [FromBody] AddMemberToProjectRequest request, CancellationToken ct)
    {
        var ok = await _members.AddToProjectAsync(projectId, request.MemberId, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("api/projects/{projectId:guid}/members", Name = "GetProjectMembers")]
    public async Task<ActionResult<IReadOnlyList<MemberDto>>> GetProjectMembers(Guid projectId, CancellationToken ct)
    {
        var list = await _members.GetProjectMembersAsync(projectId, ct);
        foreach (var m in list) AddLinks(m, projectId);
        return Ok(list);
    }

    [HttpDelete("api/projects/{projectId:guid}/members/{memberId:guid}", Name = "RemoveProjectMember")]
    public async Task<IActionResult> RemoveFromProject(Guid projectId, Guid memberId, CancellationToken ct)
    {
        var ok = await _members.RemoveFromProjectAsync(projectId, memberId, ct);
        return ok ? NoContent() : NotFound();
    }

    private void AddLinks(MemberDto m, Guid? projectId)
    {
        m._links = new()
        {
            ["self"] = LinkBuilder.Link(Url, "GetMemberByIdStub", new { id = m.Id })
        };

        if (projectId is not null)
        {
            m._links["removeFromProject"] = LinkBuilder.Link(Url, "RemoveProjectMember", new { projectId, memberId = m.Id }, "DELETE");
            m._links["project"] = LinkBuilder.Link(Url, "GetProjectById", new { id = projectId });
        }
    }
}

public record AddMemberToProjectRequest(Guid MemberId);
