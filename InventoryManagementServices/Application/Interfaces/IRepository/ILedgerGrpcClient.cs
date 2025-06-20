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
        public Task<Responses<object>> AddSupplierLedger(Supplier newSupplier);
    }
}
