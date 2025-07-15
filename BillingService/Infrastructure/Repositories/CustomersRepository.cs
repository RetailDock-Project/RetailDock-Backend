using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Domain.Entites;
using Infrastructure.BillingContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Repositories
{
    public class CustomersRepository : ICustomerRepository
    {
        private readonly BillingDbContext context;
        private readonly ILogger<CustomersRepository> logger;

        public CustomersRepository(BillingDbContext _context, ILogger<CustomersRepository> _logger)
        {
            context = _context;
            logger = _logger;
        }
        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Sales>> GetAllCustomers(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
        {
            try
            {
                var query = context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Products)
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.UnitOfMeasures)
                    .Include(s => s.Invoices)
        .Include(s => s.CashCustomers)
                    .Include(s => s.CreditCustomers)
                    .Where(s => s.OrganisationId == orgId)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(s => s.CreatedBy == userId);
                }

                // Order by most recent sale (adjust the field if needed)
                query = query.OrderByDescending(s => s.CreatedAt);

                // Apply pagination
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
                logger.LogError(ex, "Error fetching all sales details");
                throw;
            }
        }

        public async Task<List<CashCustomers>> fetchAllCashCustomers(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
        {
            try
            {
                var query = context.CashCustomers
                    .Where(c => c.OrganisationId == orgId)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(c => c.CreatedBy == userId);
                }

                query = query.OrderByDescending(c => c.CreatedAt); // Adjust if you have another field for latest entry

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
                logger.LogError(ex, "Error fetching cash customers");
                throw;
            }
        }

        public async Task<List<CreditCustomers>> fetchAllCreditCustomers(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
        {
            try
            {
                var query = context.CreditCustomers
                    .Where(cr => cr.OrganisationId == orgId)
                    .AsQueryable();

                if (!isFullData)
                {
                    query = query.Where(cr => cr.CreatedBy == userId);
                }

                query = query.OrderByDescending(cr => cr.CreatedAt); // Adjust if another field is used for ordering

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
                logger.LogError(ex, "Error fetching credit customers");
                throw;
            }
        }

        public async Task<CashCustomers> fetchCashCustomersById(Guid customerId, Guid orgId)
        {
            var customers = await context.CashCustomers.FirstOrDefaultAsync(x => x.Id == customerId && x.OrganisationId == orgId);
            return customers;
        }
        public async Task<CreditCustomers> fetchCreditCustomersById(Guid customerId, Guid orgId)
        {
            var customers = await context.CreditCustomers.FirstOrDefaultAsync(x => x.Id == customerId && x.OrganisationId == orgId);
            return customers;
        }
        public async Task<CashCustomers> fetchCashCustomerSaleDetailsById(Guid customerId, Guid orgId)
        {
            var customers = await context.CashCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si=>si.Products).Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .FirstOrDefaultAsync(x => x.Id == customerId && x.OrganisationId == orgId);
            return customers;
        }
        public async Task<CreditCustomers> fetchCreditCustomerSaleDetailsById(Guid customerId, Guid orgId)
        {
            var customers = await context.CreditCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si=>si.Products).Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .FirstOrDefaultAsync(x => x.Id == customerId && x.OrganisationId == orgId);
            return customers;
        }
        public async Task<List<CreditCustomers>> fetchCreditCustomerSaleDetailsByDate(DateTime fromDate, DateTime toDate, Guid orgId)
        {
            var customers = await context.CreditCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si=>si.Products).Include(c => c.Sales).ThenInclude(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate && x.OrganisationId == orgId).ToListAsync();
            return customers;
        }
     
        public async Task<CreditCustomers> fetchCreditCusomersByMobile(string mobile, Guid orgId)
        {
            var creditCustomers = await context.CreditCustomers.FirstOrDefaultAsync(x => x.ContactNumber == mobile && x.OrganisationId == orgId);



            return creditCustomers;

        }
        public async Task<CashCustomers> fetchCashCusomersByMobile(string mobile, Guid orgId)
        {
            var Customer = await context.CashCustomers.FirstOrDefaultAsync(x => x.ContactNumber == mobile && x.OrganisationId == orgId);


            return Customer;

        }


        public async Task AddNewCashCustomer(CreateCashCustomerDto customer, Guid orgId, Guid userId)
        {
            var newCustomer = new CashCustomers { Id = Guid.NewGuid(), CustomerName = customer.CompanyName, ContactNumber = customer.PhoneNumber, OrganisationId = orgId, Email = customer.Email, CreatedBy = userId, CreatedAt = DateTime.Now, LedgerId = customer.LedgerId };

            await context.CashCustomers.AddAsync(newCustomer);
        }
        public async Task AddNewCrditCustomer(CreateCustomerDto customer, Guid orgId, Guid userId, string ledgerId)
        {
            var newCustomer = new CreditCustomers { Id = Guid.NewGuid(), CustomerName = customer.contactName, ContactNumber = customer.PhoneNumber, OrganisationId = orgId, Email = customer.Email, CreatedBy = userId, Place = customer.Place, GstNumber = customer.GstNumber, CreatedAt = DateTime.Now, LedgerId = Guid.Parse(ledgerId) };

            await context.CreditCustomers.AddAsync(newCustomer);
        }



    }
}