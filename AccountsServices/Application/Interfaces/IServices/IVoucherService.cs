using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Common;

namespace Application.Interfaces.IServices
{
    public interface IVoucherService
    {
        Task<ApiResponseDTO<bool>> AddVoucherEntrys(Guid organizationId, Guid CreatedBy, AddVouchersDTO addVoucherDTO);
        Task<ApiResponseDTO<List<GetVoucherTransactionByVoucherTypeId>>> GetTransactionsByVoucherTypeAsync(Guid voucherTypeId, Guid organizationId, DateTime? fromDate, DateTime? toDate);
        Task<ApiResponseDTO<List<GetAllVoucherTypeDTO>>> GetAllVoucherTypes();
    }

 }
