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
            await context.SaveChangesAsync();
            Console.WriteLine("updated orgrole");

        }

        public async Task AddUserOrgRole(UserOrganizationRole newUserRole) {
            await context.UserOrganizationRoles.AddAsync(newUserRole);
            await context.SaveChangesAsync();
            Console.WriteLine("updated user prgrole");
        }

        public async Task<List<OrganizationUserDto>> GetUsersByOrgId(Guid orgId) {
            //var res=await context.Users.Include(u => u.userOrganizationRole).ThenInclude(uor => uor.OrganizationRole).ThenInclude(or => or.Role).Where(u=>u.OrganisationId.ToString()==orgId && u.userOrganizationRole.OrganizationRole.RoleId!=4).ToListAsync();
            //return res;

            //var res = await context.Users.Include(u => u.userOrganizationRole).ThenInclude(uor => uor.OrganizationRole).ThenInclude(or => or.Role).Where(u => u.OrganisationId.ToString() == orgId).Select(u => new OrganizationUserDto { Name = u.Name, Email = u.Email, Roles = u.userOrganizationRole.Where(x => x.OrganizationRole.OrganizationId.ToString() == orgId).ToList() }).ToListAsync();

            var res = await context.Users
    .Include(u => u.userOrganizationRole)
        .ThenInclude(uor => uor.OrganizationRole)
            .ThenInclude(or => or.Role)
    .Where(u => u.OrganisationId == orgId)
    .Select(u => new OrganizationUserDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Roles = u.userOrganizationRole
                    .Where(x => x.OrganizationRole.OrganizationId == orgId)
                    .Select(x => x.OrganizationRole.Role.Name)
                    .ToList(),
        Created=u.CreatedAt
    })
    .ToListAsync();


            return res;
        }
    }
}
