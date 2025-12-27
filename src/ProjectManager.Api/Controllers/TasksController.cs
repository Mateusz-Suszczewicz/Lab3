using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Hateoas;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;

    public TasksController(ITaskService tasks) => _tasks = tasks;

    [HttpPost(Name = "CreateProjectTask")]
    public async Task<ActionResult<TaskDto>> Create(Guid projectId, [FromBody] CreateTaskRequest request, CancellationToken ct)
    {
        var created = await _tasks.CreateAsync(projectId, request, ct);
        if (created is null) return NotFound(new { message = "Project not found" });

        AddLinks(created);
        return CreatedAtRoute("GetProjectTaskById", new { projectId, taskId = created.Id }, created);
    }

    [HttpGet(Name = "GetProjectTasks")]
    public async Task<ActionResult<IReadOnlyList<TaskDto>>> GetAll(Guid projectId, CancellationToken ct)
    {
        var list = await _tasks.GetAllAsync(projectId, ct);
        foreach (var t in list) AddLinks(t);
        return Ok(list);
    }

    [HttpGet("{taskId:guid}", Name = "GetProjectTaskById")]
    public async Task<ActionResult<TaskDto>> GetById(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var task = await _tasks.GetByIdAsync(projectId, taskId, ct);
        if (task is null) return NotFound();
        AddLinks(task);
        return Ok(task);
    }

    [HttpPut("{taskId:guid}", Name = "UpdateProjectTask")]
    public async Task<IActionResult> Update(Guid projectId, Guid taskId, [FromBody] UpdateTaskRequest request, CancellationToken ct)
    {
        var ok = await _tasks.UpdateAsync(projectId, taskId, request, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{taskId:guid}", Name = "DeleteProjectTask")]
    public async Task<IActionResult> Delete(Guid projectId, Guid taskId, CancellationToken ct)
    {
        var ok = await _tasks.DeleteAsync(projectId, taskId, ct);
        return ok ? NoContent() : NotFound();
    }

    private void AddLinks(TaskDto t)
    {
        t._links = new()
        {
            ["self"] = LinkBuilder.Link(Url, "GetProjectTaskById", new { projectId = t.ProjectId, taskId = t.Id }),
            ["update"] = LinkBuilder.Link(Url, "UpdateProjectTask", new { projectId = t.ProjectId, taskId = t.Id }, "PUT"),
            ["delete"] = LinkBuilder.Link(Url, "DeleteProjectTask", new { projectId = t.ProjectId, taskId = t.Id }, "DELETE"),
            ["project"] = LinkBuilder.Link(Url, "GetProjectById", new { id = t.ProjectId }),
            ["comments"] = LinkBuilder.Link(Url, "GetTaskComments", new { taskId = t.Id })
        };
    }
}
