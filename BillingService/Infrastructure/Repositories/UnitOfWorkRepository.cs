using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repository_Interfaces;
using Infrastructure.BillingContext;

namespace Infrastructure.Repositories
{
    public  class UnitOfWorkRepository: IUnitOfWorkRepository
    {
        private BillingDbContext _context;

        public UnitOfWorkRepository(BillingDbContext context) { _context = context; }

        public async Task _BiginTransaction()
        {
            await _context.Database.BeginTransactionAsync();

        }
        public async Task _CommitTransaction()
        {
            await _context.Database.CommitTransactionAsync();
        }
        public async Task _RolBackTransaction()
        {
            await _context.Database.RollbackTransactionAsync();
        }
       public async Task _SaveChnages()
        {
            await _context.SaveChangesAsync();
        }
    }
}
