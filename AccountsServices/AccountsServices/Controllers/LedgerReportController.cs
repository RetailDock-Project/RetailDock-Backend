using System;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerReportController : ControllerBase
    {
        private readonly ILedgerReportServices _services;
        public LedgerReportController(ILedgerReportServices services)
        {
            _services = services;
        }
        [HttpGet("ledger/report/byid")]
        public async Task<IActionResult>GetLedgerReports(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDateid)
        {
            var result= await _services.GetLedgerDetailsAsync(organizationId, ledgerId, startDate, endDateid);
            return StatusCode(result.StatusCode, result);
                 
        }
        [HttpGet("all/ledger/report")]
        public async Task<IActionResult> GetAllLedgerReports(Guid organizationId,  DateTime? startDate, DateTime? endDateid)
        {
            var result = await _services.GetAllLedgerSummariesAsync(organizationId,  startDate, endDateid);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("all/ledger/report/bygroup")]
        public async Task<IActionResult> GetAllLedgerReportsByGroupId(Guid groupId, Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            var result = await _services.GetLedgerSummaryByGroupAsync(groupId,organizationId,startDate,endDate);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("all/ledgerandgroup/report/bygroupid")]
        public async Task<IActionResult> GetAllLedgersAndGroupReportsByGroupId(Guid groupId, Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            var result = await _services.GetGroupLedgerSummaryAsync(groupId, organizationId, startDate, endDate);
            return StatusCode(result.StatusCode, result);

        }
    }
}
