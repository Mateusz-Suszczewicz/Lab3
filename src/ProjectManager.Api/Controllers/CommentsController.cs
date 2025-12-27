using Microsoft.AspNetCore.Mvc;
using ProjectManager.Api.Hateoas;
using ProjectManager.Application.Abstractions;
using ProjectManager.Application.Models;

namespace ProjectManager.Api.Controllers;

[ApiController]
[Route("api/tasks/{taskId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments;

    public CommentsController(ICommentService comments) => _comments = comments;

    [HttpPost(Name = "CreateTaskComment")]
    public async Task<ActionResult<CommentDto>> Create(Guid taskId, [FromBody] CreateCommentRequest request, CancellationToken ct)
    {
        var created = await _comments.CreateAsync(taskId, request, ct);
        if (created is null) return NotFound(new { message = "Task not found" });

        AddLinks(created);
        return CreatedAtRoute("GetTaskComments", new { taskId }, created);
    }

    [HttpGet(Name = "GetTaskComments")]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetAll(Guid taskId, CancellationToken ct)
    {
        var list = await _comments.GetAllAsync(taskId, ct);
        foreach (var c in list) AddLinks(c);
        return Ok(list);
    }

    [HttpDelete("{commentId:guid}", Name = "DeleteTaskComment")]
    public async Task<IActionResult> Delete(Guid taskId, Guid commentId, CancellationToken ct)
    {
        var ok = await _comments.DeleteAsync(taskId, commentId, ct);
        return ok ? NoContent() : NotFound();
    }

    private void AddLinks(CommentDto c)
    {
        c._links = new()
        {
            ["self"] = LinkBuilder.Link(Url, "GetTaskComments", new { taskId = c.TaskItemId }),
            ["delete"] = LinkBuilder.Link(Url, "DeleteTaskComment", new { taskId = c.TaskItemId, commentId = c.Id }, "DELETE")
        };
    }
}
