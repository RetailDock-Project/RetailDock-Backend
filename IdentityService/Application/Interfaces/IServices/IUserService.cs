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
        Task<ResponseDto<List<OrganizationUserDto>>> GetUsersByOrgId(Guid orgId);   }
}
