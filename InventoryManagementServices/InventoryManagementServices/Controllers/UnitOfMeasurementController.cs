using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitOfMeasurementController : ControllerBase
    {
        private readonly IUnitOfMeasureServices _services;

        public UnitOfMeasurementController(IUnitOfMeasureServices services)
        {
            _services = services;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddUnit([FromBody] UnitOfMeasureDto dto)
        {
            try
            {
                var result = await _services.AddUnitOfMeasure(dto);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(Guid OrganaiztionId)
        {
            try
            {
                var result = await _services.GetAllUnitOfMeasures(OrganaiztionId);
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _services.GetUnitOfMeasureById(id);
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
        public async Task<IActionResult> Update(int Id, [FromBody] UnitOfMeasureDto dto)
        {
            try
            {
                var result = await _services.UpdateUnitOfMeasure(Id,dto);
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _services.DeleteUnitOfMeasure(id);
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
