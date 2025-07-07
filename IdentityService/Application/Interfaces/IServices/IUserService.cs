using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common;

namespace Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task UpdateOrganizationIdandRole(Guid userId, Guid organizationId);
        Task<ResponseDto<List<OrganizationUserDto>>> GetUsersByOrgId(Guid orgId);

        Task<ResponseDto<UserDto>> GetUsersById(Guid userId);
        Task<ResponseDto<object>> UpdateUserOrganizationRoleAsync(UpdateUserRoleDto dto);
        Task<ResponseDto<object>> SoftDeleteUserAsync(Guid userId, Guid orgId);
        Task<ResponseDto<List<OrganizationUserDto>>> GetFilteredUsersByOrgId(
    Guid orgId, string? search, Guid? roleId, Guid? userId,Guid user);

        Task<ResponseDto<UserStatsDto>> GetUserStatsByOrgId(Guid orgId);
    }
}
