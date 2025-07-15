using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Domain.Entites;

namespace Application.Interfaces.Repository_Interfaces
{
    public  interface I_InvoiceRepository
    {
        Task<List<SalesInvoice>> getAllSaleInvoices(Guid orgId, Guid userId, bool isFullData, int? skip, int? take);

        Task<SalesInvoice> getSaleInvoicesByB2BInvoiceNumber(Guid orgId, string invoiceNum);

        Task<SalesInvoice> getSaleInvoicesByB2CInvoiceNumber(Guid orgId, string invoiceNum);
        
        Task<List<SalesReturnInvoice>> getAllSaleReturnInvoices(Guid orgId, Guid userId, bool isFullData, int? skip, int? take);

        Task<SalesReturnInvoice> getSaleReturnByB2BInvoiceNumber(Guid orgId, string invoiceNum);
        Task<List<SalesReturnInvoice>> getSaleReturnInvoicesByDate(Guid orgId, Guid userId, bool isFullData, DateTime fromDate, DateTime toDate);
        Task<SalesReturnInvoice> getSaleReturnByB2CInvoiceNumber(Guid orgId, string invoiceNum);

        Task<List<SalesInvoice>> getSaleInvoicesByDueDate(Guid orgId,Guid userId,bool isfullData, DateTime fromdate, DateTime todate);
        Task<List<SalesInvoice>> getPendingSalesInvoices(Guid orgId, Guid userId, bool isFullData,DateTime? FromDate,DateTime? todate );
        Task<List<SalesInvoice>> getSaleInvoicesByDate(Guid orgId, Guid userId, bool isFullData, DateTime fromdate, DateTime todate);
    }

}

