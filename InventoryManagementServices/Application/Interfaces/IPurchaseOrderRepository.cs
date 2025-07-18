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
        Task<PurchaseOrder> GetPurchaseOrderByIdAsync(Guid? id);

        Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<bool> DeletePurchaseOrderAsync(Guid id);

        Task<string> GetLastPurchaseOrderNumber(Guid organizationId);
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(
    Guid orgId,
    string? searchString,
    string? status,
    DateTime? startDate,
    DateTime? endDate,
    int? pageNumber,
    int? pageSize);

        Task<object> GetPurchaseOrderStatsAsync(Guid orgId);

        Task<PurchaseOrder> GetByIdAsync(Guid? purchaseOrderId);
        Task UpdateAsync(PurchaseOrder order);
        Task AddItemsAsync(IEnumerable<PurchaseOrderItem> items);

    }
}
