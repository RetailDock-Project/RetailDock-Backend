using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IRoleRepository
    {
        Task AddRole(Role newRole);
        Task UpdateRole(Role role);
        Task SoftDeleteRole(int id);
        Task<List<Role>> GetAllRoles(Guid orgId);
        Task AddOrganizationRoles(List<OrganizationRole> orgRoles);
        Task<List<OrganizationRole>> GetOrganizationRoles(Guid organizationId);
        Task AddOrgRolePermission(OrganizationRolePermission orgRolePermission);
        Task<List<OrganizationRolePermission>> GetOrgRolePermissions(Guid organizationRoleId);
        Task UpdateOrganizationRolePermissions(OrganizationRolePermission updatedPermission);
        Task AssignUserOrganizationRole(UserOrganizationRole newOrgUser);
        Task<List<Permission>> GetPermissions();
        Task AddPermission(Permission newPermission);
        Task<object> checkAlreadyAddedRole(List<OrgRoleDto> orgRoles);

    }
}
