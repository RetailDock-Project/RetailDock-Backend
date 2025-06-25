using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTOs;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Common;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Services.AccountsService
{
    public class LedgerReportService : ILedgerReportServices
    {
        private readonly ILogger<LedgerReportService> _logger;
        private readonly ILedgerReportRepository _ledgerReportRepository;
        public LedgerReportService(ILogger<LedgerReportService> logger, ILedgerReportRepository ledgerReportRepository)
        {
            _logger = logger;
            _ledgerReportRepository = ledgerReportRepository;
        }
        public async Task<ApiResponseDTO<LedgerDetailsReportDTO>> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate)
        {
            try
            {


                var data = await _ledgerReportRepository.GetLedgerDetailsAsync(organizationId, ledgerId, startDate, endDate);
                if (data != null)
                {
                    return new ApiResponseDTO<LedgerDetailsReportDTO>
                    {
                        StatusCode = 200,
                        Message = "Ledger Report Fetched Succussfylly",
                        Data = data

                    };
                }
                return new ApiResponseDTO<LedgerDetailsReportDTO>
                {
                    StatusCode = 404,
                    Message = "LedgerId or Organization Not Found"

                };
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message, "Error in fetching details");
                return new ApiResponseDTO<LedgerDetailsReportDTO>
                {
                    StatusCode = 500,
                    Message = "Error in fetching details"
                };

            }
        }

        public async Task<ApiResponseDTO<List<LedgerSummaryDTO>>> GetAllLedgerSummariesAsync(Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var summaries = await _ledgerReportRepository.GetAllLedgerSummariesAsync(organizationId, startDate, endDate);

                return new ApiResponseDTO<List<LedgerSummaryDTO>>
                {
                    StatusCode = 200,
                    Message = "Ledger summaries fetched successfully",
                    Data = summaries
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error occurred while fetching ledger summaries for OrgId: {OrgId}", organizationId);

                return new ApiResponseDTO<List<LedgerSummaryDTO>>
                {
                    StatusCode = 500,
                    Message = "Failed to fetch ledger summaries",

                };
            }
        }
    public async Task <ApiResponseDTO<List<LedgerSummaryDTO>>> GetLedgerSummaryByGroupAsync(Guid groupId, Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            try
            {


                var data = await _ledgerReportRepository.GetLedgerSummaryByGroupHierarchyAsync(groupId, organizationId, startDate, endDate);
                if (data != null)
                {
                    return new ApiResponseDTO<List<LedgerSummaryDTO>>
                    {
                        StatusCode = 200,
                        Message = "Ledgers Fetched Succusfully",
                        Data = data
                    };
                }
                return new ApiResponseDTO<List<LedgerSummaryDTO>>
                {
                    StatusCode = 200,
                    Message = "No ledger found this group or organization"
                };
            }
            catch(Exception ex)
            {

                _logger
                    .LogError(ex.Message, "Error in getting Ledgers");
                return new ApiResponseDTO<List<LedgerSummaryDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in getting Ledgers"
                };
            }
        }
        public async Task<ApiResponseDTO<GroupWithLedgersSummaryDTO>> GetGroupLedgerSummaryAsync(Guid groupId, Guid organizationId, DateTime? start, DateTime? end)
        {
            try
            {
                var result = await _ledgerReportRepository.GetGroupAndLedgerSummaryAsync(groupId, organizationId, start, end);
                if(result != null)
                {
                    return new ApiResponseDTO<GroupWithLedgersSummaryDTO>
                    {
                        StatusCode = 200,
                        Message = "Ledgers and group Fetched Succusfully",
                        Data = result
                    };
                }
                return new ApiResponseDTO<GroupWithLedgersSummaryDTO>
                {
                    StatusCode = 200,
                    Message = "No ledger found this group or organization"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Service error");
                return new ApiResponseDTO<GroupWithLedgersSummaryDTO>
                {
                    StatusCode = 500,
                    Message = "Error in getting Ledgers"
                };
            }
        }

    }
}
