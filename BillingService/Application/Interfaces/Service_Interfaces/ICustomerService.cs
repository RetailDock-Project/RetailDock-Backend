using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common.ResponseDto;
using Domain.Entites;

namespace Application.Interfaces.Service_Interfaces
{
    public interface ICustomerService
    {
        Task<ResponseDto<List<ViewCustomerSalesDto>>> GetAllCustomers(Guid orgId);
        Task<ResponseDto<object>> addCustomer(Guid orgId, Guid userId, CreateCustomerDto customer);
        Task<ResponseDto<ViewCustomerDto>> viewCustomerByMobile(string phoneNum,Guid orgId);
        Task<ResponseDto<ViewCustomerDto>> viewCustomerById(Guid customerId,Guid orgId);
        Task<ResponseDto<List<ViewCustomerSalesDto>>> viewCustomerSalesDetails(Guid customerId,Guid orgId);
        Task<ResponseDto<List<ViewCustomerSalesDto>>> fetchCreditCustomerSaleDetailsByDate(DateTime fromDate, DateTime? toDate,Guid orgId);
        Task<ResponseDto<List<ViewCustomerDto>>> GetAllCreditCustomers(Guid orgId);
        
        }
}
