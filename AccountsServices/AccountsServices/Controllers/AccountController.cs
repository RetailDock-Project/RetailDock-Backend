using Application.DTO;
using Application.Interfaces.IServices;
using Application.Services.AccountsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountsGroupService _accountsGroupService;
        public AccountController(IAccountsGroupService accountsGroupService)
        {
            _accountsGroupService = accountsGroupService;
        }
        [HttpPost("add/new/group")]
        public async Task<IActionResult> AddNewParentGroup(Guid OrganizationId, AddParentGroupDTO addGroupDTO)
        {
            var result = await _accountsGroupService.CreateParentGroup(OrganizationId, addGroupDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("add/new/sub/group")]
        public async Task<IActionResult> AddNewSubGroup(Guid OrganizationId, AddSubGroupDTO addGroupDTO)
        {
            var result = await _accountsGroupService.CreateSubGroup(OrganizationId, addGroupDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/sub/group")]
        public async Task<IActionResult> GetAllSubGroup(Guid OrganizationId)
        {
            var result = await _accountsGroupService.GetSubGroups(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/parent/group")]
        public async Task<IActionResult> GetAllParentGroup(Guid OrganizationId)
        {
            var result = await _accountsGroupService.GetParentGroups(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("create/default/groups/organizationId")]
        public async Task<IActionResult> CreateDefaultGroups(Guid organizationId, Guid CreatedBy)
        {
            var result = await _accountsGroupService.CreateDefaultGroups(organizationId, CreatedBy);
            return StatusCode(result.StatusCode, result);

        }
    }
}

