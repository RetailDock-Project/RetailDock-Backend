using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Interfaces.IRepository
{
    public interface ILedgerGrpcClient
    {
         Task<Responses<string>> AddSupplierLedger(Supplier newSupplier,decimal? openingBalance,bool? isDebit);
    }
}
