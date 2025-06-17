using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;

namespace Application.Interfaces.IRepository
{
    public  interface ISupplierRepository
    {
        Task CreateSupplier(Supplier newSupplier);
        Task<Supplier> GetSupplierWithGSTNumber(string gstNumber,Guid orgId);
        Task<Supplier> GetSupplierByIdAndOrgId(Guid supplierId, Guid orgId);
        Task RemoveSupplier(Guid supplierId);
        Task<List<Supplier>> GetAllSuppliersByOrganizationId(Guid orgId);

    }
}
