using BillingService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductConsumerService _productService;

        public ProductController(ProductConsumerService productService)
        {
            _productService = productService;
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetProducts(Guid orgId)
        {
            var products = await _productService.GetProductsByOrganizationIdAsync(orgId);

            if (products.Count == 0)
            {
                return NotFound("No products found.");
            }

            return Ok(products);
        }
    }
}
