using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces.IRepositories
{
    public interface IUserRepository
    {
        Task<bool> UpdateUserOrganization(Guid userId, Guid orgId);
        Task AddOrganizationRole(OrganizationRole newRole);
        Task AddUserOrgRole(UserOrganizationRole newUserRole);
        //Task<User> GetUsersByOrgId(string orgId);
        Task<List<OrganizationUserDto>> GetUsersByOrgId(Guid orgId);
        Task<User> GetUserById(Guid userId);
        Task<UserOrganizationRole> GetUserOrganizationRoleAsync(Guid userId);
        Task UpdateUserOrganizationRoleAsync(UserOrganizationRole userOrgRole);
        Task UpdateUserAsync(User user);
        Task<List<OrganizationUserDto>> GetFilteredUsersByOrgId(
     Guid orgId, string? search, Guid? roleId, Guid? userId);

        Task<UserStatsDto> GetUserStatsByOrgId(Guid orgId);
    }
}
