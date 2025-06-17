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


        public async Task<ResponseDto<object>> AddRole(RoleAddDto newRole,Guid orgId) {
            var role = mapper.Map<Role>(newRole);
            role.OrganizationId = orgId;
            await roleRepository.AddRole(role);
            return new ResponseDto<object> { StatusCode = 200, Message = "Role added successfully" };

        }

        public async Task<ResponseDto<object>> UpdateRole(RoleDto updatedRole)
        {
            var role = mapper.Map<Role>(updatedRole);
            await roleRepository.UpdateRole(role);
            return new ResponseDto<object> { StatusCode = 200, Message = "Role updated successfully" };
        }

        public async Task<ResponseDto<object>> SoftDeleteRole(int id)
        {
            await roleRepository.SoftDeleteRole(id);
            return new ResponseDto<object> { StatusCode = 200, Message = "Role deleted successfully" };
        }

        public async Task<ResponseDto<List<RoleDto>>> GetAllRoles(Guid orgId) { 
            var roles=await roleRepository.GetAllRoles(orgId);
            if (roles == null || !roles.Any()) {
                return new ResponseDto<List<RoleDto>> { StatusCode = 200 ,Message="Roles not found"};
            }
            var result=mapper.Map<List<RoleDto>>(roles);
            return new ResponseDto<List<RoleDto>> { StatusCode = 200, Message = "Roles retrieved",Data=result };

        }

        public async Task<ResponseDto<object>> AddOrganizationRoles(List<OrgRoleDto> orgRoles) {
            var roles = mapper.Map<List<OrganizationRole>>(orgRoles);
            var alreadyAddedRole = await roleRepository.checkAlreadyAddedRole(orgRoles);
            if (alreadyAddedRole != null) {
                return new ResponseDto<object> { StatusCode = 200, Message = "one organization role already exist" };
            }
            await roleRepository.AddOrganizationRoles(roles);
            return new ResponseDto<object> { StatusCode = 200 ,Message="Organization roles added"};
        }

        public async Task<ResponseDto<List<GetOrgRoleDto>>> GetOrganizationRoles(Guid organizationId) { 
        var orgRoles=await roleRepository.GetOrganizationRoles(organizationId);
            var result=mapper.Map<List<GetOrgRoleDto>>(orgRoles);
            return new ResponseDto<List<GetOrgRoleDto>> { StatusCode = 200, Message = "Organization roles retrieved",Data=result };
        }

        public async Task<ResponseDto<object>> AddOrgRolePermission(OrgRolePermissionAddDto orgRolePermission) {

            //var alreadyAddedPermission = await roleRepository.checkPermissionAlreadyAdded(orgRolePermission);
            foreach (int permissionId in orgRolePermission.PermissionIds) {
                var permission = new OrgRolePermissionDto { OrganizationRoleId = orgRolePermission.OrganizationRoleId, PermissionId = permissionId };
                var data= mapper.Map<OrganizationRolePermission>(permission);
            await roleRepository.AddOrgRolePermission(data);
            }
            return new ResponseDto<object> { StatusCode = 200, Message = "Organization role permissions added" };

        }

        public async Task<ResponseDto<List<GetOrgRolePermissionDto>>> GetOrgRolePermissions(Guid organizationRoleId) { 
            var data=await roleRepository.GetOrgRolePermissions(organizationRoleId);
            var result =mapper.Map<List<GetOrgRolePermissionDto>>(data);
            return new ResponseDto<List<GetOrgRolePermissionDto>> { StatusCode = 200, Message = "Organization role permissions retrieved",Data=result };

        }

        public async Task<ResponseDto<object>> UpdateOrganizationRolePermissions(OrgRolePermissionAddDto updatedPermissions) {
            foreach (int permissionId in updatedPermissions.PermissionIds)
            {
                var permission = new OrgRolePermissionDto { OrganizationRoleId = updatedPermissions.OrganizationRoleId, PermissionId = permissionId };
                var data = mapper.Map<OrganizationRolePermission>(permission);
                await roleRepository.UpdateOrganizationRolePermissions(data);
            }
            return new ResponseDto<object> { StatusCode = 200, Message = "Organization role permissions updated" };
        }

        public async Task<ResponseDto<object>> AssignUserOrganizationRole(UserOrgRole newOrgUser) {
            var data=mapper.Map<UserOrganizationRole>(newOrgUser);
            await roleRepository.AssignUserOrganizationRole(data);
            return new ResponseDto<object> { StatusCode = 200, Message = "assigned user to organization role" };
        }

        public async Task<ResponseDto<List<PermissionDto>>> GetPermissions()
        {
            var data = await roleRepository.GetPermissions();
            var res=mapper.Map<List<PermissionDto>>(data);
            
            return new ResponseDto<List<PermissionDto>> { StatusCode = 200, Message = "Organization role permissions retrieved", Data = res };

        }

        public async Task<ResponseDto<object>> AddPermission(PermissionDto newPermission)

        {
            var permission=mapper.Map<Permission>(newPermission);
            await roleRepository.AddPermission(permission);
            return new ResponseDto<object> { StatusCode = 200, Message = "New permission is added" };

        }
    }
}
