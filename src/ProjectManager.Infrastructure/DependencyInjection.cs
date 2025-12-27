using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManager.Application.Abstractions;
using ProjectManager.Infrastructure.Persistence;
using ProjectManager.Infrastructure.Services;

namespace ProjectManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("Default");
        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));

        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ICommentService, CommentService>();

        return services;
    }
}
