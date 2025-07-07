using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;

namespace Application.Interfaces.IRepository
{
    public interface IAccountsReportRepository
    {
        Task<(GrossProfitDto Gross, ProfitAndLossDto PL)> GetPLRawDataAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate);
        Task<BalanceSheetResponseDto> GetBalanceSheetSummaryAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate);
    }
}
