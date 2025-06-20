using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(Guid orgnaizationId);
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(Guid id);
        Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeletePurchaseOrderAsync(Guid id);

        Task<string> GetLastPurchaseOrderNumber(Guid organizationId);
    }
}
