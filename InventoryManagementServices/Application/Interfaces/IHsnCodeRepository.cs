using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IHsnCodeRepository
    {
        Task<HsnCode>AddHsn(HsnCode hsnCode);
        Task<List<HsnCode>> GetAllHsnCodes(Guid OrganaiztionId);
        Task<HsnCode> GetByHsnCode(int hsnCodeNumber);
        Task<HsnCode> GetByHsnCodeAndOrg(Guid organizationId, string hsnCodeNumber);
        Task<HsnCode>UpdateHsn(HsnCode hsnCode);
        Task<bool> DeleteHsnCode(int hsnCode);
        Task<InvoiceSummeryDTO> GetInvoiceSummary(Guid orgId, DateTime? fromDate, DateTime? toDate, InvoiceType invoiceType);    
    }
}
