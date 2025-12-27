namespace ProjectManager.Application.Models;

public record CreateCommentRequest(string Content);

public class CommentDto
{
    public Guid Id { get; init; }
    public Guid TaskItemId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }

    public Dictionary<string, LinkDto> _links { get; set; } = new();
}
