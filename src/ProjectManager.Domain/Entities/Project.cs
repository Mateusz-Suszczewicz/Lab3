using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Domain.Entities;

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? PlannedEndDate { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
}
