using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repository_Interfaces;
using Infrastructure.BillingContext;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories
{
    public  class InvoiceRepository:I_InvoiceRepository
    {
        private readonly BillingDbContext context;
        private readonly ILogger<InvoiceRepository> logger;

        public InvoiceRepository(BillingDbContext _context, ILogger<InvoiceRepository> _logger)
        {
            context = _context;
            logger = _logger;
        }



    }
}
