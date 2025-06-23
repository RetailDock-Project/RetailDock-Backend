using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTOs;
using Common;

namespace Application.Interfaces.IServices
{
    public interface ILedgerReportServices
    {
        Task<ApiResponseDTO<LedgerDetailsReportDTO>> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate);
        Task<ApiResponseDTO<List<LedgerSummaryDTO>>> GetAllLedgerSummariesAsync(Guid organizationId, DateTime? startDate, DateTime? endDate);
    }
}
