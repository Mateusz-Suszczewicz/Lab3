namespace ProjectManager.Application.Models;

public record CreateMemberRequest(string Name, string Email);

public class MemberDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public Dictionary<string, LinkDto> _links { get; set; } = new();
}
