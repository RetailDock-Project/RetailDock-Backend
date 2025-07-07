using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Application.Interfaces.IRepository;
using Dapper;
using Infrastructure.DapperContext;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repository.AccountsRepository
{
    public  class AccountsReportRepository:IAccountsReportRepository
    {
        private readonly DapperConection _dapperConnection;
        private readonly ILogger<AccountsReportRepository> logger;
        public AccountsReportRepository(DapperConection dapperConnection, ILogger<AccountsReportRepository> _logger)
        {
            _dapperConnection = dapperConnection;
            logger = _logger;   

        }
        public async Task<(GrossProfitDto Gross, ProfitAndLossDto PL)> GetPLRawDataAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            using var connection = _dapperConnection.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var gross = await connection.QuerySingleAsync<dynamic>(
                    "CALL GetGrossProfit(@OrgId, @From, @To)",
                    new { OrgId = organizationId, From = fromDate, To = toDate },
                    transaction: transaction);

               
                var grosssInformation = new GrossProfitDto
                {
                    DirectIncomeGroupId = Guid.Parse(gross.DirectIncomeGroupId),
                    DirectExpenseGroupId =Guid.Parse(gross.DirectExpenseGroupId),
                    GrossProfit = gross.GrossProfit ?? 0,
                    NetDirectExpense = gross.NetDirectExpense ?? 0,
                    NetDirectIncome = gross.NetDirectIncome ?? 0,
                };
                var pl = await connection.QuerySingleAsync<dynamic>(
                    "CALL GetProfitAndLoss(@OrgId, @From, @To)",
                    new { OrgId = organizationId, From = fromDate, To = toDate },
                    transaction: transaction);

                var pandlInformation = new ProfitAndLossDto
                {
                    IndirectIncomeGroupId =pl.IndirectIncomeGroupId, 
                    IndirectExpenseGroupId =pl.IndirectExpenseGroupId, 
                    NetIndirectExpense = pl.NetIndirectExpense ?? 0,
                    NetIndirectIncome = pl.NetIndirectIncome ?? 0,
                };

                transaction.Commit();

                return (grosssInformation, pandlInformation);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
         
        }
       public async Task <BalanceSheetResponseDto> GetBalanceSheetSummaryAsync(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            using var connection = _dapperConnection.CreateConnection();
            var result = await connection.QueryAsync<BalanceSheetGroupSummaryDto>(
                "CALL GetBalanceSheetSummary(@OrgId, @FromDate, @ToDate)",
                new { OrgId = organizationId, FromDate = fromDate, ToDate = toDate }
            );

            var grouped = result.ToList();
            var response = new BalanceSheetResponseDto
            {
                Items = grouped,
                TotalAssets = grouped.Where(x => x.Side == "Asset").Sum(x => x.NetAmount),
                TotalLiabilities = grouped.Where(x => x.Side == "Liability").Sum(x => x.NetAmount)
            };

            return response;
        }
    }
    }


