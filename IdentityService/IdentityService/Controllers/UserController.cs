using Application.Interfaces.IServices;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService userService;
        public UserController(IUserService _userService) {
        userService = _userService;
        }
        [HttpGet("{organizationId}/users")]
        public async Task<IActionResult> GetUsersByOrgId() {
            var response = await userService.GetUsersByOrgId(OrgId);
            return StatusCode(response.StatusCode, response);
            
        }
    }
}
