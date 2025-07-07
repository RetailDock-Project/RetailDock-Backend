using AccountsServices.Controllers.BaseControllers;
using Application.DTO;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : BaseController
    {
        private readonly IVoucherService _voucherService;
        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }
        [HttpPost("add/new/voucherentry")]
        public async Task<IActionResult> AddAVoucherEntry( AddVouchersDTO addVoucherDTO)
        {
            var result= await _voucherService.AddVoucherEntrys(OrgId, UserId, addVoucherDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/voucher/report")]
        public async Task<IActionResult> GetVoucherTransactionReports(Guid voucherTypeId, DateTime? fromDate, DateTime? toDate)
        {
            var result = await _voucherService.GetTransactionsByVoucherTypeAsync(voucherTypeId, OrgId, fromDate, toDate);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/vouchertypes")]
        public async Task<IActionResult> GetAllVoucherTypes()
        {
            var result = await _voucherService.GetAllVoucherTypes();
            return StatusCode(result.StatusCode, result);
        }
    }
}
