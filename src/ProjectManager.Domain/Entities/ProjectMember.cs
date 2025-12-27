namespace ProjectManager.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Project? Project { get; set; }

    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
}
