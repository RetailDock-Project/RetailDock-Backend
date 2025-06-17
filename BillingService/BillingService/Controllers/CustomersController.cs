using Application.DTOs;
using Application.Interfaces.Service_Interfaces;
using Common.ResponseDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService customerService;
        public CustomersController(ICustomerService _customerService)
        {
            customerService =_customerService;

        }
        [HttpGet("getAllCustomers")]
        public async Task<IActionResult> fetchallCustomers(Guid orgId)
        {
            var result = await customerService.GetAllCustomers(orgId);
            return StatusCode(result.StatusCode, result);
        }   
        [HttpGet("getCreditCustomers")]
        public async Task<IActionResult> fetchCreditCustomers(Guid orgId)
        {
            var result = await customerService.GetAllCreditCustomers(orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("viewCustomerById")]
        public async Task<IActionResult> viewCustomer(Guid customerId, Guid orgId)
        {
            var result = await customerService.viewCustomerById(customerId, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("viewCustomerByMobile")]
        public async Task<IActionResult> viewCustomerMobile(string mobile, Guid orgId)
        {
            var result = await customerService.viewCustomerByMobile(mobile, orgId);
            return StatusCode(result.StatusCode, result);
        } 
        [HttpGet("viewCustomerSale")]
        public async Task<IActionResult> CustomerSalesDetails(Guid customerId, Guid orgId)
        {
            var result = await customerService.viewCustomerSalesDetails(customerId, orgId);
            return StatusCode(result.StatusCode, result);
        }  
        [HttpGet("viewCustomerDetailsByDate")]
        public async Task<IActionResult> viewCustomerDetailsByDate(DateTime fromDate, DateTime? toDate, Guid orgId)
        {
            var result = await customerService.fetchCreditCustomerSaleDetailsByDate(fromDate,toDate, orgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("addNewCustomer")]
        public async Task<IActionResult> addNewCustomer(Guid orgId, Guid userId, CreateCustomerDto customer)
        {
            var result = await customerService.addCustomer( orgId, userId,  customer);
            return StatusCode(result.StatusCode, result);
        }



    }
}
