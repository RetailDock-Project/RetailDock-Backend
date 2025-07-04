using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsReportController : ControllerBase
    {
        private readonly IAccountsReportService _accountsReportService;
        public AccountsReportController(IAccountsReportService accountsReportService)
        {
            _accountsReportService = accountsReportService;
        }
        [HttpGet("get/pandl/account")]
        public async Task <IActionResult> GetPandLAccount(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            var data= await _accountsReportService.GetPLRawDataAsync(organizationId, fromDate, toDate);
            return StatusCode(data.StatusCode, data);
        }
        [HttpGet("get/balacesheet/report")]
        public async Task<IActionResult> GetBalanceSheet(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            var data = await _accountsReportService.GetBalanceSheetSummaryAsync(organizationId, fromDate, toDate);
            return StatusCode(data.StatusCode, data);
        }
    }
}
