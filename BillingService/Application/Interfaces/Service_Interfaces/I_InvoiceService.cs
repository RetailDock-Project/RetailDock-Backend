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
    public interface I_InvoiceService
    {

        Task<ResponseDto<List<SalesInvoiceViewDto>>> getAllSaleInvoices(Guid orgId, Guid userId, bool? isFullData, int? skip, int? take);

        Task<ResponseDto<SalesInvoiceViewDto>> getSaleInvoicesByInvoiceNumber(Guid orgId, string invoiceNum);

        Task<ResponseDto<List<SalesReturnInvoiceViewDto>>> getSaleReturnInvoicesByDate(Guid orgId, Guid userId, bool? isFullData, DateTime? fromDate, DateTime toDate);
        Task<ResponseDto<List<SalesInvoiceViewDto>>> getSaleInvoicesByDueDate(Guid orgId,Guid userId ,bool? isFullData, DateTime? fromdate, DateTime todate);

        Task<ResponseDto<List<SalesInvoiceViewDto>>> getPendingSalesInvoices(Guid orgId, Guid userId, bool? isFullData, DateTime? fromDate, DateTime? toDate);
        Task<ResponseDto<List<SalesReturnInvoiceViewDto>>> getAllSaleReturnInvoices(Guid orgId, Guid userId, bool? isFullData, int? skip, int? take);

        Task<ResponseDto<SalesReturnInvoiceViewDto>> getSaleReturnByInvoiceNumber(Guid orgId, string invoiceNum);
        Task<ResponseDto<List<SalesInvoiceViewDto>>> getSaleInvoicesByDate(Guid orgId, Guid userId, bool? isFullData, DateTime? fromdate, DateTime todate);




    }
}
