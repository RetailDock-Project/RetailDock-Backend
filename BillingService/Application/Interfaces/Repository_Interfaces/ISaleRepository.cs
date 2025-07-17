using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common.ResponseDto;
using Domain.Entites;

namespace Application.Interfaces.Repository_Interfaces
{
    public interface ISaleRepository
    {
        Task<CreditCustomers> GetCreditCustomers(string phoneNUmber, Guid OrgId);
        Task<string> GenerateB2BInvoiceNumber(Guid orgId);
        Task<string> GenerateB2CInvoiceNumber(Guid orgId);
        Task<CashCustomers> GetCashCustomers(string phoneNUmber, Guid OrgId);
        Task<List<Sales>> GetDebtorsSales(Guid debtorId, Guid orgId);
        Task<Product> GetProductById(Guid productId, Guid orgId);
        Task<HsnCode> GetHsnCode(int HsnCodeId);
        Task<ResponseDto<object>> AddNewCashSale(SalesAddDto sales,CreateSaleIdsDto allIdsDto);
        Task<ResponseDto<object>>  AddNewCreditSale(SalesAddDto sales,CreateSaleIdsDto allIdsDto);
        Task CashReceived(Guid debtorsId, decimal receivedAmount, decimal currentBalance, Guid orgId);
        Task<List<Sales>> GetSaleDetailsByDate(DateTime fromDate, DateTime toDate, Guid orgId, Guid userId, bool fullData, int? skip, int? take);
        Task<List<Sales>> GetAllSalesDetails(Guid orgId, Guid userId, bool isFullData, int? skip, int? take);
        Task<Sales> GetSalesDetailsById(Guid saleId,Guid orgId);
        Task<Sales> GetB2CSalesDetailsByInvoice(string invoiceNum,Guid orgId);
        Task<Sales> GetB2BSalesDetailsByInvoice(string invoiceNum,Guid orgId);
        Task SaveChanges();
    }
}
