using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Dapper;
using Domain.Entities;
using Infrastructure.DapperContext;

namespace Infrastructure.Repository.GroupRepository
{
    public class LedgerRepository : ILedgerRepository
    {
        private readonly DapperConection _dapperContext;
        public LedgerRepository(DapperConection dapperContext)
        {
            _dapperContext = dapperContext;
        }
       
        public async Task<bool> CreateLedgerAsync(Ledger ledger)
        {
            var sqlLedger = @"
                            INSERT INTO Ledgers 
                            (Id, LedgerName, GroupId, OrganizationId, OpeningBalance, ClosingBalance, DrCr, IsActive, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, Nature)
                            VALUES 
                            (@Id, @LedgerName, @GroupId, @OrganizationId, @OpeningBalance, @ClosingBalance, @DrCr, @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy,@Nature)";

            var sqlDetails = @"
                             INSERT INTO LedgerDetails 
                             (Id, LedgerId, ContactName, ContactNumber, Address, GSTNumber, BankName, AccountNumber, IFSCCode, UPIId, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
                             VALUES 
                             (@Id, @LedgerId, @ContactName, @ContactNumber, @Address, @GSTNumber, @BankName, @AccountNumber, @IFSCCode, @UPIId, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy)";

            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var result = await connection.ExecuteAsync(sqlLedger, ledger, transaction);

                if (ledger.LedgerDetails != null)
                {
                    await connection.ExecuteAsync(sqlDetails, ledger.LedgerDetails, transaction);
                }

                transaction.Commit();
                return result > 0;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<bool> IsLedgerNameExistsAsync(string ledgerName, Guid organizationId)
        {
            var sql = @"SELECT COUNT(1)
                FROM Ledgers
                WHERE LOWER(LedgerName) = LOWER(@LedgerName)
                AND OrganizationId = @OrganizationId";

            using var connection = _dapperContext.CreateConnection();
            var count = await connection.ExecuteScalarAsync<int>(sql, new { LedgerName = ledgerName, OrganizationId = organizationId });

            return count > 0;
        }
        public async Task<List<GetLedgerDetailsDTO>> GetAllLedgers(Guid organizationId)
        {
            var sql = @"SELECT 
             l.Id,
             l.LedgerName,
             l.GroupId,
             g.GroupName,
             l.OrganizationId,
             l.OpeningBalance,
             l.ClosingBalance,
             l.DrCr,
             l.IsActive,
             l.CreatedAt,
             l.UpdatedAt,
             l.CreatedBy,
             l.UpdatedBy,
                       d.ContactName,
                       d.ContactNumber,
                       d.Address,
                       d.GSTNumber,
                       d.BankName,
                       d.AccountNumber,
                       d.IFSCCode,
                       d.UPIId,
                       d.CreatedAt AS LedgerDetailsCreatedAt,
                       d.UpdatedAt AS LedgerDetailsUpdatedAt,
                       d.CreatedBy AS LedgerDetailsCreatedBy,
                       d.UpdatedBy AS LedgerDetailsUpdatedBy
                       FROM Ledgers l
                       LEFT JOIN LedgerDetails d ON l.Id = d.LedgerId
                      LEFT JOIN AccountsGroups g ON l.GroupId = g.Id 
                      WHERE l.OrganizationId = @OrganizationId And ;";



            using var connection = _dapperContext.CreateConnection();
            var data = await connection.QueryAsync<GetLedgerDetailsDTO>(sql, new { OrganizationId = organizationId });

            return data.ToList();
        }

        public async Task<GetLedgerDetailsDTO> GetLedgerById(Guid id, Guid organizationId)
        {
            var sql = @"SELECT   l.Id,
    l.LedgerName,
    l.GroupId,
    g.GroupName,
    l.OrganizationId,
    l.OpeningBalance,
    l.ClosingBalance,
    l.DrCr,
    l.IsActive,
    l.CreatedAt,
    l.UpdatedAt,
    l.CreatedBy,
    l.UpdatedBy,

    d.ContactName,
    d.ContactNumber,
    d.Address,
    d.GSTNumber,
    d.BankName,
    d.AccountNumber,
    d.IFSCCode,
    d.UPIId,
    d.CreatedAt AS LedgerDetailsCreatedAt,
    d.UpdatedAt AS LedgerDetailsUpdatedAt,
    d.CreatedBy AS LedgerDetailsCreatedBy,
    d.UpdatedBy AS LedgerDetailsUpdatedBy
      FROM Ledgers l
      LEFT JOIN LedgerDetails d ON l.Id = d.LedgerId
      LEFT JOIN AccountsGroups g ON l.GroupId = g.Id 
      WHERE l.Id = @Id AND l.OrganizationId = @OrganizationId;";
  
            using var connection = _dapperContext.CreateConnection();
            var data = await connection.QueryFirstOrDefaultAsync<GetLedgerDetailsDTO>(sql, new { Id = id, OrganizationId = organizationId });

            return data;
        }
        public async Task<List<GetLedgerDetailsDTO>> GetLedgersByGroupHierarchyAsync(Guid groupId, Guid organizationId)
        {
            var sql = "CALL GetAllLedgersUnderGroupHierarchy(@GroupId, @OrganizationId);";

            using var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<GetLedgerDetailsDTO>(sql, new
            {
                GroupId = groupId,
                OrganizationId = organizationId
            });

            return result.ToList();
        }
        public async Task<List<GetLedgerDetailsDTO>> GetLedgersByGroup(Guid groupId, Guid organizationId)
        {
            var sql = "CALL GetAllLedgersUnderGroupHierarchy(@GroupId, @OrganizationId);";

            using var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<GetLedgerDetailsDTO>(sql, new
            {
                GroupId = groupId,
                OrganizationId = organizationId
            });

            return result.ToList();
        }
        public async Task<bool> UpdateLedgerDetails(Guid ledgerId, UpdateLedger updateLedger)
        {
            using var connection = _dapperContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
               
                var existingLedger = await connection.QueryFirstOrDefaultAsync<dynamic>(
                    "SELECT LedgerName, GroupId, OpeningBalance, DrCr,Nature FROM Ledgers WHERE Id = @LedgerId",
                    new { LedgerId = ledgerId }, transaction);

                if (existingLedger == null)
                    throw new Exception("Ledger not found");

               
                var updatedLedger = new
                {
                    LedgerName = string.IsNullOrWhiteSpace(updateLedger.LedgerName)
                                 ? (string)existingLedger.LedgerName
                                 : updateLedger.LedgerName.Trim(),

                    GroupId = updateLedger.GroupId == Guid.Empty
                              ? (Guid)existingLedger.GroupId
                              : updateLedger.GroupId,

                    OpeningBalance = updateLedger.OpeningBalance == 0
                                     ? (decimal)existingLedger.OpeningBalance
                                     : updateLedger.OpeningBalance,

                    DrCr = string.IsNullOrWhiteSpace(updateLedger.DrCr)
                           ? (string)existingLedger.DrCr
                           : updateLedger.DrCr,

                    UpdatedBy = updateLedger.UpdateBy,
                    LedgerId = ledgerId,
                    Nature= updateLedger.Nature
                };

                // Step 3: Update
                var sqlLedger = @"
            UPDATE Ledgers
            SET 
                LedgerName = @LedgerName,
                GroupId = @GroupId,
                OpeningBalance = @OpeningBalance,
                DrCr = @DrCr,
                UpdatedAt = CURRENT_TIMESTAMP,
                UpdatedBy = @UpdatedBy,
                 Nature =@Nature
            WHERE Id = @LedgerId";

                await connection.ExecuteAsync(sqlLedger, updatedLedger, transaction);

                // Step 4: Update LedgerDetails (if present)
                if (updateLedger.Details != null)
                {
                    var sqlDetails = @"
                UPDATE LedgerDetails
                SET 
                    ContactName = @ContactName,
                    ContactNumber = @ContactNumber,
                    Address = @Address,
                    GSTNumber = @GSTNumber,
                    BankName = @BankName,
                    AccountNumber = @AccountNumber,
                    IFSCCode = @IFSCCode,
                    UPIId = @UPIId,
                    UpdatedAt = CURRENT_TIMESTAMP,
                    UpdatedBy = @UpdatedBy
                WHERE LedgerId = @LedgerId";

                    await connection.ExecuteAsync(sqlDetails, new
                    {
                        ContactName = updateLedger.Details.ContactName,
                        ContactNumber = updateLedger.Details.ContactNumber,
                        Address = updateLedger.Details.Address,
                        GSTNumber = updateLedger.Details.GSTNumber,
                        BankName = updateLedger.Details.BankName,
                        AccountNumber = updateLedger.Details.AccountNumber,
                        IFSCCode = updateLedger.Details.IFSCCode,
                        UPIId = updateLedger.Details.UPIId,
                        UpdatedBy = updateLedger.UpdateBy,
                        LedgerId = ledgerId
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<bool> DeleteLedger(Guid ledgerId)
        {
            var sql = @"Update Ledgers set IsActive=false where Id=@id";
            var connection= _dapperContext.CreateConnection();
            var data= await connection.ExecuteAsync(sql,new {id=ledgerId});
            return data > 0;
        }

        public async Task<List<GetLedgerDetailsDTO>> GetLedgersUnderSalesAccountGroup(Guid OrganizationId)
        {
            var sql = @"Call GetLedgersUnderSalesGroup(@OrganizationId);";
            var connection = _dapperContext.CreateConnection();
            var data = await connection.QueryAsync<GetLedgerDetailsDTO>(sql, new { organizationId = OrganizationId });
            return data.ToList();
        }
        public async Task<List<GetLedgerDetailsDTO>> GetLedgersUnderPurchaseAccountGroup(Guid OrganizationId)
        {
            var sql = @"Call GetLedgersUnderSalesGroup(@OrganizationId);";
            var connection = _dapperContext.CreateConnection();
            var data = await connection.QueryAsync<GetLedgerDetailsDTO>(sql, new { organizationId = OrganizationId });
            return data.ToList();
        }
        public async Task<List<Guid>> GetDebtorAndCreditorGroupIds(Guid OrganizationId)
        {
            var sql = @"SELECT Id 
                FROM AccountsGroups 
                WHERE GroupName IN ('Debtors', 'Creditors') 
                  AND OrganizationId = @OrganizationId;";

            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<Guid>(sql, new { OrganizationId });
            return result.ToList();
        }
        public async Task<Guid?> GetOutputGSTGroupId(Guid organizationId)
        {
            var sql = @"SELECT Id 
                FROM AccountsGroups 
                WHERE GroupName = 'Input GST'
                  AND OrganizationId = @OrganizationId;";

            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { OrganizationId = organizationId });
            return result;
        }
        public async Task<Guid?> GetInputGSTGroupId(Guid organizationId)
        {
            var sql = @"SELECT Id 
                FROM AccountsGroups 
                WHERE GroupName = 'Input GST'
                  AND OrganizationId = @OrganizationId;";

            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<Guid?>(sql, new { OrganizationId = organizationId });
            return result;
        }
        public async Task<GetLedgerDetailsDTO> GetCOGSLedgerByBame(Guid organizationId)
        {
            var sql = @"SELECT Id 
                FROM Ledgers 
                WHERE LedgerName = 'COGS'
                  AND OrganizationId = @OrganizationId;";

            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<GetLedgerDetailsDTO>(sql, new { OrganizationId = organizationId });
            return result;
        }
        public async Task<GetLedgerDetailsDTO> GetInventryTransactionLedgerByBame(Guid organizationId)
        {
            var sql = @"SELECT Id 
                FROM Ledgers 
                WHERE LedgerName = 'Inventory Transaction'
                  AND OrganizationId = @OrganizationId;";

            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<GetLedgerDetailsDTO>(sql, new { OrganizationId = organizationId });
            return result;
        }
        public async Task<List<GetLedgerDetailDTO>> GetCashAndBankLedgers(Guid organizationId)
        {
            var sql = @"SELECT 
                  L.Id AS Id,
                     L.LedgerName,
                    G.GroupName,
                     L.OrganizationId
                    FROM Ledgers L
                   JOIN AccountsGroups G ON L.GroupId = G.Id
                    WHERE G.Id IN (
                   '720b45fc-429f-11f0-a0c7-862ccfb05833',  -- Cash in Hand
                   '720c48e9-429f-11f0-a0c7-862ccfb05833'   -- Bank Accounts
                     )
                   AND L.OrganizationId = @OrganizationId;";
            var connection = _dapperContext.CreateConnection();
            var result = await connection.QueryAsync<GetLedgerDetailDTO>(sql, new { OrganizationId = organizationId });
            return result.ToList();
        }
        public async Task<string> GetNatureByGroupIdOrMasterGroupIdAsync(Guid id)
        {
            var sql = "SELECT Nature FROM AccountsGroups WHERE Id = @Id OR AccountsMasterGroupId = @Id LIMIT 1;";
            using var connection = _dapperContext.CreateConnection();
            var nature = await connection.QueryFirstOrDefaultAsync<string>(sql, new { Id = id });
            return nature;
        }

        public async Task<Guid> GetGroupIdByNameAndOrganizationId(Guid organizationId, string groupname)
        {
            var sql = @"Select Id from AccountsGroups Where GroupName=@GroupName And OrganizationId=@OrganizationId;";
            using var connection = _dapperContext.CreateConnection();
            var GroupId = await connection.QueryFirstOrDefaultAsync<Guid>(sql, new { GroupName = groupname, OrganizationId = organizationId });
            return GroupId;
        }

    }
}
