using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.IRepository;
using Dapper;
using Infrastructure.DapperContext;

namespace Infrastructure.Repository.AccountsRepository
{
    public class LedgerReportRepository:ILedgerReportRepository
    {
        private readonly DapperConection _dapperConection;
        public LedgerReportRepository(DapperConection adapperConection)
        {
            _dapperConection = adapperConection;
        }
        public async Task<LedgerReportDTO> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var end = endDate ?? DateTime.UtcNow;


            //  Info (Name,  Opening, Opening Type)
            var sql = @"SELECT LedgerName, OpeningBalance, DrCr , Nature FROM Ledgers WHERE Id = @LedgerId";
            var connection = _dapperConection.CreateConnection();
            var ledgerInfo = await connection.QueryFirstOrDefaultAsync<(string LedgerName, decimal OpeningBalance, string DrCr,string Nature)>
                (sql, new { LedgerId = ledgerId });

            if (ledgerInfo.LedgerName == null)
            {
                return null;
            }
            //  Get all transactions before start date
            var openingTxnSql = @"
                                SELECT 
                                IFNULL(SUM(CASE WHEN T.IsDebit = 1 THEN T.Amount ELSE 0 END), 0) AS TotalDebit,
                                IFNULL(SUM(CASE WHEN T.IsDebit = 0 THEN T.Amount ELSE 0 END), 0) AS TotalCredit
                                FROM Transactions T
                                INNER JOIN Vouchers V ON V.Id = T.VoucherId
                                WHERE T.LedgerId = @LedgerId
                                AND V.OrganizationId = @OrgId
                                AND V.VoucherDate < @StartDate;";
            var txn = await connection.QuerySingleAsync<(decimal TotalDebit, decimal TotalCredit)>(
                      openingTxnSql, new { LedgerId = ledgerId, OrgId = organizationId, StartDate = startDate });



            // Adjust opening balance sign
            decimal adjustedOpening = (ledgerInfo.Nature?.ToLower() == ledgerInfo.DrCr?.ToLower())
                ? ledgerInfo.OpeningBalance
                : -ledgerInfo.OpeningBalance;

            decimal finalBalance = 0;
            string finalType = ""; // "Dr" or "Cr"

            //  Calculate net balance based on nature
            if (ledgerInfo.Nature?.ToLower() == "dr")
            {
                finalBalance = adjustedOpening + txn.TotalDebit - txn.TotalCredit;
            }
            else if (ledgerInfo.Nature?.ToLower() == "cr")
            {
                finalBalance = adjustedOpening + txn.TotalCredit - txn.TotalDebit;
            }

            //  Determine final nature
            if (finalBalance >= 0)
            {
                finalType = ledgerInfo.Nature; // retain same nature
            }
            else
            {
                finalType = ledgerInfo.Nature?.ToLower() == "dr" ? "Cr" : "Dr"; // flip it
                finalBalance = Math.Abs(finalBalance); // make positive
            }
        }
    }
}
