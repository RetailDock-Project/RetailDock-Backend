using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PurchaseOrderRepository:IPurchaseOrderRepository   
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PurchaseOrder> AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            await _context.PurchaseOrders.AddAsync(purchaseOrder);
            await _context.SaveChangesAsync();
            return purchaseOrder;
        }
        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(Guid orgnaizationId)
        {
            return await _context.PurchaseOrders
                                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)

                .ThenInclude(i => i.Product)
                .Where(x=>x.OrganizationId == orgnaizationId)
                .ToListAsync();
        }
        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(Guid? id)
        {
            return await _context.PurchaseOrders
                                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .ThenInclude(i => i.Product).ThenInclude(p=>p.HsnCode)
                .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);
        }
        public async Task UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            // EF Core is tracking the changes already
            await _context.SaveChangesAsync();
        }
        public async Task<bool> DeletePurchaseOrderAsync(Guid id)
        {
            var order = await _context.PurchaseOrders.FindAsync(id);
            if (order == null)
                return false;

            _context.PurchaseOrders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetLastPurchaseOrderNumber(Guid organizationId) {

            var lastPo = await _context.PurchaseOrders.Where(po => po.OrganizationId == organizationId).OrderByDescending(po => po.CreatedAt).Select(po => po.PurchaseOrderNumber).FirstOrDefaultAsync();
            return lastPo;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync(
    Guid orgId,
    string? searchString,
    string? status,
    DateTime? startDate,
    DateTime? endDate,
    int? pageNumber,
    int? pageSize)
        {
            var query = _context.PurchaseOrders
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p=>p.HsnCode)
                .Where(p => p.OrganizationId == orgId)
                .AsQueryable();

            // 🔍 Filter by search string (supplier or product)
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Supplier.Name.Contains(searchString) ||
                    p.PurchaseOrderItems.Any(i => i.Product.ProductName.Contains(searchString))
                );
            }

            // 📦 Filter by status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.OrderStatus==status);
            }

            // 📅 Filter by date range
            if (startDate.HasValue)
            {
                query = query.Where(p => p.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.OrderDate <= endDate.Value);
            }

            // 📄 Order by latest
            query = query.OrderByDescending(p => p.OrderDate);

            // 📃 Apply pagination only if both pageNumber and pageSize are provided
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                query = query
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }


        public async Task<object> GetPurchaseOrderStatsAsync(Guid orgId)
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Include(p => p.PurchaseOrderItems)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.HsnCode)
                .Where(p => p.OrganizationId == orgId)
                .ToListAsync();

            int totalOrders = purchaseOrders.Count;
            int currentMonthOrders = purchaseOrders
                .Count(p => p.OrderDate.Month == DateTime.UtcNow.Month && p.OrderDate.Year == DateTime.UtcNow.Year);

            // Calculate total value including GST
            decimal totalValue = 0;
            decimal pendingValue = 0;
            int pendingOrders = 0;

            foreach (var po in purchaseOrders)
            {
                decimal orderTotalWithTax = 0;

                foreach (var item in po.PurchaseOrderItems)
                {
                    var amount = item.RatePerPiece * item.Quantity;
                    var gstRate = item.Product?.HsnCode?.GstRate ?? 0;
                    var tax = amount * gstRate / 100;
                    orderTotalWithTax += amount + tax;
                }

                totalValue += orderTotalWithTax;

                if (po.OrderStatus == "Pending")
                {
                    pendingOrders++;
                    pendingValue += orderTotalWithTax;
                }
            }

            decimal avgValue = totalOrders > 0 ? totalValue / totalOrders : 0;

            return new
            {
                TotalOrders = totalOrders,
                TotalValue = totalValue,
                AvgValue = avgValue,
                CurrentMonthOrders = currentMonthOrders,
                PendingOrders = pendingOrders,
                PendingValue = pendingValue
            };
        }


        //public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(Guid id)
        //{
        //    return await _context.PurchaseOrders
        //        .Include(p => p.PurchaseOrderItems)
        //        .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);
        //}

        //public async Task UpdatePurchaseOrderAsync(PurchaseOrder order)
        //{
        //    _context.PurchaseOrders.Update(order);
        //    await _context.SaveChangesAsync();
        //}

        public async Task<PurchaseOrder> GetByIdAsync(Guid? purchaseOrderId)
        {
            return await _context.PurchaseOrders
                .Include(po => po.PurchaseOrderItems)
                .FirstOrDefaultAsync(po => po.PurchaseOrderId == purchaseOrderId);
        }

        public async Task UpdateAsync(PurchaseOrder order)
        {
            _context.PurchaseOrders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task AddItemsAsync(IEnumerable<PurchaseOrderItem> items)
        {
            await _context.PurchaseOrdersItem.AddRangeAsync(items);
            await _context.SaveChangesAsync();
        }



    }
}
