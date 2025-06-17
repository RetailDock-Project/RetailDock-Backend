using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Common;

namespace Application.Interfaces.IServices
{
    public interface IAccountsGroupService
    {
        Task<ApiResponseDTO<object>>CreateParentGroup(Guid OrganizationId,AddParentGroupDTO addGroupDTO);
        Task<ApiResponseDTO<object>> CreateSubGroup(Guid OrganizationId, AddSubGroupDTO addGroupDTO);

        Task<ApiResponseDTO<List<GetSubGroupsDTO>>> GetSubGroups(Guid organizationId);
        Task<ApiResponseDTO<List<GetParentGroupsDTO>> >GetParentGroups(Guid organizationId);
        Task<ApiResponseDTO<bool>> CreateDefaultGroups(Guid organizationId, Guid createdBy);
      




    }
}
