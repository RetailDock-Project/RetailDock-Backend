using API.Controllers.Base;
using Application.Dto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : BaseController
    {
        private readonly ISupplierService supplierService; 
        public SupplierController(ISupplierService _supplierService) {
            supplierService = _supplierService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupplier(SupplierDto newSupplier) {
            var response = await supplierService.CreateSupplier(newSupplier, OrgId, UserId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{supplierId}/remove")]
        public async Task<IActionResult> RemoveSupplier(Guid supplierId,Guid orgId,Guid userId)
        {
            var response = await supplierService.RemoveSupplier(supplierId, orgId, userId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSuppliersByOrgId(Guid orgId)
        {
            var response = await supplierService.GetAllSuppliersByOrganizationId(orgId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("supplier-filter")]
        public async Task<IActionResult> GetSuppliersByFilter(
        [FromQuery] string? search,
        [FromQuery] bool? isActive,
        [FromQuery] int? pageNumber,
        [FromQuery] int? pageSize)
        {
            var result = await supplierService.GetSuppliersByFilterAsync(OrgId, search, isActive, pageNumber, pageSize);
            return StatusCode(result.StatusCode, result);
        }

    }
}
