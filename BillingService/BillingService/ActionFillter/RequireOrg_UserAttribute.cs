using Common.ResponseDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BillingService.ActionFillter
{
    public class RequireOrg_UserAttribute:ActionFilterAttribute
    {
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.User;
            var orgId = user.FindFirst("org_id")?.Value;
            var userId = user.FindFirst("user_id")?.Value;

            if (!Guid.TryParse(orgId, out var orgGuid) || orgGuid == Guid.Empty ||
                !Guid.TryParse(userId, out var userGuid) || userGuid == Guid.Empty)
              
            {

                var response = new ResponseDto<object>
                {
                    Message = "Invalid or missing OrganisationId/UserId.",
                    StatusCode = 401,
                };
              context.Result = new BadRequestObjectResult(response);
                return;
            }
       
        }
    }
}
