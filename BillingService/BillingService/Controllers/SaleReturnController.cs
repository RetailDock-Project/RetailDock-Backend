using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleReturnController : BaseController
    {
        private readonly ISaleReturnService saleReturnService;
        public SaleReturnController(ISaleReturnService _saleReturnService)
        {
            saleReturnService = _saleReturnService;
        }
        [HttpGet("GetAllSaleReturn")]

        public async Task<IActionResult> GetAllSalesReturn()
        {
            var result = await saleReturnService.GetAllSalesReturnDetails(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnById")]

        public async Task<IActionResult> GetSalesReturnById( Guid returnId)
        {
            var result = await saleReturnService.GetSalesReturnDetailsById(returnId,OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnByInvoice")]

        public async Task<IActionResult> GetSalesReturnByInvoice( string invoiceNum)
        {
            var result = await saleReturnService.GetSalesReturnDetailsByInvoice(invoiceNum, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleReturnByDate")]
        public async Task<IActionResult> GetSalesReturnByDate(DateTime fromDate,DateTime? toDate)
        {
            var result = await saleReturnService.GetSalesReturnByDate(fromDate,toDate, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("AddSaleReturn")]
        public async Task<IActionResult> AddSaleReturn(AddSalesReturnDto salesReturn)
        {
            var result = await saleReturnService.AddSalesReturn(salesReturn, OrgId, UserId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
