using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Services;
using Domain.Entites;

namespace Application.Interfaces.Repository_Interfaces
{
    public interface ISaleReturnRepository
    {
        Task SaveChanges();
        Task<Sales> fetchSalesByInvoice(string invoiceNum,Guid orgId);
        Task addNewB2BSalesReturn(AddSalesReturnDto salesReturn, Guid saleId, Guid orgId, Guid userId,GST_Type gst_type);
        Task addNewB2CSalesReturn(AddSalesReturnDto salesReturn, Guid saleId, Guid orgId, Guid userId,GST_Type gst_type);
        Task<List<SalesReturn>> fetchAllSalesReturn(Guid orgId);
        Task<SalesReturn> GetSalesReturnDetailsByInvoice(string invoiceNum,Guid orgId);
        Task<List<SalesReturn>> GetSalesReturnDetailsBydate(DateTime fromDate, DateTime? toDate,Guid orgId);
        Task <SalesReturn> GetSalesReturnDetailsById(Guid returnId,Guid orgId);

    }
}
