using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IRepository
{
    public interface IPurchaseRepository
    {
        Task<decimal> GetGstByProductId(Guid productId);
        Task AddDocument(Domain.Entities.Document newDocument);
        Task AddPurchaseItems(List<PurchaseItem> purchaseItems);
        Task AddPurchaseInvoice(PurchaseInvoice newPurchaseInv);
        Task AddPurchase(Purchase purchase);
        Task UpdateStocksAndUnitPrice(PurchaseItem purchaseItem, Guid userId);
        Task<List<Purchase>> GetAllPurchase(Guid organaizationId);
        Task<Purchase>GetPurchaseById(Guid purchaseId);


        Task CreatePurchaseReturn(PurchaseReturn purchaseReturn);
        Task CreatePurchaseReturnInvoice(PurchaseReturnInvoice invoice);
        Task<PurchaseInvoice> GetPurchaseInvoiceById(Guid invoiceId);
        Task<Product> GetProductById(Guid productId);
        Task<PurchaseItem> GetPurchaseItemById(Guid itemId);
        Task<int> GetTotalReturnedQuantity(Guid originalPurchaseItemId);

        Task UpdateProductStock(PurchaseReturnItem purchaseReturnItem);
        Task<List<PurchaseReturn>> GetAllPurchaseReturn(Guid organaizationId);
        Task<PurchaseReturn> getPurchaseReturn(Guid PurchaseReturnId);
//>>>>>>> b80b0ab47de4d81e95c3c12fc1b6ca358902868d

        Task<string> GetLastPurchaseInvoiceNumber(Guid orgId);
        Task<string> GetLastPurchaseReturnInvoiceNumber(Guid orgId);

        Task<PurchaseOrderItem> GetProductPurchaseOrder(Guid productId, Guid? purchaseOrderId);
    }
}
