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
    public interface IAccountsRepository
    {
         Task<string> GenerateVoucherNumber(Guid organizationId, Guid voucherTypeId);
        //Task<bool> AddVoucherEntry(Guid organizationId, Guid CreatedBy);
        Task<bool> AddVoucherEntrys(Guid organizationId, Guid CreatedBy, Vouchers vouchers, List<TransactionsDTO> transactions);
    }
}
