using System.Security.Claims;

namespace CVMaker.Services;

public class ContextAccessor : IContextAccessorService
{
    public ContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated is true)
        {
           UserId = Guid.Parse(httpContextAccessor.HttpContext.User.FindFirst(claim => claim.Type == ClaimTypes.Sid)?.Value ?? string.Empty); 
        }
    }
    public Guid UserId { get; set; } 
}