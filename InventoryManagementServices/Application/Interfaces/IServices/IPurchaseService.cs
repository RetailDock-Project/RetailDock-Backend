using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.IServices
{
    public interface IPurchaseService
    {
        Task<Responses<object>> AddPurchase(PurchaseAddDto newPurchase, Guid OrgId,Guid userId);
        Task<Responses<List<GetPurchaseDto>>> GetAllPurchases(Guid organaizationId, DateTime? fromDate, DateTime? toDate);
        Task<Responses<GetPurchaseDetailsDto>> GetPurchaseDetails(Guid purchaseId);
        Task<Responses<object>> AddPurchaseReturn(PurchaseReturnDto newPurchaseReturn, Guid userId,Guid orgId);

        Task <Responses<List<GetPurchaseReturnDto>>> GetAllPurchaseReturn(Guid organaizationId);
        Task<Responses<GetPurchaseReturnDetailsDto>> GetPurchaseReturn(Guid PurchaseReturnId);
        Task<byte[]?> ExportPurchases(Guid organizationId, DateTime? fromDate, DateTime? toDate);        Task<Responses<List<GetPurchaseDto>>> GetPurchasesAsync(
    Guid organizationId,
    string? searchTerm,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize);

        Task<Responses<List<GetPurchaseReturnDto>>> GetPurchaseReturnsFilterAsync(
    Guid organizationId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize);
    }
}
