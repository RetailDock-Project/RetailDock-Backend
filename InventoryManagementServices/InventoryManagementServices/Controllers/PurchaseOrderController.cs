using API.Controllers.Base;
using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : BaseController
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrderController(IPurchaseOrderService service)
        {
            _service = service;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder( [FromBody] AddPurchaseOrderDto dto )
        {
            var result = await _service.AddPurchaseOrderAsync(OrgId,UserId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _service.GetAllOrdersAsync(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        //[HttpPut("{id}/status")]
        //public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        //{
        //    var result = await _service.UpdateOrderStatusAsync(id, dto);
        //    return StatusCode(result.StatusCode, result);
        //}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var result = await _service.DeleteOrderAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}/export-pdf")]
        public async Task<IActionResult> ExportPurchaseOrderPdf(Guid id)
        {
            try
            {
                var pdfBytes = await _service.ExportPurchaseOrderPdfBytesAsync(id);

                return File(
                    fileContents: pdfBytes,
                    contentType: "application/pdf",
                    fileDownloadName: $"PurchaseOrder_{id}.pdf"
                );
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }



        [HttpGet("all/filters")]
        public async Task<IActionResult> GetPurchaseOrdersWithFilters(
    [FromQuery] string? searchString,
    [FromQuery] string? status,
    [FromQuery] DateTime? startDate,
    [FromQuery] DateTime? endDate,
    [FromQuery] int? pageNumber,
    [FromQuery] int? pageSize )
        {
            var result = await _service.GetAllOrdersAsync(OrgId, searchString, status, startDate, endDate, pageNumber, pageSize);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPurchaseOrderStats()
        {
            var result = await _service.GetOrderStatsAsync(OrgId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateOrder([FromBody] UpdatePurchaseOrderDto dto)
        {
            var result = await _service.UpdatePurchaseOrderAsync(OrgId, UserId, dto);
            return StatusCode(result.StatusCode, result);
        }


    }
}
