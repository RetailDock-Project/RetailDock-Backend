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
    }
}
