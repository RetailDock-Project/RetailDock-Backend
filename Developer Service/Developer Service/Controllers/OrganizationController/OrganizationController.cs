using Application.DTOs;
using Application.Interfaces.IService;
using Application.Services.OrganizationService;
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
        [HttpPost("organization-subscription/add")]
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
        [HttpGet("details/get")]
        public async Task<IActionResult> GetOrganizationDetailsAsync()
        {
            var result = await _organizationServices.GetAllOrganizationWithSubscription();

            return StatusCode(result.StatusCode, result);
        }
        [HttpPatch("{organizationId}/block-unblock")]
        public async Task<IActionResult> BlockOrganization(Guid organizationId)
        {
            var result = await _organizationServices.BlockOrganization(organizationId);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetOrganizationDetailById(Guid id)
        {
            var result = await _organizationServices.GetOrganizationDetailById(id);

            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("dashboard/summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _organizationServices.GetDashboardSummary();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("signup-chart")]
        public async Task<IActionResult> GetSignupChartData()
        {
            var result = await _organizationServices.GetOrganizationSignupLast7MonthsAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetMonthlyRevenueChart()
        {
            var result = await _organizationServices.GetLast7MonthsRevenueAsync();
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("get-filter")]
        public async Task<IActionResult> GetAllOrganizations([FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _organizationServices.GetAllOrganizationsAsync(search, status);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("{organizationId}/details")]
        public async Task<IActionResult> GetOrganizationDetails(Guid organizationId)
        {
            var result = await _organizationServices.GetOrganizationDetailsAsync(organizationId);
            return StatusCode(result.StatusCode, result);
        }

    }
}
