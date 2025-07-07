using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Common;
using Grpc.Core;

namespace AccountsServices.Controllers.BaseControllers.ValidateUserClaimsAttribute
{
    public class ValidateUserClaimsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            var userIdStr = user.FindFirst("user_id")?.Value;
            var orgIdStr = user.FindFirst("org_id")?.Value;

            var isValidUserId = Guid.TryParse(userIdStr, out var userId) && userId != Guid.Empty;
            var isValidOrgId = Guid.TryParse(orgIdStr, out var orgId) && orgId != Guid.Empty;

            if (!isValidUserId || !isValidOrgId)
            {
                var response = new ApiResponseDTO<object>
                {
                    StatusCode = 401,
                    Message = "Invalid or missing user_id/org_id in token",


                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = 401
                };

                return;

                base.OnActionExecuting(context);
            }
        }
    }
}
