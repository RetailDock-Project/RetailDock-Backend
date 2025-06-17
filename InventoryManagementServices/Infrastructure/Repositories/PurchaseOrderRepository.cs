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
        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(Guid id)
        {
            return await _context.PurchaseOrders
                                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);
        }
        public async Task<PurchaseOrder> UpdatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            _context.PurchaseOrders.Update(purchaseOrder);
            await _context.SaveChangesAsync();
            return purchaseOrder;
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
    }
}
