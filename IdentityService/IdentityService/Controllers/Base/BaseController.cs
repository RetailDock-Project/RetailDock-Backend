using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid UserId => TryParseGuid(User.FindFirst("user_id")?.Value);
        protected Guid OrgId => TryParseGuid(User.FindFirst("org_id")?.Value);

        private Guid TryParseGuid(string? value)
        {
            return Guid.TryParse(value, out var result) ? result : Guid.Empty;
        }

        protected string? Role =>
            User.FindFirst(ClaimTypes.Role)?.Value;

        protected List<string> Permissions =>
            User.FindFirst("permissions")?.Value?
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? new List<string>();
    }
}
