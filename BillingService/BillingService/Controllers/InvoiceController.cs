using Application.DTOs;
using Application.Interfaces.Service_Interfaces;
using Application.Services;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : BaseController
    {
        private readonly I_InvoiceService invoiceService;
        public InvoiceController(I_InvoiceService _invoiceService)
        {
            invoiceService = _invoiceService;
        }

        [HttpGet("GetAllSaleInoices")]
    public async Task<IActionResult> AllSaleInvoices( bool? isFullData, int? skip, int? take)
        {
            var result = await invoiceService.getAllSaleInvoices(OrgId, UserId, isFullData, skip, take);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("getSaleInvoiceNumber")]
        public async Task<IActionResult> SaleInvoicesByInvoiceNumber( string invoiceNum)
        {
            var result = await invoiceService.getSaleInvoicesByInvoiceNumber(OrgId,invoiceNum);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("getSaleInvoiceByDate")]
        public async Task<IActionResult> SaleInvoicesByDate(bool? isFullData, DateTime? fromdate, DateTime todate)
        {
            var result = await invoiceService.getSaleInvoicesByDate(OrgId, UserId, isFullData, fromdate, todate);
            return StatusCode(result.StatusCode, result);
        }
      
        [HttpGet("getsaleInvoiceByDueDate")]
        public async Task<IActionResult> SaleInvoicesByDueDate(bool? isFullData, DateTime? fromdate, DateTime todate)
        {
            var result = await invoiceService.getSaleInvoicesByDueDate(OrgId, UserId, isFullData, fromdate, todate);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("pendingSaleinvoices")]
        public async Task<IActionResult> PendingSalesInvoices(bool? isFullData, DateTime? fromDate, DateTime? toDate)
        {
            var result = await invoiceService.getPendingSalesInvoices(OrgId, UserId, isFullData, fromDate, toDate);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("getallSaleReturnInvoice")]
        public async Task<IActionResult> AllSaleReturnInvoices( bool? isFullData, int? skip, int? take)
        {
            var result = await invoiceService.getAllSaleReturnInvoices(OrgId, UserId, isFullData, skip, take);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("getsaleReturnByInvoiceNum")]

        public async Task<IActionResult> SaleReturnByInvoiceNumber( string invoiceNum)
        {
            var result = await invoiceService.getSaleReturnByInvoiceNumber(OrgId, invoiceNum);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("getSaleReturnInvoiceBydate")]

        public async Task<IActionResult> SaleReturnInvoicesByDate(bool? isFullData, DateTime? fromDate, DateTime toDate)
        {
            var result = await invoiceService.getSaleReturnInvoicesByDate(OrgId, UserId, isFullData, fromDate, toDate);
            return StatusCode(result.StatusCode, result);

        }
        [HttpGet("getInvoiceDetails")]
        public async Task<IActionResult> getdetailsByInvoiceNumber(string invoiceNum)
        {
            if (invoiceNum.Contains("SR"))
            {
                var response = await invoiceService.getSaleReturnByInvoiceNumber(OrgId, invoiceNum);
                return StatusCode(response.StatusCode, response);
            }
            var result = await invoiceService.getSaleInvoicesByInvoiceNumber(OrgId, invoiceNum);
            return StatusCode(result.StatusCode, result);
        }

    }
}
