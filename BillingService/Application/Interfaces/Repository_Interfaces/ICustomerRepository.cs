using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entites;
using Microsoft.Extensions.Logging;

namespace Application.Interfaces.Repository_Interfaces
{
    public  interface ICustomerRepository
    {

 Task<List<Sales>> GetAllCustomers(Guid orgId);
  
        Task<List<CreditCustomers>> fetchCreditCustomerSaleDetailsByDate(DateTime fromDate, DateTime toDate,Guid orgId);
        Task<List<CashCustomers>> fetchAllCashCustomers(Guid orgId);
     
        Task<List<CreditCustomers>> fetchAllCreditCustomers(Guid orgId);

        Task<CashCustomers> fetchCashCustomersById(Guid customerId,Guid orgId);
        Task<CreditCustomers> fetchCreditCustomersById(Guid customerId,Guid orgId);
        Task SaveChanges();
        Task<CreditCustomers> fetchCreditCusomersByMobile(string mobile,Guid orgId);
      
        Task<CashCustomers> fetchCashCusomersByMobile(string mobile,Guid orgId);
        Task<CreditCustomers> fetchCreditCustomerSaleDetailsById(Guid customerId,Guid orgId);
        Task<CashCustomers> fetchCashCustomerSaleDetailsById(Guid customerId,Guid orgId);
        Task<List<CreditCustomers>> GetCreditCustomers(Guid orgId); 
        Task AddNewCashCustomer(CreateCustomerDto customer, Guid orgId, Guid userId);

       

           Task AddNewCrditCustomer(CreateCustomerDto customer, Guid orgId, Guid userId);


  Task AddNewB2BCustomers(CreateCustomerDto customer, Guid orgId, Guid userId);
            

        }
    }
