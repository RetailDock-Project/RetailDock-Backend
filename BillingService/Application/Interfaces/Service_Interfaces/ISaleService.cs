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
    public interface ISaleService
    {
        Task<ResponseDto<object>> AddNewSale(SalesAddDto sales, Guid orgId, Guid userId);
        Task<ResponseDto<List<SalesResponseDto>>> GetAllSalesDetails(Guid orgId);
        Task<ResponseDto<SalesResponseDto>> GetSalesDetailsById(Guid saleId,Guid orgId);
        Task<ResponseDto<SalesResponseDto>> GetSalesDetailsByInvoice(string invoiceNumber,Guid orgId);
        Task<ResponseDto<List<SalesResponseDto>>> GetSalesByDate(DateTime fromDate,DateTime? toDate,Guid orgId);

    }
}
