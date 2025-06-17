using API.Controllers.Base;
using Application.Dto;
using Application.Interfaces.IServices;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : BaseController
    {
        private readonly IPurchaseService purchaseService;
        public PurchaseController(IPurchaseService _purchaseService) { 
            
            purchaseService = _purchaseService;
        
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddPurchase(PurchaseAddDto newPurchase, Guid orgId, Guid userId) {

            //orgId, userId from claim
            var response = await purchaseService.AddPurchase(newPurchase, orgId, userId);
            return StatusCode(response.StatusCode, response);

        }

        //[HttpPost]
        [HttpGet("all/organaizationId")]
        public async Task <IActionResult>GetAllPurchases(Guid organaizationId)
        {
            var responses= await purchaseService.GetAllPurchases(organaizationId);
            return StatusCode(responses.StatusCode, responses);
        }
        [HttpGet("get/{purchaseId}")]
        public async Task <IActionResult>GetByPurchaseId(Guid purchaseId)
        {
            var result= await purchaseService.GetPurchaseDetails(purchaseId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("return")]
        public async Task<IActionResult> AddPurchaseReturn(PurchaseReturnDto newPurchaseReturn,Guid userId,Guid orgid)
        {
            var result=await purchaseService.AddPurchaseReturn(newPurchaseReturn, userId,orgid);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("Get/Return/OrgnaizationId")]

        public async Task <IActionResult>GetAllPurchaseReturn(Guid orgnaizationId)
        {
            var result= await purchaseService.GetAllPurchaseReturn(orgnaizationId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("Get/Return/{PurchaseReturnId}")]

        public async Task   <IActionResult>GetPurchaseReurn(Guid PurchaseReturnId)
        {
            var result= await purchaseService.GetPurchaseReturn(PurchaseReturnId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{organizationId}/export")]
        public async Task<IActionResult> ExportPurchases(Guid organizationId)
        {
            var result = await purchaseService.ExportPurchases(organizationId);

         

            return StatusCode(result.StatusCode, result);
        }
    }
}
