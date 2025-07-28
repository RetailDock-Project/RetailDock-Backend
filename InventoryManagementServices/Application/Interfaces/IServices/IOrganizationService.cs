using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;

namespace Application.Interfaces.IServices
{
    public interface IOrganizationService
    {
        Task<OrganizationDetailsDto> GetOrganizationByIdAsync(Guid id);
    }
}
