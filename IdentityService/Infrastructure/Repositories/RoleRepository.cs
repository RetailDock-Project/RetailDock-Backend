using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using AutoMapper;
using Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository:IRoleRepository
    {
        private readonly IdentityDbContext context;
        private readonly IMapper mapper;
        public RoleRepository(IdentityDbContext _context,IMapper _mapper) {
            context = _context;
            mapper = _mapper;
        }

        public async Task<List<Role>> GetAllRoles(Guid orgId) { 
        return await context.Roles.Where(role=>role.OrganizationId==orgId && !role.IsDeleted).ToListAsync();
        }

        public async Task AddRole(Role role)
        {
                await context.Roles.AddAsync(role);
                await context.SaveChangesAsync();
        }

        public async Task UpdateRole(Role updatedRole)
        {
            var role = await context.Roles.FindAsync(updatedRole.Id);
            if (role != null)
            {
                role.Name = updatedRole.Name;
                context.Roles.Update(role);
                await context.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteRole(int id)
        {
            var role = await context.Roles.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                context.Roles.Update(role);
                await context.SaveChangesAsync();
            }
        }


        public async Task AddOrganizationRoles(List<OrganizationRole> orgRoles) {

            
            foreach (OrganizationRole orgRole in orgRoles) 
            {
                await context.OrganizationRoles.AddAsync(orgRole);
                await context.SaveChangesAsync();

            }

        }

        public async Task<List<OrganizationRole>> GetOrganizationRoles(Guid organizationId) {
            return await context.OrganizationRoles.Include(x=>x.Role).Where(x => x.OrganizationId == organizationId).ToListAsync();
        }

        public async Task AddOrgRolePermission(OrganizationRolePermission orgRolePermission) {
            await context.OrganizationRolePermissions.AddAsync(orgRolePermission);
            await context.SaveChangesAsync();

        }

        public async Task<List<OrganizationRolePermission>> GetOrgRolePermissions(Guid organizationRoleId) {
            return await context.OrganizationRolePermissions.Include(x=>x.Permission).Where(x => x.OrganizationRoleId == organizationRoleId).ToListAsync();
        }

        public async Task UpdateOrganizationRolePermissions(OrganizationRolePermission updatePermission) {
            var data=await context.OrganizationRolePermissions.FirstOrDefaultAsync(x => x.OrganizationRoleId == updatePermission.OrganizationRoleId);
            data.PermissionId= updatePermission.PermissionId;
            await context.SaveChangesAsync();
        }
        public async Task AssignUserOrganizationRole(UserOrganizationRole newOrgUser) {
            await context.UserOrganizationRoles.AddAsync(newOrgUser);
            await context.SaveChangesAsync();
        }

        public async Task<List<Permission>> GetPermissions()
        {
            var data = await context.Permissions.ToListAsync();
            return data;

        }

        public async Task AddPermission(Permission newPermission)
        {
            await context.Permissions.AddAsync(newPermission);
            await context.SaveChangesAsync();
        }

        public async Task<object> checkAlreadyAddedRole(List<OrgRoleDto> orgRoles) {
            foreach (OrgRoleDto i in orgRoles) {

                var exist=await context.OrganizationRoles.FirstOrDefaultAsync(x => x.RoleId == i.RoleId && x.OrganizationId == i.OrganizationId);
                if (exist != null) {
                return exist;
                }
            }
            return null;
        }

        public async Task checkPermissionAlreadyAdded(OrgRolePermissionAddDto orgRolePermission) {
            foreach (int permissionId in orgRolePermission.PermissionIds) {
                var exist = await context.OrganizationRolePermissions.FirstOrDefaultAsync(x => x.PermissionId == permissionId && x.OrganizationRoleId == orgRolePermission.OrganizationRoleId);
            }
        }
    }
}
