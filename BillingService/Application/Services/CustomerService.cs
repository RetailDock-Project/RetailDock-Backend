using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using AutoMapper;
using Common.ResponseDto;
using Domain.Entites;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class CustomerService : ICustomerService
    {

        private readonly ICustomerRepository customerRepo;
        private readonly IMapper mapper;
        private readonly ILogger<CustomerService> logger;
        public CustomerService(ICustomerRepository _repo, IMapper _mapper, ILogger<CustomerService> _logger)
        {
            customerRepo = _repo;
            mapper = _mapper;
            _logger = logger;
        }
        public async Task<ResponseDto<List<ViewCustomerSalesDto>>> GetAllCustomers(Guid orgId)
        {
            try
            {
              var customers=await   customerRepo.GetAllCustomers(orgId);
                if(customers == null)
                {
                    return new ResponseDto<List<ViewCustomerSalesDto>> {Message="no customers found",StatusCode=404};
                }
                var mapped=mapper.Map<List<ViewCustomerSalesDto>>(customers);
                return new ResponseDto<List<ViewCustomerSalesDto>> {Data=mapped, Message = "customers fetched succeesfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error occured while fetching all customer");
                return new ResponseDto<List<ViewCustomerSalesDto>> { Message = "internalServer", StatusCode = 500 };
            }

        }
    
        public async Task<ResponseDto<List<ViewCustomerDto>>> GetAllCreditCustomers(Guid orgId)
        {
            try
            {
              var customers=await   customerRepo.GetCreditCustomers(orgId);
                if(customers == null)
                {
                    return new ResponseDto<List<ViewCustomerDto>> {Message="no cerdit customers found",StatusCode=404};
                }
                var mapped=mapper.Map<List<ViewCustomerDto>>(customers);
                return new ResponseDto<List<ViewCustomerDto>> {Data=mapped, Message = "credit customers fetched succeesfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error occured while fetching all credit customer");
                return new ResponseDto<List<ViewCustomerDto>> { Message = "internalServer", StatusCode = 500 };
            }

        }
        public async Task<ResponseDto<object>> addCustomer(Guid orgId, Guid userId, CreateCustomerDto customer)
        {
            try
            { 
                var saleMode = customer.SaleMode.ToLower();

                if (saleMode == "credit")
                {
                    var existing = await customerRepo.fetchCreditCusomersByMobile(customer.PhoneNumber, orgId);
                    if (existing == null)
                    {
                        await customerRepo.AddNewCrditCustomer(customer, orgId, userId);
                        await customerRepo.SaveChanges();
                        return new ResponseDto<object> { Message = "New Debtor Added", StatusCode = 201 };
                    }

                    return new ResponseDto<object> { Message = "Customer Already Exists", StatusCode = 200 };
                }
                else if (saleMode == "cash")
                {
                    var existing = await customerRepo.fetchCashCusomersByMobile(customer.PhoneNumber, orgId);
                    if (existing == null)
                    {
                        await customerRepo.AddNewCashCustomer(customer, orgId, userId);
                        await customerRepo.SaveChanges();
                        return new ResponseDto<object> { Message = "New Cash Customer Added", StatusCode = 201 };
                    }

                    return new ResponseDto<object> { Message = "Customer Already Exists", StatusCode = 200 };
                }
                else
                {
                    var existing = await customerRepo.fetchCreditCusomersByMobile(customer.PhoneNumber, orgId);
                    if (existing == null)
                    {
                        await customerRepo.AddNewB2BCustomers(customer, orgId, userId);
                        await customerRepo.SaveChanges();
                        return new ResponseDto<object> { Message = "New B2B Customer Added", StatusCode = 201 };
                    }

                    return new ResponseDto<object> { Message = "Customer Already Exists", StatusCode = 200 };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error From Adding new customer");
                return new ResponseDto<object> { Message = "Internal Server Error", StatusCode = 500 };
            }
        }


        public async  Task<ResponseDto<List<ViewCustomerSalesDto>>> fetchCreditCustomerSaleDetailsByDate(DateTime fromDate, DateTime? toDate,Guid orgId)
        {
            try
            {
                var tillDate=toDate??DateTime.Now;
               var creditCustomer=await customerRepo.fetchCreditCustomerSaleDetailsByDate(fromDate, tillDate,orgId);
                if (creditCustomer == null)
                {

                    return new ResponseDto<List<ViewCustomerSalesDto>>
                    {

                        Message = "no sales is found between that date",
                        StatusCode = 404
                    };
                }
                    var mapped = mapper.Map<List<ViewCustomerSalesDto>>(creditCustomer);
                    return new ResponseDto<List<ViewCustomerSalesDto>>
                    {
                        Data = mapped,
                        Message = "Sales fetched successfully",
                        StatusCode = 200
                    };
                }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all sales details by date");
                return new ResponseDto<List<ViewCustomerSalesDto>>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }
           
        public async Task<ResponseDto<List<ViewCustomerSalesDto>>> viewCustomerSalesDetails(Guid customerId,Guid orgId)
        {
            try
            {
                var cashCustomer = await customerRepo.fetchCashCustomersById(customerId,orgId);
                var creditCustomer = await customerRepo.fetchCreditCustomersById(customerId,orgId);
                if (creditCustomer != null)
                {
                    {
                        var creditCustomerSales = await customerRepo.fetchCreditCustomerSaleDetailsById(customerId,orgId);
                        if (creditCustomerSales != null)
                        {
                            var salesDetails = mapper.Map<List<ViewCustomerSalesDto>>(creditCustomerSales);
                            return new ResponseDto<List<ViewCustomerSalesDto>> { Data = salesDetails, Message = "fetch credit customers sales successfully", StatusCode = 200 };
                        }

                        return new ResponseDto<List<ViewCustomerSalesDto>> { Message = "Debtor found with No sales ", StatusCode = 200 };
                    }
                }
                if (cashCustomer != null)
                {

                    var cashCustomerSales = await customerRepo.fetchCashCustomerSaleDetailsById(customerId,orgId);

                    if (cashCustomerSales != null)
                    {
                        var salesDetails = mapper.Map<List<ViewCustomerSalesDto>>(cashCustomer);
                        return new ResponseDto<List<ViewCustomerSalesDto>> { Data = salesDetails, Message = "fetch cash customers Sale details successfully", StatusCode = 200 };
                    }
                    return new ResponseDto<List<ViewCustomerSalesDto>> { Message = "customer found with No sales ", StatusCode = 200 };
                }
                return new ResponseDto<List<ViewCustomerSalesDto>> { Message = "no Customer found by Id", StatusCode = 404 };
            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "Error From ViewCustomer salesDetails");
                return new ResponseDto<List<ViewCustomerSalesDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        public async Task<ResponseDto<ViewCustomerDto>> viewCustomerByMobile(string phoneNum,Guid orgId)
        {
            try
            {
                var cashCustomer = await customerRepo.fetchCashCusomersByMobile(phoneNum,orgId);
                var creditCustomer = await customerRepo.fetchCreditCusomersByMobile(phoneNum,orgId);
                if (creditCustomer != null)
                {
                    var customer = mapper.Map<ViewCustomerDto>(creditCustomer);
                    return new ResponseDto<ViewCustomerDto> { Data = customer, Message = "fetch credit customers successfully", StatusCode = 200 };
                }
                if (cashCustomer != null)
                {
                    var customer = mapper.Map<ViewCustomerDto>(cashCustomer);
                    return new ResponseDto<ViewCustomerDto> { Data = customer, Message = "fetch cash customers successfully", StatusCode = 200 };
                }
                return new ResponseDto<ViewCustomerDto> { Message = "no Customer found on this Number", StatusCode = 404 };

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error From ViewCustomer by  Mobile");
                return new ResponseDto<ViewCustomerDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        public async Task<ResponseDto<ViewCustomerDto>> viewCustomerById(Guid customerId,Guid orgId)
        {
            try
            {
                var cashCustomer = await customerRepo.fetchCashCustomersById(customerId,orgId);
                var creditCustomer = await customerRepo.fetchCreditCustomersById(customerId,orgId);
                if (creditCustomer != null)
                {
                    var customer = mapper.Map<ViewCustomerDto>(creditCustomer);
                    return new ResponseDto<ViewCustomerDto> { Data = customer, Message = "fetch credit customers successfully", StatusCode = 200 };
                }
                if (cashCustomer != null)
                {
                    var customer = mapper.Map<ViewCustomerDto>(cashCustomer);
                    return new ResponseDto<ViewCustomerDto> { Data = customer, Message = "fetch cash customers successfully", StatusCode = 200 };
                }
                return new ResponseDto<ViewCustomerDto> { Message = "no Customer found on this Id", StatusCode = 404 };

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error From ViewCustomer by  customerId");
                return new ResponseDto<ViewCustomerDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }


    }
}

