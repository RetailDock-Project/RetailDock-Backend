using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IRoleService
    {
        Task<ResponseDto<object>> AddRole(RoleAddDto newRole,Guid orgId);
        Task<ResponseDto<object>> UpdateRole(RoleDto updatedRole);
        Task<ResponseDto<object>> SoftDeleteRole(int id);
        Task<ResponseDto<List<RoleDto>>> GetAllRoles(Guid orgId);
        Task<ResponseDto<object>> AddOrganizationRoles(List<OrgRoleDto> orgRoles);
        Task<ResponseDto<List<GetOrgRoleDto>>> GetOrganizationRoles(Guid organizationId);

        Task<ResponseDto<object>> AddOrgRolePermission(OrgRolePermissionAddDto orgRolePermission);
        Task<ResponseDto<List<GetOrgRolePermissionDto>>> GetOrgRolePermissions(Guid organizationRoleId);
        Task<ResponseDto<object>> UpdateOrganizationRolePermissions(OrgRolePermissionAddDto updatedPermissions);
        Task<ResponseDto<object>> AssignUserOrganizationRole(UserOrgRole newOrgUser);
        Task<ResponseDto<List<PermissionDto>>> GetPermissions();
        Task<ResponseDto<object>> AddPermission(PermissionDto newPermission);
    }
}
