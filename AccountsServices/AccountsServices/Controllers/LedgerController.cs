using AccountsServices.Controllers.BaseControllers;
using Application.DTO;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccountsServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LedgerController : BaseController
    {
        private readonly ILedgerServices _ledgerServices;
        public LedgerController(ILedgerServices ledgerServices)
        {
            {
                _ledgerServices = ledgerServices;
            }
        }
        [HttpPost("add/new/ledger")]
        public async Task<IActionResult>CreateNewLedger(AddLedgerDTO addLedgerDTO)
        {
            addLedgerDTO.CreatedBy = UserId;
            var result= await _ledgerServices.CreateLedger(addLedgerDTO, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/all/ledgers")]
        public async Task<IActionResult> GetAllLedgers()
        {
            var result = await _ledgerServices.GetAllLedgers(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledger/byId")]
        public async Task<IActionResult> GetLedgerById(Guid id)
        {
            var result = await _ledgerServices.GetLedgerById(id, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/bygroup")]
        public async Task<IActionResult> GetLedgersByGroups(Guid id)
        {
            var result = await _ledgerServices.GetLedgersByGroup(id, OrgId);
            return StatusCode(result.StatusCode, result);
        }



        //add organization id validation
        [HttpPatch("update/ledger")]
        public async Task<IActionResult> UpdateLedger(Guid ledgerId, UpdateLedger updateLedgerDetailsDTO)
        {
            updateLedgerDetailsDTO.UpdateBy= UserId;
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
        public async Task<IActionResult> GetledgersUnderSalkes()
        {
            var result = await _ledgerServices.GetSalesAcoountLedgerts(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/purchase")]
        public async Task<IActionResult> GetledgersUnderPurchase()
        {
            var result = await _ledgerServices.GetPurchaseAccountLedgerts(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/debtors")]
        public async Task<IActionResult> GetDebtors()
        {
            var result = await _ledgerServices.GetDebtorsandCreditors(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/output/gst")]
        public async Task<IActionResult> GetLedgersOfOutPutTax()
        {
            var result = await _ledgerServices.GetOutputGSTLedgers(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/ledgers/input/gst")]
        public async Task<IActionResult> GetLedgersOfInPutTax()
        {
            var result = await _ledgerServices.GetInputGSTLedgers(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/COGS/ledger/byname")]
        public async Task<IActionResult> GetCOGSLedgerByName()
        {
            var result = await _ledgerServices.GetCOGSLedgerDetails(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/inventrytransaction/ledger/byname")]
        public async Task<IActionResult> GetInventryTransactionLedgerByName()
        {
            var result = await _ledgerServices.GetInventryTransactionDetails(OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("get/bankandcash/ledgers")]
        public async Task<IActionResult> GetCashAndBankLedgers()
        {
            var result = await _ledgerServices.GetCashAndBankLedgers(OrgId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
