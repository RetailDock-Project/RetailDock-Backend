using Application.DTOs;
using Application.Interfaces.Service_Interfaces;
using BillingService.ActionFillter;
using Common.ResponseDto;
using IdentityService.Controllers.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BillingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController
    {
        private readonly ICustomerService customerService;
        public CustomersController(ICustomerService _customerService)
        {
            customerService =_customerService;

        }
        [HttpGet("getAllCustomers")]
        public async Task<IActionResult> fetchallCustomers(bool isFullData, int? skip, int? take)
        {
            var result = await customerService.GetAllCustomers(OrgId,UserId,isFullData,skip,take);
            return StatusCode(result.StatusCode, result);
        }   
        [HttpGet("getCreditCustomers")]
        public async Task<IActionResult> fetchCreditCustomers(bool isFullData, int? skip, int? take)
        {
            var result = await customerService.GetAllCreditCustomers(OrgId, UserId, isFullData, skip, take);
            return StatusCode(result.StatusCode, result);
        }
        [HttpGet("viewCustomerById")]
        public async Task<IActionResult> viewCustomer(Guid customerId)
        {
            var result = await customerService.viewCustomerById(customerId, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        
        [HttpGet("viewCustomerByMobile")]
        public async Task<IActionResult> viewCustomerMobile(string mobile)
        {
            var result = await customerService.viewCustomerByMobile(mobile, OrgId);
            return StatusCode(result.StatusCode, result);
        } 
        [HttpGet("viewCustomerSale")]
        public async Task<IActionResult> CustomerSalesDetails(Guid customerId)
        {
            var result = await customerService.viewCustomerSalesDetails(customerId, OrgId);
            return StatusCode(result.StatusCode, result);
        }  
        [HttpGet("viewCustomerDetailsByDate")]
        public async Task<IActionResult> viewCustomerDetailsByDate(DateTime fromDate, DateTime? toDate)
        {
            var result = await customerService.fetchCreditCustomerSaleDetailsByDate(fromDate,toDate, OrgId);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("addNewCreditCustomer")]
        public async Task<IActionResult> addNewCreditCustomer(  CreateCustomerDto customer)
        {
            var result = await customerService.addCreditCustomer( OrgId, UserId,  customer);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("addNewCashCustomer")]
        public async Task<IActionResult> addNewCashCustomer( CreateCashCustomerDto customer)
        {
            var result = await customerService.addCashCustomer( OrgId, UserId,  customer);
            return StatusCode(result.StatusCode, result);
        }



    }
}
