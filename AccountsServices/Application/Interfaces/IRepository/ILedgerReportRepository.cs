using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTOs;

namespace Application.Interfaces.IRepository
{
    public interface ILedgerReportRepository
    {
        Task<LedgerDetailsReportDTO> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate);
        Task<List<LedgerSummaryDTO>> GetAllLedgerSummariesAsync(Guid organizationId, DateTime? startDate, DateTime? endDate);
    }
}
