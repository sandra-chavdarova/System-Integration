using Microsoft.AspNetCore.Http;
using Service.Interface;
using System.Security.Claims;

namespace Service.Implementation;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;
    public CurrentUserService(IHttpContextAccessor accessor) { _accessor = accessor; }
    public string? GetUserId() =>
        _accessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}
