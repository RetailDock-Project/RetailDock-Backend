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
        Task<ResponseDto<object>> AddNewCashSale(SalesAddDto sales,CreateSaleIdsDto allIdsDto);
        Task<ResponseDto<object>>  AddNewCreditSale(SalesAddDto sales,CreateSaleIdsDto allIdsDto);
        Task<List<Sales>>GetSaleDetailsByDate(DateTime fromDate, DateTime toDate,Guid orgId);
        Task<List<Sales>> GetAllSalesDetails(Guid orgId);
        Task<Sales> GetSalesDetailsById(Guid saleId,Guid orgId);
        Task<Sales> GetB2CSalesDetailsByInvoice(string invoiceNum,Guid orgId);
        Task<Sales> GetB2BSalesDetailsByInvoice(string invoiceNum,Guid orgId);
        Task SaveChanges();
    }
}
