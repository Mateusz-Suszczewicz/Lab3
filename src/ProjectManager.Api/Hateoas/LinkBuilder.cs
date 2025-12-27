using Microsoft.AspNetCore.Mvc;
using ProjectManager.Application.Models;

namespace ProjectManager.Api.Hateoas;

public static class LinkBuilder
{
    public static LinkDto Link(IUrlHelper url, string routeName, object? values = null, string method = "GET")
    {
        var href = url.Link(routeName, values) ?? string.Empty;
        return new LinkDto(href, method);
    }
}
