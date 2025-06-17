using Application.DTO;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpPost("add/new/voucherentry")]
        public async Task<IActionResult> AddAVoucherEntry(Guid organizationId, Guid CreatedBy, AddVouchersDTO addVoucherDTO)
        {
            var result= await _voucherService.AddVoucherEntrys(organizationId,CreatedBy,addVoucherDTO);
            return StatusCode(result.StatusCode, result);
        }
    }
}
