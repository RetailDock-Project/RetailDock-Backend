using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepositories;
using Common;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class UserRepository: IUserRepository
    {
        private readonly IdentityDbContext context;
        public UserRepository(IdentityDbContext _context) {
            context = _context;
        }



        public async Task<User> GetUserById(Guid userId) {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            

        }



        public async Task<bool> UpdateUserOrganization(Guid userId,Guid orgId) {
            var user=await context.Users.FirstOrDefaultAsync(x=>x.Id== userId);

            if (user != null) { 
            user.OrganisationId = orgId;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task AddOrganizationRole(OrganizationRole newOrgRole) {

            await context.OrganizationRoles.AddAsync(newOrgRole);
            var permissions=await context.Permissions.ToListAsync();
            foreach (var permission in permissions) {
                await context.OrganizationRolePermissions.AddAsync(new OrganizationRolePermission { OrganizationRoleId = newOrgRole.Id, PermissionId = permission.Id });

            }
            await context.SaveChangesAsync();
            Console.WriteLine("updated orgrole");

        }

        public async Task AddUserOrgRole(UserOrganizationRole newUserRole) {
            await context.UserOrganizationRoles.AddAsync(newUserRole);
            await context.SaveChangesAsync();
            Console.WriteLine("updated user prgrole");
        }

        public async Task<List<OrganizationUserDto>> GetUsersByOrgId(Guid orgId) {

            var res = await context.Users
    .Include(u => u.UserOrganizationRole)
        .ThenInclude(uor => uor.OrganizationRole)
    .Where(u => u.OrganisationId == orgId)
    .Select(u => new OrganizationUserDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Role = u.UserOrganizationRole.OrganizationRole.Name,
        Created=u.CreatedAt
    })
    .ToListAsync();


            return res;
        }

        public async Task<UserOrganizationRole> GetUserOrganizationRoleAsync(Guid userId)
        {
            return await context.UserOrganizationRoles
                .FirstOrDefaultAsync(uor => uor.UserId == userId);
        }

        public async Task UpdateUserOrganizationRoleAsync(UserOrganizationRole userOrgRole)
        {
            context.UserOrganizationRoles.Update(userOrgRole);
            await context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }


        public async Task<List<OrganizationUserDto>> GetFilteredUsersByOrgId(
    Guid orgId, string? search, Guid? roleId, Guid? userId)
        {
            var query = context.Users
                .Include(u => u.UserOrganizationRole)
                    .ThenInclude(uor => uor.OrganizationRole)
                .Where(u => u.OrganisationId == orgId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Name.Contains(search) || u.Email.Contains(search));
            }

            if (roleId.HasValue)
            {
                query = query.Where(u => u.UserOrganizationRole.OrganizationRoleId == roleId.Value);
            }

            if (userId.HasValue)
            {
                query = query.Where(u => u.Id == userId.Value);
            }

            var users = await query.Select(u => new OrganizationUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.UserOrganizationRole.OrganizationRole.Name,
                Created = u.CreatedAt
            }).ToListAsync();

            return users;
        }


        public async Task<UserStatsDto> GetUserStatsByOrgId(Guid orgId)
        {
            var users = context.Users.Where(u => u.OrganisationId == orgId);

            var totalUsers = await users.CountAsync();
            var activeUsers = await users.CountAsync(u => !u.IsDeleted);
            var inactiveUsers = totalUsers - activeUsers;

            return new UserStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                InactiveUsers = inactiveUsers
            };
        }


    }
}
