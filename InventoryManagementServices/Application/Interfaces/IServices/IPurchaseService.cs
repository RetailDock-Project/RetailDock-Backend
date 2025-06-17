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
        Task<Responses<List<GetPurchaseDto>>> GetAllPurchases(Guid organaizationId);
        Task<Responses<GetPurchaseDetailsDto>> GetPurchaseDetails(Guid purchaseId);
        Task<Responses<object>> AddPurchaseReturn(PurchaseReturnDto newPurchaseReturn, Guid userId,Guid orgId);

        Task <Responses<List<GetPurchaseReturnDto>>> GetAllPurchaseReturn(Guid organaizationId);
        Task<Responses<GetPurchaseReturnDetailsDto>> GetPurchaseReturn(Guid PurchaseReturnId);
        Task<Responses<object>> ExportPurchases(Guid organizationId);
    }
}
