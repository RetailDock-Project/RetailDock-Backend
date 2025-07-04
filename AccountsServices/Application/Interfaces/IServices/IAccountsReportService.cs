using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Common;

namespace Application.Interfaces.IServices
{
    public interface IAccountsReportService
    {
        Task<ApiResponseDTO<PLAccountSummaryDto>> GetPLRawDataAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate);
        Task<ApiResponseDTO<BalanceSheetResponseDto>> GetBalanceSheetSummaryAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate);
        

        }
}
