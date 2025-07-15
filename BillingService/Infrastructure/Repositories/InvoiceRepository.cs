using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Repository_Interfaces;
using Domain.Entites;
using Infrastructure.BillingContext;
using Microsoft.EntityFrameworkCore;
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
        public async Task<List<SalesInvoice>> getAllSaleInvoices(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
        {
            try
            {
                var query = context.SalesInvoices
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.Products)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.UnitOfMeasures)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CashCustomers)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CreditCustomers)
                    .Where(x => x.OrganisationId == orgId)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(x => x.Sales.CreatedBy == userId);
                }

                query = query.OrderByDescending(x => x.CreatedAt); // Ensure SalesInvoice has a CreatedDate field

                if (skip.HasValue && take.HasValue)
                {
                    query = query.Skip(skip.Value).Take(take.Value);
                }
                else if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching sales invoices");
                throw;
            }
        }



        public async Task<SalesInvoice> getSaleInvoicesByB2BInvoiceNumber(Guid orgId,string invoiceNum)
        {
return await  context.SalesInvoices.Include(si => si.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.Products).Include(si => si.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures).Include(si=>si.Sales).ThenInclude(s=>s.CashCustomers).Include(si => si.Sales).ThenInclude(s => s.CreditCustomers).FirstOrDefaultAsync(x=>x.OrganisationId==orgId && x.B2BInvoiceNumber == invoiceNum);
        }
        public async Task<SalesInvoice> getSaleInvoicesByB2CInvoiceNumber(Guid orgId, string invoiceNum)
        {
            return await context.SalesInvoices.Include(si => si.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.Products).Include(si => si.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures).Include(si=>si.Sales).ThenInclude(s=>s.CashCustomers).Include(si => si.Sales).ThenInclude(s => s.CreditCustomers).AsQueryable().FirstOrDefaultAsync(x => x.OrganisationId == orgId && x.B2CInvoiceNumber == invoiceNum); 
        }
        public async Task<List<SalesReturnInvoice>> getAllSaleReturnInvoices(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
        {
            try
            {
                var query = context.SalesReturnInvoice
                    .Include(si => si.SalesReturn)
                        .ThenInclude(sr => sr.SalesReturnItems)
                            .ThenInclude(sri => sri.Products)
                    .Include(si => si.SalesReturn)
                        .ThenInclude(sr => sr.SalesReturnItems)
                            .ThenInclude(sri => sri.UnitOfMeasures)
                    .Include(si => si.SalesReturn)
                        .ThenInclude(sr => sr.Sales)
                            .ThenInclude(s => s.CashCustomers)
                    .Include(si => si.SalesReturn)
                        .ThenInclude(sr => sr.Sales)
                            .ThenInclude(s => s.CreditCustomers)
                    .Where(si => si.OrganisationId == orgId)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(si => si.SalesReturn.CreatedBy == userId);
                }

                query = query.OrderByDescending(si => si.CreatedAt); // Make sure this field exists

                if (skip.HasValue && take.HasValue)
                {
                    query = query.Skip(skip.Value).Take(take.Value);
                }
                else if (take.HasValue)
                {
                    query = query.Take(take.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching Sales Return Invoices");
                throw;
            }
        }

        public async Task<SalesReturnInvoice> getSaleReturnByB2BInvoiceNumber(Guid orgId,string invoiceNum)
        {

            return await context.SalesReturnInvoice.Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.Products).Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.UnitOfMeasures).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CashCustomers).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CreditCustomers).FirstOrDefaultAsync(sr=>sr.OrganisationId ==orgId && sr.B2BReturnInvoiceNumber==invoiceNum);

        }    
        public async Task<SalesReturnInvoice> getSaleReturnByB2CInvoiceNumber(Guid orgId,string invoiceNum)
        {

            return await context.SalesReturnInvoice.Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.Products).Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.UnitOfMeasures).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CashCustomers).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CreditCustomers).FirstOrDefaultAsync(sr=>sr.OrganisationId ==orgId && sr.B2CReturnInvoiceNumber==invoiceNum);

        }
        public async Task<List<SalesInvoice>> getSaleInvoicesByDueDate(Guid orgId, Guid userId, bool isFullData, DateTime fromdate, DateTime todate)
        {
            try
            {
                var query = context.SalesInvoices
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.Products)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.UnitOfMeasures)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CashCustomers)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CreditCustomers)
                    .Where(si => si.OrganisationId == orgId && si.DueDate >= fromdate && si.DueDate <= todate)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(si => si.Sales.CreatedBy == userId);
                }

                query = query.OrderByDescending(si => si.DueDate);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching sales invoices by due date");
                throw;
            }
        } 
        public async Task<List<SalesInvoice>> getSaleInvoicesByDate(Guid orgId, Guid userId, bool isFullData, DateTime fromdate, DateTime todate)
        {
            try
            {
                var query = context.SalesInvoices
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.Products)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.UnitOfMeasures)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CashCustomers)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CreditCustomers)
                    .Where(si => si.OrganisationId == orgId && si.CreatedAt >= fromdate && si.CreatedAt <= todate)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(si => si.Sales.CreatedBy == userId);
                }

                query = query.OrderByDescending(si => si.CreatedAt);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching sales invoices by due date");
                throw;
            }
        }

        public async Task<List<SalesInvoice>> getPendingSalesInvoices(Guid orgId, Guid userId, bool isFullData, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var query = context.SalesInvoices
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.Products)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.SaleItems)
                            .ThenInclude(si => si.UnitOfMeasures)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CashCustomers)
                    .Include(si => si.Sales)
                        .ThenInclude(s => s.CreditCustomers)
                    .Where(si => si.OrganisationId == orgId && si.RecievedAmount != si.TotalAmount)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(si => si.Sales.CreatedBy == userId);
                }

                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(si => si.DueDate >= fromDate.Value && si.DueDate <= toDate.Value);
                }
                else if (fromDate.HasValue)
                {
                    query = query.Where(si => si.DueDate >= fromDate.Value);
                }
                else if (toDate.HasValue)
                {
                    query = query.Where(si => si.DueDate <= toDate.Value);
                }

                query = query.OrderByDescending(si => si.DueDate);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching pending sales invoices");
                throw;
            }
        }

        public async  Task<List<SalesReturnInvoice>> getSaleReturnInvoicesByDate(Guid orgId, Guid userId, bool isFullData, DateTime fromDate, DateTime toDate)
        {

            var query= context.SalesReturnInvoice.Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.Products).Include(si => si.SalesReturn).ThenInclude(s => s.SalesReturnItems).ThenInclude(si => si.UnitOfMeasures).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CashCustomers).Include(si => si.SalesReturn).ThenInclude(s => s.Sales).ThenInclude(s => s.CreditCustomers).Where(sr => sr.OrganisationId == orgId && sr.CreatedAt >= fromDate && sr.CreatedAt <= toDate).AsQueryable();


            if (!isFullData)
            {
                query = query.Where(sr => sr.SalesReturn.CreatedBy == userId);
            }
            query = query.OrderByDescending(si => si.CreatedAt);

            return await query.ToListAsync();

        }
    }
}
