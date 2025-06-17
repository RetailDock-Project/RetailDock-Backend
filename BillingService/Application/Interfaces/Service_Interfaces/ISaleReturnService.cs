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
        Task<ResponseDto<List<SalesReturnViewDto>>> GetAllSalesReturnDetails(Guid orgId);
        Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsById(Guid saleId, Guid orgId);
        Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsByInvoice(string ReturninvoiceNum, Guid orgId);
        Task<ResponseDto<List<SalesReturnViewDto>>> GetSalesReturnByDate(DateTime fromDate, DateTime? toDate, Guid orgId);
    }
}
