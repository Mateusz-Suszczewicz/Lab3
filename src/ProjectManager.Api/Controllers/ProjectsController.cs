using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Hateoas;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projects;

    public ProjectsController(IProjectService projects) => _projects = projects;

    [HttpPost(Name = "CreateProject")]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request, CancellationToken ct)
    {
        var created = await _projects.CreateAsync(request, ct);
        AddLinks(created);
        return CreatedAtRoute("GetProjectById", new { id = created.Id }, created);
    }

    [HttpGet(Name = "GetProjects")]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken ct)
    {
        var list = await _projects.GetAllAsync(ct);
        foreach (var p in list) AddLinks(p);
        return Ok(list);
    }

    [HttpGet("{id:guid}", Name = "GetProjectById")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken ct)
    {
        var project = await _projects.GetByIdAsync(id, ct);
        if (project is null) return NotFound();
        AddLinks(project);
        return Ok(project);
    }

    [HttpPut("{id:guid}", Name = "UpdateProject")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken ct)
    {
        var ok = await _projects.UpdateAsync(id, request, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}", Name = "DeleteProject")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var ok = await _projects.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }

    private void AddLinks(ProjectDto p)
    {
        p._links = new()
        {
            ["self"] = LinkBuilder.Link(Url, "GetProjectById", new { id = p.Id }),
            ["update"] = LinkBuilder.Link(Url, "UpdateProject", new { id = p.Id }, "PUT"),
            ["delete"] = LinkBuilder.Link(Url, "DeleteProject", new { id = p.Id }, "DELETE"),
            ["tasks"] = LinkBuilder.Link(Url, "GetProjectTasks", new { projectId = p.Id }),
            ["members"] = LinkBuilder.Link(Url, "GetProjectMembers", new { projectId = p.Id })
        };
    }
}
