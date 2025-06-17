using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IHsnCodeRepository
    {
        Task<HsnCode>AddHsn(HsnCode hsnCode);
        Task<List<HsnCode>> GetAllHsnCodes(Guid OrganaiztionId);
        Task<HsnCode> GetByHsnCode(int hsnCodeNumber);
        Task<HsnCode>UpdateHsn(HsnCode hsnCode);
        Task<bool> DeleteHsnCode(int hsnCode);
        Task<HsnCode> GetByHsnCodeAndOrg(Guid organizationId, int hsnCodeNumber);
    }
}
