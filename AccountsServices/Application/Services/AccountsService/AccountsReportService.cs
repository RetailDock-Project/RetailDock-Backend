using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Common;
using Microsoft.Extensions.Logging;

namespace Application.Services.AccountsService
{
    public class AccountsReportService:IAccountsReportService
    {
        private readonly IAccountsReportRepository _accountsReportRepository;
        private readonly ILogger<AccountsReportService> _logger;
      
        public AccountsReportService(IAccountsReportRepository accountsReport,ILogger<AccountsReportService> logger)
        {
            _accountsReportRepository = accountsReport;
           _logger = logger;
        }
        public async Task<ApiResponseDTO<PLAccountSummaryDto>> GetPLRawDataAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                if (organizationId == Guid.Empty)
                {
                    return new ApiResponseDTO<PLAccountSummaryDto>
                    {
                        StatusCode = 400,
                        Message = "OrganizationId is Required"
                    };
                }


                var (gross, pl) = await _accountsReportRepository.GetPLRawDataAsync(organizationId, fromDate, toDate);
                if (gross == null || pl == null)
                {
                    return new ApiResponseDTO<PLAccountSummaryDto>
                    {
                        StatusCode = 200,
                        Message = "No data fouund"
                    };

                }
                var data = new PLAccountSummaryDto
                {
                    GrossProfitSection = gross,
                    PLAcoountsSection = pl,

                    NetProfit =( gross.GrossProfit??0) + (pl.NetIndirectIncome??0 )- (pl.NetIndirectExpense??0),
                };
                return new ApiResponseDTO<PLAccountSummaryDto>
                {
                    StatusCode = 200,
                    Message = "P&L Succussfully Fetched",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in profit and loss account fetching");
                return new ApiResponseDTO<PLAccountSummaryDto>
                {
                    StatusCode = 500,
                    Message = "Error in profit and loss account fetching"
                };
            }

        } 
        public async Task<ApiResponseDTO<BalanceSheetResponseDto>> GetBalanceSheetSummaryAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                if (organizationId == Guid.Empty)
                {
                    return new ApiResponseDTO<BalanceSheetResponseDto>
                    {
                        StatusCode = 400,
                        Message = "OrganizationId is Required"
                    };
                }
                var (gross, pl) = await _accountsReportRepository.GetPLRawDataAsync(organizationId, fromDate, toDate);
                if (gross == null || pl == null)
                {
                    return new ApiResponseDTO<BalanceSheetResponseDto>
                    {
                        StatusCode = 200,
                        Message = "No data fouund gross and pl account"
                    };

                }
                var data = await _accountsReportRepository.GetBalanceSheetSummaryAsync(organizationId, fromDate, toDate);
                if (data == null)
                {
                    return new ApiResponseDTO<BalanceSheetResponseDto>
                    {
                        StatusCode = 200,
                        Message = "No data fouund balance sheet"
                    };

                }
                var adjustedProfit = new BalanceSheetResponseDto
                {
                    Items = data.Items,
                    TotalAssets = data.TotalAssets,
                    TotalLiabilities = data.TotalLiabilities+ (gross.GrossProfit ?? 0) + (pl.NetIndirectIncome ?? 0) - (pl.NetIndirectExpense ?? 0),
                    NetProfit = (gross.GrossProfit ?? 0) + (pl.NetIndirectIncome ?? 0) - (pl.NetIndirectExpense ?? 0),
                };



                return new ApiResponseDTO<BalanceSheetResponseDto>
                {
                    StatusCode = 200,
                    Message = "Balance sheet Fetched Succussfully",
                    Data = adjustedProfit,
                };




            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in Balance sheet Calculation");
                return new ApiResponseDTO<BalanceSheetResponseDto>
                {
                    StatusCode = 500,
                    Message = "Error in Balance sheet Calculation",

                };
            
            }

        }

    }
}
