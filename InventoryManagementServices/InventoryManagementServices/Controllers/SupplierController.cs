using Application.Dto;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService supplierService; 
        public SupplierController(ISupplierService _supplierService) {
            supplierService = _supplierService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSupplier(SupplierDto newSupplier,Guid orgId,Guid userId) {
            var response = await supplierService.CreateSupplier(newSupplier, orgId, userId);
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

    }
}
