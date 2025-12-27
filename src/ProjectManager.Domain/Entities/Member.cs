using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Domain.Entities;

public class Member
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    public ICollection<ProjectMember> Projects { get; set; } = new List<ProjectMember>();
}
