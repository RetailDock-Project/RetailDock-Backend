using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Common.ResponseDto;

namespace Application.Interfaces.Service_Interfaces
{
    public  interface ISaleReturnService
    {
        Task<ResponseDto<object>> AddSalesReturn(AddSalesReturnDto salesReturn, Guid orgId, Guid userId);
        Task<ResponseDto<List<SalesReturnViewDto>>> GetAllSalesReturnDetails(Guid orgId,Guid userId,bool isFullData,int? skip,int? Take);

        Task<ResponseDto<decimal>> getReturnedProductCount(Guid saleId, Guid productId, Guid orgId);
        Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsById(Guid saleId, Guid orgId);
        Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsByInvoice(string ReturninvoiceNum, Guid orgId);
        Task<ResponseDto<List<SalesReturnViewDto>>> GetSalesReturnByDate(DateTime fromDate, DateTime? toDate, bool? isFullData, Guid orgId,Guid userId);
        Task<ResponseDto<SalesReturnTaxReportDto>> GetSalesReturnTaxReport(DateTime? fromDate, DateTime? toDate, Guid orgId, Guid userId);
        Task<ResponseDto<string>> GenerateB2CReturnInvoiceNumber(Guid orgId);
        Task<ResponseDto<string>> GenerateB2BReturnInvoiceNumber(Guid orgId);
    }
}
