using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseRepository:IPurchaseRepository
    {
        private readonly AppDbContext context;
        public PurchaseRepository(AppDbContext _context) {
            context=_context;
                }
        public async Task<decimal> GetGstByProductId(Guid productId) {

            return await context.Products
                .Where(p => p.Id == productId)
                .Select(p => p.HsnCode.GstRate)
                .FirstOrDefaultAsync();
        }

        public async Task AddDocument(Domain.Entities.Document newDocument) { 
            await context.AddAsync(newDocument);
        }


        public async Task AddPurchase(Purchase purchase) { 
            await context.Purchases.AddAsync(purchase);
        }

        public async Task AddPurchaseInvoice(PurchaseInvoice newPurchaseInv) { 
            await context.PurchaseInvoices.AddAsync(newPurchaseInv);
        }

        public async Task AddPurchaseItems(List<PurchaseItem> purchaseItems) { 
            await context.PurchaseItems.AddRangeAsync(purchaseItems);
        }

        public async Task UpdateStocksAndUnitPrice(PurchaseItem purchaseItem, Guid userId)
        {
            var product=await context.Products.FirstOrDefaultAsync(p=>p.Id== purchaseItem.ProductId);
            if (product != null)
            {
                var existingStock = product.Stock;
                var existingCost = product.CostPrice;

                var newQty = purchaseItem.Quantity;
                var newRate = purchaseItem.RatePerPiece;

                var totalStock = existingStock + newQty;

                var newCost = ((existingStock * existingCost) + (newQty * newRate)) / totalStock;

                product.Stock = totalStock;
                product.CostPrice = newCost;
                product.UpdatedBy = userId;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
       public async Task<List<Purchase>> GetAllPurchase(Guid organaizationId)
        {
            return await context.Purchases
               
                .Include(p=>p.PurchaseInvoice)
                .Where(x=>x.OrganizationId== organaizationId)
                .ToListAsync();

        }
       public async Task<Purchase> GetPurchaseById(Guid purchaseId)
        {
            Console.WriteLine($"Searching for purchaseId: {purchaseId}");
            Console.WriteLine($"Searching for purchaseId: {purchaseId}");

            Console.WriteLine($"Searching for purchaseId: {purchaseId}");

            Console.WriteLine($"Searching for purchaseId: {purchaseId}");

            Console.WriteLine($"Searching for purchaseId: {purchaseId}");



            return await context.Purchases
                .Include(p => p.PurchaseInvoice)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseItems)
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.Id == purchaseId);
        }
        public async Task<List<PurchaseReturn>> GetAllPurchaseReturn(Guid organaizationId)
        {
            return await context.PurchaseReturns
                .Include(pr=>pr.Items)
                .Include(pr=>pr.PurchaseReturnInvoice)
                .Include(pr=>pr.Purchase)
                .ThenInclude(p=>p.PurchaseInvoice)
                .Where(x=>x.OrganizationId == organaizationId)
                .ToListAsync();
        }

        public async Task<PurchaseReturn>getPurchaseReturn(Guid PurchaseReturnId)
        {
            return await context.PurchaseReturns
                .Include(pr=>pr.Supplier)
                .Include(pr => pr.Items)
                .ThenInclude(i => i.Product)
                .Include(pr => pr.PurchaseReturnInvoice)
                .Include(pr => pr.Purchase) 
    .ThenInclude(p => p.PurchaseItems) 
.Include(pr => pr.Purchase) 
    .ThenInclude(p => p.PurchaseInvoice)

                .FirstOrDefaultAsync(x => x.Id == PurchaseReturnId);
                
        }


        public async Task CreatePurchaseReturn(PurchaseReturn purchaseReturn)
        {
            await context.PurchaseReturns.AddAsync(purchaseReturn);
        }

        public async Task CreatePurchaseReturnInvoice(PurchaseReturnInvoice invoice)
        {
            await context.PurchaseReturnInvoices.AddAsync(invoice);
            //return invoice;
        }

        //public async Task<Purchase> GetPurchaseById(Guid purchaseId)
        //{
        //    return await context.Purchases
        //        .Include(p => p.PurchaseItems)
        //        .FirstOrDefaultAsync(p => p.Id == purchaseId);
        //}

        public async Task<PurchaseInvoice> GetPurchaseInvoiceById(Guid invoiceId)
        {
            return await context.PurchaseInvoices
                .FirstOrDefaultAsync(pi => pi.Id == invoiceId);
        }

        public async Task<Product> GetProductById(Guid productId)
        {
            return await context.Products.Include(p=>p.HsnCode)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }

        public async Task<PurchaseItem> GetPurchaseItemById(Guid itemId)
        {
            return await context.PurchaseItems
                .FirstOrDefaultAsync(pi => pi.Id == itemId);
        }

        public async Task<int> GetTotalReturnedQuantity(Guid originalPurchaseItemId)
        {
            return await context.PurchaseReturnItems
                .Where(x => x.OriginalPurchaseItemId == originalPurchaseItemId)
                .SumAsync(x => (int?)x.ReturnedQuantity) ?? 0;
        }


        public async Task UpdateProductStock(PurchaseReturnItem purchaseReturnItem) {

            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == purchaseReturnItem.ProductId);
            if (product != null)
            {
                product.Stock -= purchaseReturnItem.ReturnedQuantity;
            }
        }

        public async Task<string> GetLastPurchaseInvoiceNumber(Guid orgId) {
            return await context.Purchases.Include(p => p.PurchaseInvoice).OrderByDescending(p => p.PurchaseInvoice.CreatedAt).Select(p => p.PurchaseInvoice.InvoiceNumber).FirstOrDefaultAsync();
        
        }

        public async Task<string> GetLastPurchaseReturnInvoiceNumber(Guid orgId)
        {
            return await context.PurchaseReturns.Include(p => p.PurchaseReturnInvoice).OrderByDescending(p => p.PurchaseReturnInvoice.CreatedAt).Select(p => p.PurchaseReturnInvoice.InvoiceNumber).FirstOrDefaultAsync();

        }


        public async Task<PurchaseOrderItem> GetProductPurchaseOrder(Guid productId, Guid? purchaseOrderId) {
            return await context.PurchaseOrdersItem.Include(x => x.Product).FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.PurchaseOrderId == purchaseOrderId);
        }

    }
}
