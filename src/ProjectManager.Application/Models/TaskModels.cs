using ProjectManager.Domain.Entities;

namespace ProjectManager.Application.Models;

public record CreateTaskRequest(string Title, string? Description, TaskPriority Priority, DateOnly? DueDate);
public record UpdateTaskRequest(string Title, string? Description, TaskPriority Priority, DateOnly? DueDate);

public class TaskDto
{
    public Guid Id { get; init; }
    public Guid ProjectId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public TaskPriority Priority { get; init; }
    public DateOnly? DueDate { get; init; }

    public Dictionary<string, LinkDto> _links { get; set; } = new();
}
