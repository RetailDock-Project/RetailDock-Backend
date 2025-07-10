using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.IRepository;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class SupplierRepository:ISupplierRepository
    {
        private readonly AppDbContext context;
        public SupplierRepository(AppDbContext _context) { 
            context = _context;
        }
        public async Task CreateSupplier(Supplier newSupplier) { 
            await context.Suppliers.AddAsync(newSupplier);
        }

        public async Task<Supplier> GetSupplierWithGSTNumber(string gstNumber,Guid orgId)
        {
            return await context.Suppliers.FirstOrDefaultAsync(s => s.GSTNumber == gstNumber && s.OrganizationId==orgId);
        }

        public async Task<Supplier> GetSupplierByIdAndOrgId(Guid supplierId,Guid orgId)
        {
            return await context.Suppliers.FirstOrDefaultAsync(s => s.Id==supplierId && s.OrganizationId==orgId);
        }
        public async Task RemoveSupplier(Guid supplierId)
        {
            var supplier =await context.Suppliers.FirstOrDefaultAsync(s => s.Id == supplierId);
            supplier.IsActive= false;
            await context.SaveChangesAsync();   
        }

        public async Task<List<Supplier>> GetAllSuppliersByOrganizationId(Guid orgId)
        {
            return await context.Suppliers
                .Where(s => s.OrganizationId == orgId && s.IsActive)
                .ToListAsync();
        }

        public async Task<(List<Supplier>, int)> GetSuppliersByFilterAsync(Guid orgId, string? search, bool? isActive, int? pageNumber, int? pageSize)
        {
            var query = context.Suppliers
                .Where(s => s.OrganizationId == orgId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s => s.Name.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            int totalCount = await query.CountAsync();

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            var result = await query.ToListAsync();

            return (result, totalCount);
        }

    }
}
