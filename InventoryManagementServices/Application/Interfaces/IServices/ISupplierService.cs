using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Interfaces.IServices
{
    public interface ISupplierService
    {
        Task<Responses<object>> CreateSupplier(SupplierDto newSupplier, Guid orgId, Guid userId);
        Task<Responses<object>> RemoveSupplier(Guid supplierId, Guid orgId, Guid userId);
        Task<Responses<List<SupplierDto>>> GetAllSuppliersByOrganizationId(Guid orgId);
        Task<Responses<List<SupplierDto>>> GetSuppliersByFilterAsync(Guid orgId, string? search, bool? isActive, int? pageNumber, int? pageSize);

        Task<Responses<object>> ChangeStatusOfSupplier(Guid supplierId,Guid userId);
    }
}
