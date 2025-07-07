using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using AutoMapper;
using Common;
using Domain.Entities;

namespace Application.Services
{
    public class RoleService:IRoleService
    {
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        public RoleService(IRoleRepository _roleRepository, IMapper _mapper) { 
            roleRepository= _roleRepository;
            mapper = _mapper;
        }


        //public async Task<ResponseDto<object>> AddRole(RoleAddDto newRole,Guid orgId) {
        //    var role = mapper.Map<OrganizationRole>(newRole);
        //    role.OrganizationId = orgId;
        //    //await roleRepository.AddRole(role);
        //    return new ResponseDto<object> { StatusCode = 200, Message = "Role added successfully" };

        //}



        public async Task<ResponseDto<object>> AddRoleWithPermissionsAsync(RoleDto newRole, Guid orgId)
        {
            var role = new OrganizationRole
            {
                Id = Guid.NewGuid(),
                Name = newRole.Name,
                OrganizationId = orgId
            };

            // Map permission IDs to OrganizationRolePermission
            role.OrganizationRolePermissions = newRole.PermissionIds.Select(permissionId => new OrganizationRolePermission
            {
                Id = Guid.NewGuid(),
                PermissionId = permissionId,
                OrganizationRoleId = role.Id // Set manually since not saved yet
            }).ToList();

            await roleRepository.AddRoleWithPermissionsAsync(role);

            return new ResponseDto<object>
            {
                StatusCode = 201,
                Message = "Role with permissions added successfully"
            };
        }



        public async Task<ResponseDto<object>> SoftDeleteRoleAsync(Guid roleId, Guid orgId)
        {
            var role = await roleRepository.GetRoleWithPermissionsAsync(roleId, orgId);
            if (role == null || role.IsDeleted)
            {
                return new ResponseDto<object>
                {
                    StatusCode = 404,
                    Message = "Role not found"
                };
            }

            role.IsDeleted = true;

            foreach (var permission in role.OrganizationRolePermissions)
            {
                permission.IsDeleted = true;
            }

            await roleRepository.UpdateRoleWithPermissionsAsync(role);

            return new ResponseDto<object>
            {
                StatusCode = 200,
                Message = "Role and its permissions soft deleted successfully"
            };
        }


        //public async Task<ResponseDto<object>> UpdateRole(RoleDto updatedRole)
        //{
        //    var role = mapper.Map<Role>(updatedRole);
        //    //await roleRepository.UpdateRole(role);
        //    return new ResponseDto<object> { StatusCode = 200, Message = "Role updated successfully" };
        //}

        public async Task<ResponseDto<object>> UpdateRoleWithPermissionsAsync(Guid roleId, RoleDto updatedRole, Guid orgId)
        {
            var role = await roleRepository.GetRoleWithPermissionsAsync(roleId, orgId);
            if (role == null)
            {
                return new ResponseDto<object>
                {
                    StatusCode = 404,
                    Message = "Role not found"
                };
            }

            // Update role name
            role.Name = updatedRole.Name;

            // Remove existing permissions
            role.OrganizationRolePermissions.Clear();

            // Add new permissions
            foreach (var permissionId in updatedRole.PermissionIds)
            {
                role.OrganizationRolePermissions.Add(new OrganizationRolePermission
                {
                    Id = Guid.NewGuid(),
                    OrganizationRoleId = roleId,
                    PermissionId = permissionId
                });
            }

            await roleRepository.UpdateRoleWithPermissionsAsync(role);

            return new ResponseDto<object>
            {
                StatusCode = 200,
                Message = "Role updated successfully"
            };
        }


        public async Task<ResponseDto<object>> SoftDeleteRole(int id)
        {
            //await roleRepository.SoftDeleteRole(id);
            return new ResponseDto<object> { StatusCode = 200, Message = "Role deleted successfully" };
        }

        //public async Task<ResponseDto<List<GetOrgRoleDto>>> GetAllRoles(Guid orgId) { 
        //    var roles=await roleRepository.GetAllRoles(orgId);
        //    if (roles == null || !roles.Any()) {
        //        return new ResponseDto<List<GetOrgRoleDto>> { StatusCode = 200 ,Message="Roles not found"};
        //    }
        //    var result=mapper.Map<List<GetOrgRoleDto>>(roles);
        //    return new ResponseDto<List<GetOrgRoleDto>> { StatusCode = 200, Message = "Roles retrieved",Data=result };

        //}

        //public async Task<ResponseDto<object>> AddOrganizationRoles(List<OrgRoleDto> orgRoles) {
        //    var roles = mapper.Map<List<OrganizationRole>>(orgRoles);
        //    var alreadyAddedRole = await roleRepository.checkAlreadyAddedRole(orgRoles);
        //    if (alreadyAddedRole != null) {
        //        return new ResponseDto<object> { StatusCode = 200, Message = "one organization role already exist" };
        //    }
        //    await roleRepository.AddOrganizationRoles(roles);
        //    return new ResponseDto<object> { StatusCode = 200 ,Message="Organization roles added"};
        //}

        public async Task<ResponseDto<List<GetOrgRoleDto>>> GetOrganizationRoles(Guid organizationId) { 
        var orgRoles=await roleRepository.GetOrganizationRoles(organizationId);
            var result=mapper.Map<List<GetOrgRoleDto>>(orgRoles);
            return new ResponseDto<List<GetOrgRoleDto>> { StatusCode = 200, Message = "Organization roles retrieved",Data=result };
        }

        //public async Task<ResponseDto<object>> AddOrgRolePermission(OrgRolePermissionAddDto orgRolePermission) {

        //    //var alreadyAddedPermission = await roleRepository.checkPermissionAlreadyAdded(orgRolePermission);
        //    foreach (int permissionId in orgRolePermission.PermissionIds) {
        //        var permission = new OrgRolePermissionDto { OrganizationRoleId = orgRolePermission.OrganizationRoleId, PermissionId = permissionId };
        //        var data= mapper.Map<OrganizationRolePermission>(permission);
        //    await roleRepository.AddOrgRolePermission(data);
        //    }
        //    return new ResponseDto<object> { StatusCode = 200, Message = "Organization role permissions added" };

        //}

        public async Task<ResponseDto<List<GetOrgRolePermissionDto>>> GetOrgRolePermissions(Guid organizationRoleId) { 
            var data=await roleRepository.GetOrgRolePermissions(organizationRoleId);
            var result =mapper.Map<List<GetOrgRolePermissionDto>>(data);
            return new ResponseDto<List<GetOrgRolePermissionDto>> { StatusCode = 200, Message = "Organization role permissions retrieved",Data=result };

        }

        //public async Task<ResponseDto<object>> UpdateOrganizationRolePermissions(OrgRolePermissionAddDto updatedPermissions) {
        //    foreach (int permissionId in updatedPermissions.PermissionIds)
        //    {
        //        var permission = new OrgRolePermissionDto { OrganizationRoleId = updatedPermissions.OrganizationRoleId, PermissionId = permissionId };
        //        var data = mapper.Map<OrganizationRolePermission>(permission);
        //        await roleRepository.UpdateOrganizationRolePermissions(data);
        //    }
        //    return new ResponseDto<object> { StatusCode = 200, Message = "Organization role permissions updated" };
        //}

        public async Task<ResponseDto<object>> AssignUserOrganizationRole(UserOrgRole newOrgUser) {
            var data=mapper.Map<UserOrganizationRole>(newOrgUser);
            await roleRepository.AssignUserOrganizationRole(data);
            return new ResponseDto<object> { StatusCode = 200, Message = "assigned user to organization role" };
        }

        public async Task<ResponseDto<List<PermissionDto>>> GetPermissions()
        {
            var data = await roleRepository.GetPermissions();
            var res=mapper.Map<List<PermissionDto>>(data);
            
            return new ResponseDto<List<PermissionDto>> { StatusCode = 200, Message = "Permissions retrieved", Data = res };

        }

        public async Task<ResponseDto<object>> AddPermission(PermissionDto newPermission)

        {
            var permission=mapper.Map<Permission>(newPermission);
            await roleRepository.AddPermission(permission);
            return new ResponseDto<object> { StatusCode = 200, Message = "New permission is added" };

        }

        public async Task<ResponseDto<List<GetOrgRoleWithPermissionsDto>>> GetOrganizationRolesWithPermissions(Guid organizationId)
        {
            var roles = await roleRepository.GetOrganizationRolesWithPermissions(organizationId);
            var result = mapper.Map<List<GetOrgRoleWithPermissionsDto>>(roles);

            return new ResponseDto<List<GetOrgRoleWithPermissionsDto>>
            {
                StatusCode = 200,
                Message = "Organization roles with permissions retrieved",
                Data = result
            };
        }


    }
}
