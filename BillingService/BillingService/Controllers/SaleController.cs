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

        public async Task<IActionResult> GetAllSalesDetails( bool? isFullData, int? skip, int? take)
        {
            var result = await saleService.GetAllSalesDetails(OrgId,UserId,isFullData,skip,take);
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
        [HttpGet("GetSaleById")]

        public async Task<IActionResult> GetSalesById( Guid saleId)
        {
            var result = await saleService.GetSalesDetailsById(saleId,  OrgId);
            return StatusCode(result.StatusCode, result);
        } 
        
        [HttpGet("GetB2BSaleInvoice")]

        public async Task<IActionResult> GetB2BSaleInvoiceNumber( )
        {

            var result = await saleService.GenerateB2BInvoiceNumber(  OrgId);
            return StatusCode(result.StatusCode, result);
        }  
          [HttpGet("GetB2CSaleInvoice")]

        public async Task<IActionResult> GetB2CSaleInvoiceNumber( )
        {

            var result = await saleService.GenerateB2CInvoiceNumber(  OrgId);
            return StatusCode(result.StatusCode, result);
        }  
        [HttpGet("GetsaleBill")]

        public async Task<IActionResult> GetSalesBill( string invoiceNum)
        {
            var result = await saleService.GetSalesDetailsByInvoice(invoiceNum, OrgId);
            var document = new InvoiceDocument(result.Data);
            var pdfBytes = document.GeneratePdf();


            return File(pdfBytes, "application/pdf", $"Invoice_{result.Data.InvoiceNumber}.pdf");
        }
        [HttpGet("GetsaleByInvoice")]
        public async Task<IActionResult> GetSalesByInvoice( string invoiceNum)
        {
            var result = await saleService.GetSalesDetailsByInvoice(invoiceNum, OrgId);
            return StatusCode(result.StatusCode, result);
        } 
        [HttpGet("GetsaleInvoiceNumber")]
        public async Task<IActionResult> getSaleInvoiceNumber( string saleMode)
        {
            if(saleMode == "B2C")
            {
                var result = await saleService.GenerateB2CInvoiceNumber( OrgId);
                return StatusCode(result.StatusCode, result);
            }
            else
            {
                var result = await saleService.GenerateB2BInvoiceNumber(OrgId);
                return StatusCode(result.StatusCode, result);
            }
   
        }
        [HttpGet("GetAllSaleByDate")]
        public async Task<IActionResult> GetSalesByDate( DateTime? fromDate, DateTime? toDate,bool? isFulldata,int? skip,int? take)
        {
            var result = await saleService.GetSalesByDate(fromDate, toDate, OrgId,UserId,isFulldata,skip,take);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("AddNewSale")]
        public async Task<IActionResult> AddNewSale(SalesAddDto sales)
        {
            var result = await saleService.AddNewSale(sales, OrgId, UserId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("getSaleTaxReport")]

        public async Task<IActionResult> getSalesDetailsWithTaxReport(DateTime? fromDate, DateTime? toDate,  int? skip, int? take)
        {
            var result = await saleService.getSalesDetailsWithTaxReport(fromDate,toDate, OrgId, UserId,skip,take);
            return StatusCode(result.StatusCode, result);
        }
    }
}
