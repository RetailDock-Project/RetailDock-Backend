using Application.DTO;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerController : ControllerBase
    {
        private readonly ILedgerServices _ledgerServices;
        public LedgerController(ILedgerServices ledgerServices)
        {
            {
                _ledgerServices = ledgerServices;
            }
        }
        [HttpPost("add/new/ledger")]
        public async Task<IActionResult>CreateNewLedger(AddLedgerDTO addLedgerDTO,Guid OrganizationId)
        {
            var result= await _ledgerServices.CreateLedger(addLedgerDTO, OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/ledgers")]
        public async Task<IActionResult> GetAllLedgers(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetAllLedgers( OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledger/byId")]
        public async Task<IActionResult> GetLedgerById(Guid id,Guid OrganizationId)
        {
            var result = await _ledgerServices.GetLedgerById(id,OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/bygroup")]
        public async Task<IActionResult> GetLedgersByGroups(Guid id, Guid OrganizationId)
        {
            var result = await _ledgerServices.GetLedgersByGroup(id, OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("update/ledger")]
        public async Task<IActionResult> UpdateLedger(Guid ledgerId, UpdateLedger updateLedgerDetailsDTO)
        {
            var result = await _ledgerServices.UpdateLedgerDetails(ledgerId, updateLedgerDetailsDTO);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("delete/ledger")]
        public async Task<IActionResult> DeleteLedger(Guid ledgerId)
        {
            var result = await _ledgerServices.DeleteLedger(ledgerId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/sales")]
        public async Task<IActionResult> GetledgersUnderSalkes(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetSalesAcoountLedgerts(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        //[HttpGet("get/ledgers/purchase")]
        //public async Task<IActionResult> GetledgersUnderPurchase(Guid OrganizationId)
        //{
        //    var result = await _ledgerServices.GetPurchaseAccountLedgerts(OrganizationId);
        //    return StatusCode(result.StatusCode, result);
        //}
        [HttpGet("get/ledgers/debtors")]
        public async Task<IActionResult> GetDebtors(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetDebtorsandCreditors(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/output/gst")]
        public async Task<IActionResult> GetLedgersOfOutPutTax(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetOutputGSTLedgers(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/input/gst")]
        public async Task<IActionResult> GetLedgersOfInPutTax(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetInputGSTLedgers(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/COGS/ledger/byname")]
        public async Task<IActionResult> GetCOGSLedgerByName(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetCOGSLedgerDetails(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/inventrytransaction/ledger/byname")]
        public async Task<IActionResult> GetInventryTransactionLedgerByName(Guid OrganizationId)
        {
            var result = await _ledgerServices.GetInventryTransactionDetails(OrganizationId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
