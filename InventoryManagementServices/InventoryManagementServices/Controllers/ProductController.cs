using Application.Dto;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductServices _services;

        public ProductController(IProductServices services)
        {
            _services = services;
        }
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDto productDto)
        {
            var result = await _services.AddProduct(productDto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPut("update/product")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] ProductDto productDto)
        {
            var result = await _services.UpdateProduct(id, productDto);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/product/byid")]
        public async Task<IActionResult> ProductGetById(Guid productId)
        {
            var result = await _services.GetProductById(productId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/product/all")]
        public async Task<IActionResult> GetAllProduct(Guid organizationId)
        {
            var result = await _services.GetAllProducts(organizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("Get/Product/Billing/All")]
        public async Task<IActionResult> GetAllProductForBilling(Guid orgnaizationId)
        {
            var result = await _services.GetAllProductsForBilling(orgnaizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("delete/product/byId")]
        public async Task<IActionResult> DeleteProductById(Guid productId)
        {
            var result = await _services.DeleteProduct(productId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/low/stock")]
        public async Task<IActionResult> GetLowStockItems(Guid organizationId)
        {
            var result = await _services.GetLowStockItems(organizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("Search")]
        public async Task<IActionResult> Serach(Guid organizationId, int? categoryId, string searchTerm)
        {
            var result = await _services.SearchProducts(organizationId, categoryId, searchTerm);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/product/bycategory")]
        public async Task<IActionResult> GetProductsByCategory(int categoryId, Guid organizationId)
        {
            var result = await _services.GetProductByCategory(categoryId, organizationId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("export/{organizationId}/products/excel")]
        public async Task<IActionResult> ExportProducts(Guid organizationId)
        {
            try
            {
                var excelBytes = await _services.ExportProductsAsExcelAsync(organizationId);
                return File(
                    fileContents: excelBytes,
                    contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileDownloadName: "ProductList.xlsx"
                );
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Export failed" });
            }
        }


        [HttpGet("Statistics")]
        public async Task<IActionResult> GetStatistics(Guid organaizationId)
        {
            var response = await _services.GetProductStatistics(organaizationId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("stock/{productId}/history")]
        public async Task<IActionResult> GetProductHistory(Guid productId) {
            var response= await _services.GetProductHistory(productId);

            return StatusCode(response.StatusCode,response);

        }
    }
}
