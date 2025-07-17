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

        public async Task<IActionResult> GetAllSalesReturn(bool isFulldata,int? skip,int take)
        {
            var result = await saleReturnService.GetAllSalesReturnDetails(OrgId,UserId,isFulldata,skip,take);
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
        public async Task<IActionResult> GetSalesReturnByDate(DateTime fromDate,DateTime? toDate,bool? isFullData)
        {
            var result = await saleReturnService.GetSalesReturnByDate(fromDate,toDate,isFullData, OrgId,UserId);
            return StatusCode(result.StatusCode, result);
        }     
        [HttpGet("GetAllSaleReturnTaxReport")]
        public async Task<IActionResult> GetSalesReturnTaxReport(DateTime? fromDate,DateTime? toDate)
        {
            var result = await saleReturnService.GetSalesReturnTaxReport(fromDate,toDate, OrgId,UserId);
            return StatusCode(result.StatusCode, result);
        } 
        [HttpGet("GetreturnedProductCount")]
        public async Task<IActionResult> GetreturnedProductCount(Guid saleId,Guid productId)
        {
            var result = await saleReturnService.getReturnedProductCount(saleId,productId, OrgId);
            return StatusCode(result.StatusCode, result);
        } 
        [HttpGet("GetsalesReturnInvoice")]
        public async Task<IActionResult> GetSalereturnInvoiceNumber(string saleMode)
        {
            if (saleMode == "B2C")
            {
                var result = await saleReturnService.GenerateB2CReturnInvoiceNumber(OrgId);
                return StatusCode(result.StatusCode, result);
            }else
            {

                var result = await saleReturnService.GenerateB2BReturnInvoiceNumber(OrgId);
                return StatusCode(result.StatusCode, result);
            }
  

        }
        [HttpPost("AddSaleReturn")]
        public async Task<IActionResult> AddSaleReturn(AddSalesReturnDto salesReturn)
        {
            var result = await saleReturnService.AddSalesReturn(salesReturn, OrgId, UserId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
