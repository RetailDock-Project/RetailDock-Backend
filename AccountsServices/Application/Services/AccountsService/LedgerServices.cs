using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Common;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services.AccountsService
{
    public class LedgerServices : ILedgerServices
    {
        private readonly ILogger<LedgerServices> _logger;
        private readonly ILedgerRepository _ledgerRepository;
        public LedgerServices(ILogger<LedgerServices> logger, ILedgerRepository ledgerRepository)
        {
            _logger = logger;
            _ledgerRepository = ledgerRepository;
        }
        public async Task<ApiResponseDTO<string>> CreateLedger(AddLedgerDTO ledgerDTO, Guid organizationId)
        {
            try
            {
                var isExists = await _ledgerRepository.IsLedgerNameExistsAsync(ledgerDTO.LedgerName, organizationId);
                if (isExists)
                {
                    return new ApiResponseDTO<string>
                    {
                        StatusCode = 409,
                        Message = "Ledger name already exists"
                    };
                }
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(ledgerDTO.GroupId);
                if (nature == null)
                {
                    return new ApiResponseDTO<string>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }


                var ledgerId = Guid.NewGuid();

                var ledger = new Ledger
                {
                    Id = ledgerId,
                    LedgerName = ledgerDTO.LedgerName.Trim(),
                    GroupId = ledgerDTO.GroupId,
                    OrganizationId = organizationId,
                    OpeningBalance = ledgerDTO.OpeningBalance,
                    ClosingBalance = ledgerDTO.OpeningBalance,
                    DrCr = ledgerDTO.DrCr,
                    CreatedBy = ledgerDTO.CreatedBy,
                    UpdatedBy = ledgerDTO.UpdateBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,

                    Nature = nature,
                };

                if (ledgerDTO.Details != null)
                {
                    ledger.LedgerDetails = new LedgerDetails
                    {
                        Id = Guid.NewGuid(),
                        LedgerId = ledgerId,
                        ContactName = ledgerDTO.Details.ContactName,
                        ContactNumber = ledgerDTO.Details.ContactNumber,
                        Address = ledgerDTO.Details.Address,
                        GSTNumber = ledgerDTO.Details.GSTNumber,
                        BankName = ledgerDTO.Details.BankName,
                        AccountNumber = ledgerDTO.Details.AccountNumber,
                        IFSCCode = ledgerDTO.Details.IFSCCode,
                        UPIId = ledgerDTO.Details.UPIId,
                        CreatedBy = ledgerDTO.CreatedBy,
                        UpdatedBy = ledgerDTO.UpdateBy,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                }

                var success = await _ledgerRepository.CreateLedgerAsync(ledger);

                return new ApiResponseDTO<string>
                {
                    StatusCode = success ? 201 : 500,
                    Message = success ? "Ledger created successfully." : "Ledger creation failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error creating ledger");
                return new ApiResponseDTO<string>
                {
                    StatusCode = 500,
                    Message = "Internal server error"
                };
            }
        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetAllLedgers(Guid organizationId)
        {
            try
            {


                var ledgers = await _ledgerRepository.GetAllLedgers(organizationId);
                if (ledgers.Count > 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Ledgers fetched successfully",
                        Data = ledgers

                    };
                }
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "NO ledgers found this organization"
                };
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message, "Error in getting ledgers");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in getting ledgers"
                };
            }

        }
        public async Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetLedgerById(Guid id, Guid organizationId)
        {
            try
            {


                var ledger = await _ledgerRepository.GetLedgerById(id, organizationId);
                if (ledger == null)
                {
                    return new ApiResponseDTO<GetLedgerDetailsDTO>
                    {
                        StatusCode = 200,
                        Message = "NO ledgers found this organization or ledgerId",


                    };
                }
                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 200,
                    Message = "Ledger Found",
                    Data = ledger
                };
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message, "Error in getting ledgers");
                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 500,
                    Message = "Error in getting ledgers"
                };
            }

        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetLedgersByGroup(Guid groupId, Guid organizationId)
        {
            try
            {


                var ledger = await _ledgerRepository.GetLedgersByGroup(groupId, organizationId);
                if (ledger.Count > 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Ledgers Found",
                        Data = ledger


                    };
                }
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "NO ledgers found this organization or groupid",


                };
            }
            catch (Exception ex)

            {
                _logger.LogError(ex.Message, "Error in getting ledgers");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in getting ledgers"
                };
            }

        }
        public async Task<ApiResponseDTO<bool>> UpdateLedgerDetails(Guid ledgerId, UpdateLedger updateLedger)
        {
            try
            {
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(updateLedger.GroupId);
                if (nature == null)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }
                updateLedger.Nature = nature;
                var result = await _ledgerRepository.UpdateLedgerDetails(ledgerId, updateLedger);
                if (result)
                {
                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 200,
                        Message = "Ledger updated Succussfully"

                    };

                }
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 200,
                    Message = "Ledger Not found"


                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Ledger updation failed");
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 500,
                    Message = "Ledger updation failed"
                };
            }
        }
        public async Task<ApiResponseDTO<bool>> DeleteLedger(Guid ledgerId)
        {
            try
            {
                var result = await _ledgerRepository.DeleteLedger(ledgerId);
                if (result)
                {

                    return new ApiResponseDTO<bool>
                    {
                        StatusCode = 200,
                        Message = "Ledger Deleted Successfully"

                    };
                }
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 200,
                    Message = "Ledger is not found"
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Ledger deletion failed");
                return new ApiResponseDTO<bool>
                {
                    StatusCode = 500,
                    Message = "Ledger deletion failed"
                };
            }
        }


        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetSalesAcoountLedgerts(Guid organizationId)
        {
            try
            {
                var data = await _ledgerRepository.GetLedgersUnderSalesAccountGroup(organizationId);
                if (data.Count > 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Succussfully fetched ledgers under sales group",
                        Data = data

                    };
                }
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "No ledgers under sales account"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in fetching ledgers under sales account group");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in fetching ledgers under sales account group"

                };
            }

        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetPurchaseAccountLedgerts(Guid organizationId)
        {
            try
            {
                var data = await _ledgerRepository.GetLedgersUnderPurchaseAccountGroup(organizationId);
                if (data.Count > 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Succussfully fetched ledgers under sales group",
                        Data = data

                    };
                }
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "No ledgers under purchase account"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in fetching ledgers under sales account group");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in fetching ledgers under sales account group"

                };
            }

        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetDebtorsandCreditors(Guid OrganizationId)
        {
            try
            {
                var groupIds = await _ledgerRepository.GetDebtorAndCreditorGroupIds(OrganizationId);
                if (groupIds == null || groupIds.Count == 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "No Debtor or Creditor group found in this organization"
                    };
                }

                var allLedgers = new List<GetLedgerDetailsDTO>();

                foreach (var groupId in groupIds)
                {
                    var data = await _ledgerRepository.GetLedgersByGroup(groupId, OrganizationId);
                    if (data != null && data.Any())
                    {
                        allLedgers.AddRange(data);
                    }
                }

                if (!allLedgers.Any())
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "No Debtors or Creditors found"
                    };
                }

                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "Debtors and Creditors fetched successfully",
                    Data = allLedgers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in fetching Debtors and Creditors");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error in fetching Debtors and Creditors"
                };
            }
        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetInputGSTLedgers(Guid organizationId)
        {
            try
            {
                var groupId = await _ledgerRepository.GetInputGSTGroupId(organizationId);
                if (groupId == null || groupId == Guid.Empty)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Input GST group not found"
                    };
                }

                var ledgers = await _ledgerRepository.GetLedgersByGroup(groupId.Value, organizationId);
                if (ledgers == null || !ledgers.Any())
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "No ledgers found under Input GST group"
                    };
                }

                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "Input GST ledgers fetched successfully",
                    Data = ledgers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching Input GST ledgers");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Internal server error while fetching Input GST ledgers"
                };
            }
        }

        public async Task<ApiResponseDTO<List<GetLedgerDetailsDTO>>> GetOutputGSTLedgers(Guid organizationId)
        {
            try
            {
                var groupId = await _ledgerRepository.GetOutputGSTGroupId(organizationId);
                if (groupId == null || groupId == Guid.Empty)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "Output GST group not found in this organization"
                    };
                }

                var ledgers = await _ledgerRepository.GetLedgersByGroup(groupId.Value, organizationId);
                if (ledgers == null || !ledgers.Any())
                {
                    return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                    {
                        StatusCode = 200,
                        Message = "No ledgers found under Output GST group"
                    };
                }

                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 200,
                    Message = "Output GST ledgers fetched successfully",
                    Data = ledgers
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching Output GST ledgers");
                return new ApiResponseDTO<List<GetLedgerDetailsDTO>>
                {
                    StatusCode = 500,
                    Message = "Error fetching Output GST ledgers"
                };
            }
        }
        public async Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetCOGSLedgerDetails(Guid OrganizationId)
        {
            try
            {
                var data = await _ledgerRepository.GetCOGSLedgerByBame(OrganizationId);

                if (data == null)
                {
                    return new ApiResponseDTO<GetLedgerDetailsDTO>
                    {
                        StatusCode = 200,
                        Message = "COGS ledger not found for this organization"
                    };
                }

                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 200,
                    Message = "COGS ledger fetched successfully",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching COGS ledger");
                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 500,
                    Message = "Error fetching COGS ledger"
                };
            }
        }

        public async Task<ApiResponseDTO<GetLedgerDetailsDTO>> GetInventryTransactionDetails(Guid OrganizationId)
        {
            try
            {
                var data = await _ledgerRepository.GetInventryTransactionLedgerByBame(OrganizationId);

                if (data == null)
                {
                    return new ApiResponseDTO<GetLedgerDetailsDTO>
                    {
                        StatusCode = 200,
                        Message = "Inventry transaction ledger not found for this organization"
                    };
                }

                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 200,
                    Message = "Inventry transaction ledger fetched successfully",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching Inventry transaction ledger");
                return new ApiResponseDTO<GetLedgerDetailsDTO>
                {
                    StatusCode = 500,
                    Message = "Error fetching Inventry transaction"
                };
            }
        }
        public async Task<ApiResponseDTO<List<GetLedgerDetailDTO>>> GetCashAndBankLedgers(Guid organizationId)
        {
            try
            {
                var data = await _ledgerRepository.GetCashAndBankLedgers(organizationId);
                if (data.Count > 0)
                {
                    return new ApiResponseDTO<List<GetLedgerDetailDTO>>
                    {
                        StatusCode = 200,
                        Message = "Cash and bank ledgers fetched successfully",
                        Data = data

                    };
                }



                return new ApiResponseDTO<List<GetLedgerDetailDTO>>
                {
                    StatusCode = 200,
                    Message = "No Bank and cash ledger under this organization",

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error fetching bank and cash ledgers");
                return new ApiResponseDTO<List<GetLedgerDetailDTO>>
                {
                    StatusCode = 500,
                    Message = "Error fetching bank and cash ledgers"
                };
            }
        }
        public async  Task<ApiResponseDTO<Guid>> CreateDebtorLedger(AddLedgerDTO ledgerDTO, Guid OrganizationId)
        {
            try
            {
                var isExists = await _ledgerRepository.IsLedgerNameExistsAsync(ledgerDTO.LedgerName, OrganizationId);
                if (isExists)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 409,
                        Message = "Ledger name already exists"
                    };
                }
                var debtorId = await _ledgerRepository.GetGroupIdByNameAndOrganizationId(OrganizationId, "Debtors");
                if (debtorId == null)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(debtorId);
                if (nature == null)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }


                var ledgerId = Guid.NewGuid();

                var ledger = new Ledger
                {
                    Id = ledgerId,
                    LedgerName = ledgerDTO.LedgerName.Trim(),
                    GroupId = debtorId,
                    OrganizationId = OrganizationId,
                    OpeningBalance = ledgerDTO.OpeningBalance,
                    ClosingBalance = ledgerDTO.OpeningBalance,
                    DrCr = ledgerDTO.DrCr,
                    CreatedBy = ledgerDTO.CreatedBy,
                    UpdatedBy = ledgerDTO.UpdateBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                   

                    Nature = nature,
                };

                if (ledgerDTO.Details != null)
                {
                    ledger.LedgerDetails = new LedgerDetails
                    {
                        Id = Guid.NewGuid(),
                        LedgerId = ledgerId,
                        ContactName = ledgerDTO.Details.ContactName,
                        ContactNumber = ledgerDTO.Details.ContactNumber,
                        Address = ledgerDTO.Details.Address,
                        GSTNumber = ledgerDTO.Details.GSTNumber,
                        BankName = ledgerDTO.Details.BankName,
                        AccountNumber = ledgerDTO.Details.AccountNumber,
                        IFSCCode = ledgerDTO.Details.IFSCCode,
                        UPIId = ledgerDTO.Details.UPIId,
                        CreatedBy = ledgerDTO.CreatedBy,
                        UpdatedBy = ledgerDTO.UpdateBy,
                        CreatedAt = DateTime.UtcNow,
                       
                    };
                }

                var success = await _ledgerRepository.CreateLedgerAsync(ledger);

                return new ApiResponseDTO<Guid>
                {
                    StatusCode = success ? 201 : 500,
                    Message = success ? "Ledger created successfully." : "Ledger creation failed.",
                    Data=ledgerId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error creating ledger");
                return new ApiResponseDTO<Guid>
                {
                    StatusCode = 500,
                    Message = "Internal server error"
                };

            }

        }
        public async Task<ApiResponseDTO<Guid>> CreateCreditorLedger(AddLedgerDTO ledgerDTO, Guid OrganizationId)
        {
            try
            {
                var isExists = await _ledgerRepository.IsLedgerNameExistsAsync(ledgerDTO.LedgerName, OrganizationId);
                if (isExists)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 409,
                        Message = "Ledger name already exists"
                    };
                }
                var creditorId = await _ledgerRepository.GetGroupIdByNameAndOrganizationId(OrganizationId, "Creditor");
                if (creditorId == null)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }
                var nature = await _ledgerRepository.GetNatureByGroupIdOrMasterGroupIdAsync(creditorId);
                if (nature == null)
                {
                    return new ApiResponseDTO<Guid>
                    {
                        StatusCode = 404,
                        Message = "Group Not found"
                    };
                }


                var ledgerId = Guid.NewGuid();

                var ledger = new Ledger
                {
                    Id = ledgerId,
                    LedgerName = ledgerDTO.LedgerName.Trim(),
                    GroupId = creditorId,
                    OrganizationId = OrganizationId,
                    OpeningBalance = ledgerDTO.OpeningBalance,
                    ClosingBalance = ledgerDTO.OpeningBalance,
                    DrCr = ledgerDTO.DrCr,
                    CreatedBy = ledgerDTO.CreatedBy,
                    UpdatedBy = ledgerDTO.UpdateBy,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                   

                    Nature = nature,
                };

                if (ledgerDTO.Details != null)
                {
                    ledger.LedgerDetails = new LedgerDetails
                    {
                        Id = Guid.NewGuid(),
                        LedgerId = ledgerId,
                        ContactName = ledgerDTO.Details.ContactName,
                        ContactNumber = ledgerDTO.Details.ContactNumber,
                        Address = ledgerDTO.Details.Address,
                        GSTNumber = ledgerDTO.Details.GSTNumber,
                        BankName = ledgerDTO.Details.BankName,
                        AccountNumber = ledgerDTO.Details.AccountNumber,
                        IFSCCode = ledgerDTO.Details.IFSCCode,
                        UPIId = ledgerDTO.Details.UPIId,
                        CreatedBy = ledgerDTO.CreatedBy,
                        UpdatedBy = ledgerDTO.UpdateBy,
                        CreatedAt = DateTime.UtcNow,
                      
                    };
                }

                var success = await _ledgerRepository.CreateLedgerAsync(ledger);

                return new ApiResponseDTO<Guid>
                {
                    StatusCode = success ? 201 : 500,
                    Message = success ? "Ledger created successfully." : "Ledger creation failed.",
                    Data = ledgerId,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error creating ledger");
                return new ApiResponseDTO<Guid>
                {
                    StatusCode = 500,
                    Message = "Internal server error"
                };

            }

        }
    }
}
