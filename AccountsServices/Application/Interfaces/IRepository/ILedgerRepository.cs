using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Common;
using Domain.Entities;

namespace Application.Interfaces.IRepository
{
    public interface ILedgerRepository
    {
       
        Task<bool> CreateLedgerAsync(Ledger ledger);
        Task<bool> IsLedgerNameExistsAsync(string ledgerName, Guid organizationId);
      
        Task<List<GetLedgerDetailsDTO>> GetAllLedgers(Guid organizationId);
        Task<GetLedgerDetailsDTO>GetLedgerById(Guid id,Guid organizationId);
        Task<List<GetLedgerDetailsDTO>> GetLedgersByGroup(Guid groupId, Guid organizationId);
        Task<bool> UpdateLedgerDetails(Guid ledgerId, UpdateLedger updateLedger);
        Task<bool> DeleteLedger(Guid ledgerId);
        Task<List<GetLedgerDetailsDTO>> GetLedgersUnderSalesAccountGroup(Guid OrganizationId);
        Task<List<GetLedgerDetailsDTO>> GetLedgersUnderPurchaseAccountGroup(Guid OrganizationId);
        Task<List<Guid>> GetDebtorAndCreditorGroupIds(Guid OrganizationId);
        Task<Guid?> GetInputGSTGroupId(Guid organizationId);
        Task<Guid?> GetOutputGSTGroupId(Guid organizationId);
        Task<GetLedgerDetailsDTO> GetCOGSLedgerByBame(Guid organizationId);
        Task<GetLedgerDetailsDTO> GetInventryTransactionLedgerByBame(Guid organizationId);
        Task<List<GetLedgerDetailDTO>> GetCashAndBankLedgers(Guid organizationId);


    }
}
