using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTOs;
using Application.Interfaces.IRepository;
using Dapper;
using Infrastructure.DapperContext;
using Infrastructure.Repository.GroupRepository;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository.AccountsRepository
{
    public class LedgerReportRepository : ILedgerReportRepository
    {
        private readonly DapperConection _dapperConection;
        ILogger<LedgerRepository> _logger;

        public LedgerReportRepository(DapperConection adapperConection, ILogger<LedgerRepository> logger)
        {
            _dapperConection = adapperConection;
            _logger = logger;
        }

       
            public async Task<LedgerDetailsReportDTO> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var end = endDate ?? DateTime.UtcNow;

            var connection = _dapperConection.CreateConnection();

            // Step 1: Get Ledger Info
            var sql = @"SELECT LedgerName, OpeningBalance, DrCr, Nature FROM Ledgers WHERE Id = @LedgerId";
            var ledgerInfo = await connection.QueryFirstOrDefaultAsync<(string LedgerName, decimal OpeningBalance, string DrCr, string Nature)>(
                sql, new { LedgerId = ledgerId });

            if (ledgerInfo.LedgerName == null)
                return null;

            // Step 2: Get Opening Transactions Before StartDate
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
                openingTxnSql, new { LedgerId = ledgerId, OrgId = organizationId, StartDate = start });

            // Step 3: Adjust Opening Balance
            decimal adjustedOpening = (ledgerInfo.Nature?.ToLower() == ledgerInfo.DrCr?.ToLower())
                ? ledgerInfo.OpeningBalance
                : -ledgerInfo.OpeningBalance;

            decimal finalOpeningBalance;
            string finalOpeningType;

            if (ledgerInfo.Nature?.ToLower() == "dr")
                finalOpeningBalance = adjustedOpening + txn.TotalDebit - txn.TotalCredit;
            else
                finalOpeningBalance = adjustedOpening + txn.TotalCredit - txn.TotalDebit;

            if (finalOpeningBalance >= 0)
            {
                finalOpeningType = ledgerInfo.Nature;
            }
            else
            {
                finalOpeningType = ledgerInfo.Nature?.ToLower() == "dr" ? "Cr" : "Dr";
                finalOpeningBalance = Math.Abs(finalOpeningBalance);
            }

            // Step 4: Get Transactions BETWEEN Start and End Date
            var betweenTxnSql = @"
        SELECT 
            IFNULL(SUM(CASE WHEN T.IsDebit = 1 THEN T.Amount ELSE 0 END), 0) AS TotalDebit,
            IFNULL(SUM(CASE WHEN T.IsDebit = 0 THEN T.Amount ELSE 0 END), 0) AS TotalCredit
        FROM Transactions T
        INNER JOIN Vouchers V ON V.Id = T.VoucherId
        WHERE T.LedgerId = @LedgerId
          AND V.OrganizationId = @OrgId
          AND V.VoucherDate BETWEEN @StartDate AND @EndDate;";

            var periodTxn = await connection.QuerySingleAsync<(decimal TotalDebit, decimal TotalCredit)>(
                betweenTxnSql, new { LedgerId = ledgerId, OrgId = organizationId, StartDate = start, EndDate = end });

            // Step 5: Adjust Closing Using Opening Type
            decimal adjustedClosing = (ledgerInfo.Nature?.ToLower() == finalOpeningType?.ToLower())
                ? finalOpeningBalance
                : -finalOpeningBalance;

            decimal closingBalance;
            string closingType;

            if (ledgerInfo.Nature?.ToLower() == "dr")
                closingBalance = adjustedClosing + periodTxn.TotalDebit - periodTxn.TotalCredit;
            else
                closingBalance = adjustedClosing + periodTxn.TotalCredit - periodTxn.TotalDebit;

            if (closingBalance >= 0)
            {
                closingType = ledgerInfo.Nature;
            }
            else
            {
                closingType = ledgerInfo.Nature?.ToLower() == "dr" ? "Cr" : "Dr";
                closingBalance = Math.Abs(closingBalance);
            }

            // Step 6: Get detailed transactions with opposite ledgers
            var transactionListSql =

   @"
   
WITH SelectedLedgerEntries AS (
    SELECT T.*
    FROM Transactions T
    INNER JOIN Vouchers V ON V.Id = T.VoucherId
    WHERE T.LedgerId = @LedgerId
      AND V.OrganizationId = @OrgId
      AND V.VoucherDate BETWEEN @StartDate AND @EndDate
),
TotalOppositeSide AS (
    SELECT 
        OT.VoucherId,
        SUM(OT.Amount) AS TotalOppAmount
    FROM Transactions OT
    INNER JOIN SelectedLedgerEntries T ON OT.VoucherId = T.VoucherId
    WHERE OT.IsDebit != T.IsDebit
    GROUP BY OT.VoucherId
),
OppositeLedgers AS (
    SELECT 
        OT.VoucherId,
        OT.LedgerId,
        L.LedgerName AS OppositeLedger,
        OT.Amount AS OppAmount,
        OT.IsDebit
    FROM Transactions OT
    INNER JOIN Ledgers L ON L.Id = OT.LedgerId
)
SELECT 
    OL.OppositeLedger,
    ROUND((OL.OppAmount / TOA.TotalOppAmount) * T.Amount, 2) AS Amount,
    V.VoucherDate,
    VT.TypeName,
    V.VoucherNumber,
    T.IsDebit
FROM SelectedLedgerEntries T
INNER JOIN Vouchers V ON T.VoucherId = V.Id
INNER JOIN VoucherTypes VT ON V.VoucherTypeId = VT.Id
INNER JOIN TotalOppositeSide TOA ON TOA.VoucherId = T.VoucherId
INNER JOIN OppositeLedgers OL ON OL.VoucherId = T.VoucherId
WHERE OL.IsDebit != T.IsDebit
  AND NOT (
      VT.TypeName IN ('Sales', 'SalesReturn') AND 
      OL.OppositeLedger LIKE '%Inventory Transaction%'  -- case-insensitive match
  );


"
;


            var transactions = (await connection.QueryAsync<LedgerReportDTO>(
                transactionListSql,
                new { LedgerId = ledgerId, OrgId = organizationId, StartDate = start, EndDate = end }
            )).ToList();
            foreach (var item in transactions)
            {
                Console.WriteLine(item.Amount);
            }
           
            // Step 7: Return Final DTO
            return new LedgerDetailsReportDTO
            {
                LedgerName = ledgerInfo.LedgerName,
                OpeningBalance = finalOpeningBalance,
                
                ClosingBalance = closingBalance,
                ClosingType = closingType,
                PeriodDr = periodTxn.TotalDebit,
                PeriodCr = periodTxn.TotalCredit,
                Transactions = transactions,
                OpeningType= finalOpeningType
                
            };
        }






        public async Task<List<LedgerSummaryDTO>> GetAllLedgerSummariesAsync(Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var end = endDate ?? DateTime.UtcNow;

            var connection = _dapperConection.CreateConnection();

            var ledgers = (await connection.QueryAsync<(Guid Id, string LedgerName, decimal OpeningBalance, string DrCr, string Nature)>(
                @"SELECT Id, LedgerName, OpeningBalance, DrCr, Nature 
          FROM Ledgers 
          WHERE OrganizationId = @OrgId AND IsActive = 1",
                new { OrgId = organizationId }
            )).ToList();

            var result = new List<LedgerSummaryDTO>();

            foreach (var ledger in ledgers)
            {
                // 1. Opening Transaction Sum
                var openingTxn = await connection.QuerySingleAsync<(decimal TotalDebit, decimal TotalCredit)>(
                    @"SELECT 
                IFNULL(SUM(CASE WHEN T.IsDebit = 1 THEN T.Amount ELSE 0 END), 0) AS TotalDebit,
                IFNULL(SUM(CASE WHEN T.IsDebit = 0 THEN T.Amount ELSE 0 END), 0) AS TotalCredit
              FROM Transactions T
              INNER JOIN Vouchers V ON V.Id = T.VoucherId
              WHERE T.LedgerId = @LedgerId
                AND V.OrganizationId = @OrgId
                AND V.VoucherDate < @StartDate",
                    new { LedgerId = ledger.Id, OrgId = organizationId, StartDate = start });

                decimal adjustedOpening = (ledger.Nature?.ToLower() == ledger.DrCr?.ToLower())
                    ? ledger.OpeningBalance
                    : -ledger.OpeningBalance;

                decimal finalOpeningBalance = (ledger.Nature?.ToLower() == "dr")
                    ? adjustedOpening + openingTxn.TotalDebit - openingTxn.TotalCredit
                    : adjustedOpening + openingTxn.TotalCredit - openingTxn.TotalDebit;

                string openingType = (finalOpeningBalance >= 0)
                    ? ledger.Nature
                    : (ledger.Nature?.ToLower() == "dr" ? "Cr" : "Dr");

                finalOpeningBalance = Math.Abs(finalOpeningBalance);

                // 2. Period Transactions
                var periodTxn = await connection.QuerySingleAsync<(decimal TotalDebit, decimal TotalCredit)>(
                    @"SELECT 
                IFNULL(SUM(CASE WHEN T.IsDebit = 1 THEN T.Amount ELSE 0 END), 0) AS TotalDebit,
                IFNULL(SUM(CASE WHEN T.IsDebit = 0 THEN T.Amount ELSE 0 END), 0) AS TotalCredit
              FROM Transactions T
              INNER JOIN Vouchers V ON V.Id = T.VoucherId
              WHERE T.LedgerId = @LedgerId
                AND V.OrganizationId = @OrgId
                AND V.VoucherDate BETWEEN @StartDate AND @EndDate",
                    new { LedgerId = ledger.Id, OrgId = organizationId, StartDate = start, EndDate = end });

                // 3. Adjust Closing
                decimal adjustedClosing = (ledger.Nature?.ToLower() == openingType?.ToLower())
                    ? finalOpeningBalance
                    : -finalOpeningBalance;

                decimal finalClosing = (ledger.Nature?.ToLower() == "dr")
                    ? adjustedClosing + periodTxn.TotalDebit - periodTxn.TotalCredit
                    : adjustedClosing + periodTxn.TotalCredit - periodTxn.TotalDebit;

                string closingType = (finalClosing >= 0)
                    ? ledger.Nature
                    : (ledger.Nature?.ToLower() == "dr" ? "Cr" : "Dr");

                finalClosing = Math.Abs(finalClosing);

                // 4. Add to result list
                result.Add(new LedgerSummaryDTO
                {
                    LedgerId = ledger.Id,
                    LedgerName = ledger.LedgerName,
                    OpeningBalance = finalOpeningBalance,
                    OpeningType = openingType,
                    PeriodDr = periodTxn.TotalDebit,
                    PeriodCr = periodTxn.TotalCredit,
                    ClosingBalance = finalClosing,
                    ClosingType = closingType
                });
            }

            return result;
        }
        public async Task<List<LedgerSummaryDTO>> GetLedgerSummaryByGroupHierarchyAsync(Guid groupId, Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            var connection = _dapperConection.CreateConnection();

            var result = await connection.QueryAsync<LedgerSummaryDTO>(
                "CALL GetLedgerSummaryByGroupHierarchy(@p_GroupId, @p_OrganizationId, @p_StartDate, @p_EndDate)",
                new
                {
                    p_GroupId = groupId,
                    p_OrganizationId = organizationId,
                    p_StartDate = startDate,
                    p_EndDate = endDate
                }
            );

            return result.ToList();
        }
        public async Task<GroupWithLedgersSummaryDTO> GetGroupAndLedgerSummaryAsync(Guid groupId, Guid organizationId, DateTime? startDate, DateTime? endDate)
        {
            using var connection = _dapperConection.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var groupSummaries = (await connection.QueryAsync<GroupLedgerSummaryDTO>(
                    "CALL GetGroupHierarchySummary(@p_GroupId, @p_OrganizationId, @p_StartDate, @p_EndDate)",
                    new
                    {
                        p_GroupId = groupId,
                        p_OrganizationId = organizationId,
                        p_StartDate = startDate,
                        p_EndDate = endDate
                    }, transaction)).ToList();

                var ledgerSummaries = (await connection.QueryAsync<DirectLedgerSummaryDTO>(
                    "CALL GetLedgerSummariesByGroup(@p_GroupId, @p_OrganizationId, @p_StartDate, @p_EndDate)",
                    new
                    {
                        p_GroupId = groupId,
                        p_OrganizationId = organizationId,
                        p_StartDate = startDate,
                        p_EndDate = endDate
                    }, transaction)).ToList();

                transaction.Commit();

                return new GroupWithLedgersSummaryDTO
                {
                    GroupSummaries = groupSummaries,
                    DirectLedgerSummaries = ledgerSummaries

                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex.Message, "Error executing group and ledger summaries");
                throw;
            }
        }
    }
}

