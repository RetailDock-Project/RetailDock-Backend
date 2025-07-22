using System;
using AccountsServices.Controllers.BaseControllers;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerReportController : BaseController
    {
        private readonly ILedgerReportServices _services;
        public LedgerReportController(ILedgerReportServices services)
        {
            _services = services;
        }
        [HttpGet("ledger/report/byid")]
        public async Task<IActionResult>GetLedgerReports( Guid ledgerId, DateTime? startDate, DateTime? endDateid)
        {
            var result= await _services.GetLedgerDetailsAsync(OrgId, ledgerId, startDate, endDateid);
            return StatusCode(result.StatusCode, result);
                 
        }
        [HttpGet("all/ledger/report")]
        public async Task<IActionResult> GetAllLedgerReports(  DateTime? startDate, DateTime? endDateid)
        {
            var result = await _services.GetAllLedgerSummariesAsync(OrgId,  startDate, endDateid);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("all/ledger/report/bygroup")]
        public async Task<IActionResult> GetAllLedgerReportsByGroupId(Guid groupId, DateTime? startDate, DateTime? endDate)
        {
            var result = await _services.GetLedgerSummaryByGroupAsync(groupId, OrgId, startDate,endDate);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("all/ledgerandgroup/report/bygroupid")]
        public async Task<IActionResult> GetAllLedgersAndGroupReportsByGroupId(Guid groupId,  DateTime? startDate, DateTime? endDate)
        {
            var result = await _services.GetGroupLedgerSummaryAsync(groupId, OrgId, startDate, endDate);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("ledger/report/closinbalance/byid")]
        public async Task<IActionResult> GetLedgerClosingBalanceById(Guid OrgIdS, Guid ledgerId, DateTime? startDate, DateTime? endDateid)
        {
            var result = await _services.GetLedgerSummariesAsyncByLedgerId(OrgIdS  ,startDate, endDateid,ledgerId);
            return StatusCode(result.StatusCode, result);

        }
    }
}
