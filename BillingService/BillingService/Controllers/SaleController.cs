using Application.DTOs;
using Application.Interfaces.Service_Interfaces;
using Application.PdfGenerator;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {

        private readonly ISaleService saleService;
        public SaleController(ISaleService _saleService)
        {
            saleService = _saleService;
        }

        [HttpGet("GetAllSaleDetails")]

        public async Task<IActionResult> GetAllSalesDetails(Guid orgId)
        {
            var result = await saleService.GetAllSalesDetails(orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleById")]

        public async Task<IActionResult> GetSalesById(Guid orgId, Guid saleId)
        {
            var result = await saleService.GetSalesDetailsById(saleId, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("GetAllSaleByInvoice")]

        public async Task<IActionResult> GetSalesByInvoice(Guid orgId, string invoiceNum)
        {
            var result = await saleService.GetSalesDetailsByInvoice(invoiceNum, orgId);
            var document = new InvoiceDocument(result.Data);
            var pdfBytes = document.GeneratePdf();


            return File(pdfBytes, "application/pdf", $"Invoice_{result.Data.InvoiceNumber}.pdf");
        }
        [HttpGet("GetAllSaleByDate")]
        public async Task<IActionResult> GetSalesByDate(Guid orgId, DateTime fromDate, DateTime? toDate)
        {
            var result = await saleService.GetSalesByDate(fromDate, toDate, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("AddNewSale")]
        public async Task<IActionResult> AddNewSale(SalesAddDto sales, Guid orgId, Guid userId)
        {
            var result = await saleService.AddNewSale(sales, orgId, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
