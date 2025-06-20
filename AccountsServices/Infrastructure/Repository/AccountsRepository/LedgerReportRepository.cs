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
    //public class LedgerReportRepository:ILedgerReportRepository
    //{
    //    private readonly DapperConection _dapperConection;
    //    public LedgerReportRepository(DapperConection adapperConection)
    //    {
    //        _dapperConection = adapperConection;    
    //    }
    //  public async  Task<LedgerReportDTO> GetLedgerDetailsAsync(Guid organizationId, Guid ledgerId, DateTime? startDate, DateTime? endDate)
    //    {
    //        var start = startDate ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
    //        var end = endDate ?? DateTime.UtcNow;


    //        // Get Ledger Info (Name,  Opening, Opening Type)
    //        var sql = @"SELECT LedgerName, OpeningBalance, DrCr FROM Ledgers WHERE Id = @LedgerId";
    //        var connection =_dapperConection.CreateConnection();
    //        var ledgerInfo = await connection.QueryFirstOrDefaultAsync<(string LedgerName, decimal OpeningBalance, string DrCr)>(
    //             sql, new { LedgerId = ledgerId });
    //        if (ledgerInfo.LedgerName == null)
    //        {
    //            return null;
    //        }
    //    }
    //}
}
