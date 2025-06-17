using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryServices _services;

        public ProductCategoryController(IProductCategoryServices services)
        {
            _services = services;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddProductCategory([FromBody] ProductCategoryDto productCategoryDto)
        {
            try
            {
                var result = await _services.AddproductCategory(productCategoryDto);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllCategories(Guid OrganaiztionId)
        {
            try
            {
                var result = await _services.GetAllCategoryProducts(OrganaiztionId);
        
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetBYId")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var result = await _services.GetCategoryById(id);
                if (result.Data == null)
                    return NotFound(result.Message);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] ProductCategoryDto productCategoryDto)
        {
            try
            {
                var result = await _services.UpdateProductCategory(id,productCategoryDto);
                if (result.StatusCode == 404)
                    return NotFound(result.Message);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _services.DeleteProductCategory(id);
                if (result.StatusCode == 404)
                    return NotFound(result.Message);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
