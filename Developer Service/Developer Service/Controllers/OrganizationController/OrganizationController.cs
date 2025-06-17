using Application.DTOs;
using Application.Interfaces.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Developer_Service.Controllers.OrganizationController
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationServices _organizationServices;
        public OrganizationController(IOrganizationServices organizationServices)
        {
            _organizationServices = organizationServices;
        }
        [HttpPost("subscription/add")]
        public async Task <IActionResult>AddSubscription(AddSubscriptionDTO addSubscriptionDTO,Guid userId)
        {
            var result= await _organizationServices.AddSubscription(addSubscriptionDTO,userId);
           
           return StatusCode(result.StatusCode,result);
        }
        [HttpGet("organization/count/get")]
        public async Task<IActionResult> GteTotalOrganizations()
        {
            var result = await _organizationServices.GetTotalCountOfOrganization();

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("subscription/received/Amount/by/date")]
        public async Task<IActionResult> GteTotalAmountReceivedBydate(DateTime FromDate,DateTime ToDate)
        {
            var result = await _organizationServices.TotalSubscriptionReceivedBySpecificDate(FromDate,ToDate);

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("subscription/received/Amount/by/current/month")]
        public async Task<IActionResult> GteTotalAmountReceivedByMonth()
        {
            var result = await _organizationServices.TotalSubscriptionReceivedByCurrentMonth();

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("subscription/received/Amount/by/current/year")]
        public async Task<IActionResult> GteTotalAmountReceivedByYear()
        {
            var result = await _organizationServices.TotalSubscriptionReceivedByCurrentYear();

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("organization/status/summery")]
        public async Task<IActionResult> GetOrganizationStatusSummeryAsync()
        {
            var result = await _organizationServices.GetOrganizationAccountStatusSummaryAsync();

            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("organization/details/get")]
        public async Task<IActionResult> GetOrganizationDetailsAsync()
        {
            var result = await _organizationServices.GetAllOrganizationWithSubscription();

            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("organization/block")]
        public async Task<IActionResult> BlockOrganization(Guid OrganizationId)
        {
            var result = await _organizationServices.BlockOrganization(OrganizationId);

            return StatusCode(result.StatusCode, result);
        }
    }
}
