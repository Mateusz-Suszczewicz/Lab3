namespace ProjectManager.Application.Models;

public record CreateProjectRequest(string Name, string? Description, DateOnly StartDate, DateOnly? PlannedEndDate);
public record UpdateProjectRequest(string Name, string? Description, DateOnly StartDate, DateOnly? PlannedEndDate);

public class ProjectDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateOnly StartDate { get; init; }
    public DateOnly? PlannedEndDate { get; init; }

    public Dictionary<string, LinkDto> _links { get; set; } = new();
}
