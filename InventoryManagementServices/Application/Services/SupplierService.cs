using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class SupplierService:ISupplierService
    {
        private readonly IMapper mapper;
        private readonly ISupplierRepository supplierRepo;
        public SupplierService(IMapper _mapper,ISupplierRepository _supplierRepo) {
            mapper = _mapper;
            supplierRepo = _supplierRepo;
        }
        public async Task<Responses<object>> CreateSupplier(SupplierDto newSupplier,Guid orgId,Guid userId)
        {
            try {

                if (orgId == Guid.Empty || userId == Guid.Empty) {
                    return new Responses<object> { StatusCode=400,Message="invalid credentials"};
                }
                var alreadyExist = await supplierRepo.GetSupplierWithGSTNumber(newSupplier.GSTNumber,orgId);

                if (alreadyExist != null) {
                    return new Responses<object> { StatusCode = 409 ,Message=$"Supplier with GST-{newSupplier.GSTNumber} already exist"};
                }

                var supplier=mapper.Map<Supplier>(newSupplier);
                supplier.OrganizationId = orgId;
                supplier.CreatedBy = userId;
                await supplierRepo.CreateSupplier(supplier);
                return new Responses<object> { StatusCode = 200 ,Message="Supplier created"};
            
            } catch (Exception ex) {
                return new Responses<object> { StatusCode = 500, Message="Error in creating supplier" };

            }

        }

        public async Task<Responses<object>> RemoveSupplier(Guid supplierId, Guid orgId, Guid userId)
        {
            try
            {
               var supplierExist= await supplierRepo.GetSupplierByIdAndOrgId(supplierId, orgId);
                if (supplierExist == null) {
                    return new Responses<object> { StatusCode = 404, Message = "Supplier not found" };
                }

                await supplierRepo.RemoveSupplier(supplierId);
                return new Responses<object> { StatusCode = 200, Message = "supplier removed successfully" };


            }
            catch (Exception ex)
            {
                return new Responses<object> { StatusCode = 500, Message = "Internal server error" };

            }

        }


        public async Task<Responses<List<SupplierDto>>> GetAllSuppliersByOrganizationId(Guid orgId)
        {
            try
            {
                if (orgId == Guid.Empty)
                {
                    return new Responses<List<SupplierDto>> { StatusCode = 400, Message = "Invalid organization ID" };
                }

                var suppliers = await supplierRepo.GetAllSuppliersByOrganizationId(orgId);

                var supplierDtos = mapper.Map<List<SupplierDto>>(suppliers);

                return new Responses<List<SupplierDto>> { StatusCode = 200, Message = "Success", Data = supplierDtos };
            }
            catch (Exception)
            {
                return new Responses<List<SupplierDto>> { StatusCode = 500, Message = "Error retrieving suppliers" };
            }
        }

    }
}
