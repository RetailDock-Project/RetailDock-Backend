using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HsnCodeController : ControllerBase
    {
        private readonly IHsnCodeServices _hsnCodeServices;

        public HsnCodeController(IHsnCodeServices hsnCodeServices)
        {
            _hsnCodeServices = hsnCodeServices;
        }

        [HttpPost("Hsn&Gst")]
        public async Task<IActionResult>AddHsn(HsnDto hsnDto)
        {
            try
            {
                var result = await _hsnCodeServices.AddHsn(hsnDto);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get-All")]
        public async Task<IActionResult> GetAllHsn(Guid OrganaiztionId)
        {
            try
            {
                var result = await _hsnCodeServices.GetAllHsn(OrganaiztionId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetByHsnCode")]

        public async Task<IActionResult>GetByHsn(int hsnCode)
        {
            try
            {
                var result = await _hsnCodeServices.Getbyhsncode(hsnCode);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateGetRate")]
        public async Task<IActionResult> UpdateGstRate(int HsnCode, UpdateHsnDto updateHsnDto)
        {
            try
            {
                var result = await _hsnCodeServices.UpdateHsn(HsnCode, updateHsnDto);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteGstRate(int HsnCode)
        {
            try
            {
                var result = await _hsnCodeServices.DeleteHsn(HsnCode);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
