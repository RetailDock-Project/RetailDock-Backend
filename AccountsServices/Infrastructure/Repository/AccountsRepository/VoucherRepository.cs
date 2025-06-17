using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Dapper;
using Domain.Entities;
using Infrastructure.DapperContext;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Infrastructure.Repository.AccountsRepository
{
    public class VoucherRepository:IAccountsRepository
    {
        private readonly DapperConection _dapperConection;
        public VoucherRepository(DapperConection dapperConection)
        {
            _dapperConection = dapperConection;
        }
        public async Task<string> GenerateVoucherNumber(Guid organizationId, Guid voucherTypeId)
        {
            var sql = @"Select Count (*) from Vouchers where OrganizationId=@organizationId and VoucherTypeId=@voucherTypeId;";
            var connection= _dapperConection.CreateConnection();
            var countObj = await connection.ExecuteScalarAsync(sql, new { organizationId= organizationId , voucherTypeId = voucherTypeId });
            int count = countObj != null ? Convert.ToInt32(countObj) : 0;
            return (count+1).ToString("D5");
        }
        public async Task<bool> AddVoucherEntrys(Guid organizationId, Guid CreatedBy, Vouchers vouchers, List<TransactionsDTO> transactions)
        {
            using var connection= _dapperConection.CreateConnection();
            connection.Open();
            using var transaction=connection.BeginTransaction();
            try
            {
                string insertVoucherSql = @"INSERT INTO Vouchers (
                Id, VoucherNumber, VoucherTypeId, VoucherDate, OrganizationId,
                Remarks, CreatedBy, UpdatedBy, CreatedAt, UpdatedAt)
                VALUES (
                 @Id, @VoucherNumber, @VoucherTypeId, @VoucherDate, @OrganizationId,
                 @Remarks, @CreatedBy, @UpdatedBy, @CreatedAt, @UpdatedAt);";

              await connection.ExecuteAsync(insertVoucherSql, vouchers, transaction);


                string insertTransactionSql = @"INSERT INTO Transactions (
                  Id, OrganizationId, VoucherId, LedgerId, Amount,
                  IsDebit, Narration, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy )
                   VALUES (
                  @Id, @OrganizationId, @VoucherId, @LedgerId, @Amount,
                  @IsDebit, @Narration, @CreatedAt, @CreatedBy, @UpdatedAt, @UpdatedBy);";

                foreach (var txn in transactions)
                {
                    var txnEntity = new Transactions
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = organizationId,
                        VoucherId = vouchers.Id,
                        LedgerId = txn.LedgerId,
                        Amount = txn.Amount,
                        IsDebit = txn.IsDebit,
                        Narration = txn.Narration,
                        CreatedAt = DateTime.Now,
                        CreatedBy = CreatedBy,
                       
                    };

                    await connection.ExecuteAsync(insertTransactionSql, txnEntity, transaction);
                }

                transaction.Commit();
                return  true;


            }
            catch (Exception ex)
            {
                transaction.Rollback();

                throw;


            }
        }
    }
}
