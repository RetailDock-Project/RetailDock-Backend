using Application.DTOs;
using Application.Interfaces.Service_Interfaces;
using Application.PdfGenerator;
using Application.Services;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : BaseController
    {

        private readonly ISaleService saleService;
        public SaleController(ISaleService _saleService)
        {
            saleService = _saleService;
        }

        [HttpGet("GetAllSaleDetails")]

        public async Task<IActionResult> GetAllSalesDetails()
        {
            var result = await saleService.GetAllSalesDetails(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("cashRecievedFromDebtor")]

        public async Task<IActionResult> CashReceivedFromDebtor(Guid debtorId, decimal recievedAmount, decimal currentBalance)
        {
            
            var result = await saleService.CashReceivedFromDebtor(debtorId, recievedAmount,currentBalance, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetDebtorSaleDetails")]

        public async Task<IActionResult>  GetDebtorsSalesDetails(Guid debtorId)
        {
            var result = await saleService.GetDebtorsSalesDetails(debtorId,OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleById")]

        public async Task<IActionResult> GetSalesById( Guid saleId)
        {
            var result = await saleService.GetSalesDetailsById(saleId,  OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleByInvoice")]

        public async Task<IActionResult> GetSalesByInvoice( string invoiceNum)
        {
            var result = await saleService.GetSalesDetailsByInvoice(invoiceNum, OrgId);
            var document = new InvoiceDocument(result.Data);
            var pdfBytes = document.GeneratePdf();


            return File(pdfBytes, "application/pdf", $"Invoice_{result.Data.InvoiceNumber}.pdf");
        }
        [HttpGet("GetAllSaleByDate")]
        public async Task<IActionResult> GetSalesByDate( DateTime fromDate, DateTime? toDate)
        {
            var result = await saleService.GetSalesByDate(fromDate, toDate, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("AddNewSale")]
        public async Task<IActionResult> AddNewSale(SalesAddDto sales)
        {
            var result = await saleService.AddNewSale(sales, OrgId, UserId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
