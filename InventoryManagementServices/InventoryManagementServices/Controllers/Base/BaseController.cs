using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid UserId => TryParseGuid(User.FindFirst("user_id")?.Value);
        protected Guid OrgId => TryParseGuid(User.FindFirst("org_id")?.Value);

        private Guid TryParseGuid(string? value)
        {
            return Guid.TryParse(value, out var result) ? result : Guid.Empty;
        }


        protected List<string> Roles =>
    User.Claims
        .Where(c => c.Type == ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList();


        protected List<string> Permissions =>
            User.FindFirst("permissions")?.Value?.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

    }
}
