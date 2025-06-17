using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IPurchaseOrderService _service;

        public PurchaseOrderController(IPurchaseOrderService service)
        {
            _service = service;
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder( [FromBody] AddPurchaseOrderDto dto ,Guid orgnaizationId)
        {
            var result = await _service.AddPurchaseOrderAsync(orgnaizationId, dto);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllOrders(Guid orgnaizationId)
        {
            var result = await _service.GetAllOrdersAsync(orgnaizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var result = await _service.GetOrderByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        {
            var result = await _service.UpdateOrderStatusAsync(id, dto);
            return StatusCode(result.StatusCode, result);
        }
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

    }
}
