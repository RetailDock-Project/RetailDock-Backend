using Application.DTOs;
using Application.Interfaces.IServices;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService roleService;
        public RoleController(IRoleService _roleService) {
            roleService = _roleService;
        }

        [HttpGet("roles/orgId")]
        public async Task<IActionResult> GetAllRoles() {
            var response=await roleService.GetAllRoles(OrgId);
            return StatusCode(response.StatusCode, response);
        }



        [HttpPost("add")]
        public async Task<IActionResult> AddRole(RoleAddDto newRole) {
            var response=await roleService.AddRole(newRole,OrgId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateRole(RoleDto updatedRole)
        {
            var response = await roleService.UpdateRole(updatedRole);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteRole(int id)
        {
            var response = await roleService.SoftDeleteRole(id);
            return StatusCode(response.StatusCode, response);
        }
        //add roles of org
        [HttpPost("organization-roles")]
        public async Task<IActionResult> AddOrganizationRoles(List<OrgRoleDto> orgRoles) {
            var response = await roleService.AddOrganizationRoles(orgRoles);
            return StatusCode(response.StatusCode, response);
        }

        //get roles org
        [HttpGet("organization-roles/{organizationId}")]
        public async Task<IActionResult> GetOrganizationRoles(Guid organizationId) { 
            var response=await roleService.GetOrganizationRoles(organizationId);
            return StatusCode(response.StatusCode, response);
        }

        //add org role permission

        [HttpPost("organization-role-permissions")]
        public async Task<IActionResult> AddOrgRolePermission(OrgRolePermissionAddDto orgRolePermission) {
            var response=await roleService.AddOrgRolePermission(orgRolePermission);
            return StatusCode(response.StatusCode,response);
        }

        //get org role permissions

        [HttpGet("organization-role-permissions/{organizationRoleId}")]
        public async Task<IActionResult> GetOrgRolePermissions(Guid organizationRoleId) { 
            var response=await roleService.GetOrgRolePermissions(organizationRoleId);   
            return StatusCode(response.StatusCode, response);
        }

        //update org role permissions
        [HttpPut("organization-role-permissions/organizationRoleId")]
        public async Task<IActionResult> UpdateOrganizationRolePermissions(OrgRolePermissionAddDto updateData) { 
            var response=await roleService.UpdateOrganizationRolePermissions(updateData);
            return StatusCode(response.StatusCode, response);
        }

        //user role assignment

        [HttpPost("user-organization-roles/userId")]
        public async Task<IActionResult> AssignUserOrganizationRole(UserOrgRole newOrgUser) {
            var response = await roleService.AssignUserOrganizationRole(newOrgUser);
            return StatusCode(response.StatusCode, response);
        }

        //get permissions list
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var response = await roleService.GetPermissions();
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("permissions/add")]
        public async Task<IActionResult> AddPermission(PermissionDto newPermission) { 
            var response= await roleService.AddPermission(newPermission);
            return StatusCode(response.StatusCode, response);
        }
    }
}
