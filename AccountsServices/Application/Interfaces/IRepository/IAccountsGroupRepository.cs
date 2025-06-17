using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interfaces.IRepository
{
    public interface IAccountsGroupRepository
    {
        Task<bool> AddParentGroup(Guid OrganizationId, AddParentGroupDTO addGroupDTO);
        Task<bool> AddSubGroup(Guid OrganizationId, AddSubGroupDTO addGroupDTO);
        Task<bool> IsGroupNameExists(Guid organizationId, string groupName);
        Task<List<GetParentGroupsDTO>> GetParentGroups(Guid OrganizationId);
        Task<List<GetSubGroupsDTO>> GetSubGroups(Guid organizationId);
        Task<bool> createGroupDefault(Guid OrganizationId, Guid CreatedBy);
 
    }
}
