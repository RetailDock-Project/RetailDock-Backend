using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerController : ControllerBase
    {
        private readonly LedgerConsumerService ledgerService;

        public LedgerController(LedgerConsumerService _ledgerService)
        {
            ledgerService = _ledgerService;
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetProducts(Guid orgId)
        {
            var response = await ledgerService.GetPurchaseLedgers(orgId);

            return StatusCode(response.StatusCode,response);
        }
    }
}
