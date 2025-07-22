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
        public async Task<List<Purchase>> GetAllPurchase(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            var query =context.Purchases
                .Include(p => p.PurchaseInvoice)
                .Where(p => p.OrganizationId == organizationId);

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= toDate.Value.Date);
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Purchase> GetPurchaseById(Guid purchaseId)
        {


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


        public async Task<List<Purchase>> GetPurchases(
    Guid organizationId,
    string? searchTerm,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize)
        {
            var query = context.Purchases
                .Include(p => p.PurchaseInvoice)
                .Include(p => p.PurchaseItems).ThenInclude(i => i.Product)
                .Where(p => p.OrganizationId == organizationId)
                .AsQueryable();

            // Date filters
            if (fromDate.HasValue)
                query = query.Where(p => p.CreatedAt >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(p => p.CreatedAt <= toDate.Value.Date);

            // Search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var lowerTerm = searchTerm.ToLower();
                query = query.Where(p =>
                    (p.PurchaseInvoice.InvoiceNumber != null && p.PurchaseInvoice.InvoiceNumber.ToLower().Contains(lowerTerm))  ||
                    p.PurchaseItems.Any(i => i.Product.ProductName.ToLower().Contains(lowerTerm))
                );
            }

            // Conditional pagination
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }


        public async Task<List<PurchaseReturn>> GetPurchaseReturnsFilterAsync(
    Guid organizationId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize)
        {
            var query = context.PurchaseReturns
                .Include(pr => pr.Items)
                .Include(pr => pr.PurchaseReturnInvoice)
                .Include(pr => pr.Purchase)
                    .ThenInclude(p => p.PurchaseInvoice)
                .Where(pr => pr.OrganizationId == organizationId)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                var from = DateOnly.FromDateTime(fromDate.Value);
                query = query.Where(pr => pr.ReturnDate >= from);
            }

            if (toDate.HasValue)
            {
                var to = DateOnly.FromDateTime(toDate.Value);
                query = query.Where(pr => pr.ReturnDate <= to);
            }


            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(pr =>
                    pr.Reason.ToLower().Contains(search) ||
                    pr.PurchaseReturnInvoice.InvoiceNumber.ToLower().Contains(search) ||
                    pr.Purchase.Supplier.Name.ToLower().Contains(search));
            }

            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }


        public async Task<List<Purchase>> GetRecentPurchases(Guid organizationId, DateTime fromDate, DateTime toDate)
        {
            return await context.Purchases
                .Include(p=>p.PurchaseInvoice)
                .Include(p => p.PurchaseItems)
                .Include(p => p.Supplier)
                .Where(p => p.OrganizationId == organizationId &&
                            p.CreatedAt >= fromDate && p.CreatedAt <= toDate)
                .AsNoTracking()

                .ToListAsync();
        }

        public async Task<List<PurchaseReturn>> GetRecentReturns(Guid organizationId, DateTime fromDate, DateTime toDate)
        {
            return await context.PurchaseReturns
                                .Include(p => p.PurchaseReturnInvoice)

                .Include(r => r.Items)
                .Include(r => r.Supplier)
                .Where(r => r.OrganizationId == organizationId &&
                            r.CreatedAt >= fromDate && r.CreatedAt <= toDate)
                .AsNoTracking()
                .ToListAsync();
        }


    }
}
