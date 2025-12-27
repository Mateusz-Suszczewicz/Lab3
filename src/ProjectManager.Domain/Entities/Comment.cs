using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Domain.Entities;

public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TaskItemId { get; set; }
    public TaskItem? TaskItem { get; set; }

    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
