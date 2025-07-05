using AccountsServices.Controllers.BaseControllers;
using Application.DTO;
using Application.Interfaces.IServices;
using Application.Services.AccountsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseController
    {
        private readonly IAccountsGroupService _accountsGroupService;
        public AccountsController(IAccountsGroupService accountsGroupService)
        {
            _accountsGroupService = accountsGroupService;
        }
        [HttpPost("add/new/group")]
        public async Task<IActionResult> AddNewParentGroup( AddParentGroupDTO addGroupDTO)
        {
            addGroupDTO.CreatedBy = UserId;
            var result = await _accountsGroupService.CreateParentGroup(OrgId, addGroupDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("add/new/sub/group")]
        public async Task<IActionResult> AddNewSubGroup(AddSubGroupDTO addGroupDTO)
        {
            addGroupDTO.CreatedBy = UserId;
            var result = await _accountsGroupService.CreateSubGroup(OrgId, addGroupDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/sub/group")]
        public async Task<IActionResult> GetAllSubGroup()
        {
            var result = await _accountsGroupService.GetSubGroups(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/parent/group")]
        public async Task<IActionResult> GetAllParentGroup()
        {
            var result = await _accountsGroupService.GetParentGroups(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("create/default/groups/organizationId")]
        public async Task<IActionResult> CreateDefaultGroups()
        {
            var result = await _accountsGroupService.CreateDefaultGroups(OrgId, UserId);
            return StatusCode(result.StatusCode, result);

        }
    }
}

