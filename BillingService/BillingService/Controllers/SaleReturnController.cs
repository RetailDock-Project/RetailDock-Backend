using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReturnController : ControllerBase
    {
        private readonly ISaleReturnService saleReturnService;
        public SaleReturnController(ISaleReturnService _saleReturnService)
        {
            saleReturnService = _saleReturnService;
        }
        [HttpGet("GetAllSaleReturn")]

        public async Task<IActionResult> GetAllSalesReturn(Guid orgId)
        {
            var result = await saleReturnService.GetAllSalesReturnDetails(orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnById")]

        public async Task<IActionResult> GetSalesReturnById(Guid orgId, Guid returnId)
        {
            var result = await saleReturnService.GetSalesReturnDetailsById(returnId, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnByInvoice")]

        public async Task<IActionResult> GetSalesReturnByInvoice(Guid orgId, string invoiceNum)
        {
            var result = await saleReturnService.GetSalesReturnDetailsByInvoice(invoiceNum, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnByDate")]
        public async Task<IActionResult> GetSalesReturnByDate(Guid orgId,DateTime fromDate,DateTime? toDate)
        {
            var result = await saleReturnService.GetSalesReturnByDate(fromDate,toDate, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("AddSaleReturn")]
        public async Task<IActionResult> AddSaleReturn(AddSalesReturnDto salesReturn, Guid orgId, Guid userId)
        {
            var result = await saleReturnService.AddSalesReturn(salesReturn, orgId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
