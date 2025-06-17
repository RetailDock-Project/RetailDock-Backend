using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Common;

namespace Application.Interfaces.IServices
{
    public interface ILedgerServices
    {
        Task<ApiResponseDTO<string>> CreateLedger(AddLedgerDTO ledgerDTO, Guid OrganizationId);
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetAllLedgers(Guid organizationId);
        Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetLedgerById(Guid id, Guid organizationId);
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetLedgersByGroup(Guid groupId, Guid organizationId);
        Task<ApiResponseDTO<bool>> UpdateLedgerDetails(Guid ledgerId, UpdateLedger updateLedger);
        Task<ApiResponseDTO<bool>> DeleteLedger(Guid ledgerId);
        Task <ApiResponseDTO<List <GetLedgerDetailsDTO>>> GetSalesAcoountLedgerts(Guid organizationId);
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetPurchaseAccountLedgerts(Guid organizationId);
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetDebtorsandCreditors(Guid OrganizationId);
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetInputGSTLedgers(Guid organizationId);
       
        Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetOutputGSTLedgers(Guid OrganizationId);
        Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetCOGSLedgerDetails(Guid OrganizationId);
        Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetInventryTransactionDetails(Guid OrganizationId);

    }
}
