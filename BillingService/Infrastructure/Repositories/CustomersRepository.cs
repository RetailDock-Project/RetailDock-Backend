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
        public async Task<List<Sales>> GetAllCustomers(Guid orgId)
        {
            try
            {
                var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).Where(c=>c.OrganisationId==orgId).ToListAsync();
                //var sales = await context.Sales.Where(x => x.OrganisationId == orgId).ToListAsync();
                return sales;



            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "error from fetching all  sales details");

                throw;
            }
        }  
        public async Task<List<CreditCustomers>> GetCreditCustomers(Guid orgId)
        {
            try
            {
                var creditCustomers = await context.CreditCustomers.Where(c=>c.OrganisationId==orgId).ToListAsync();
                //var sales = await context.Sales.Where(x => x.OrganisationId == orgId).ToListAsync();
                return creditCustomers;



            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "error from fetching all  credit customers");

                throw;
            }
        }
        public async Task<List<CashCustomers>> fetchAllCashCustomers(Guid orgId)
        {
            var customers = await context.CashCustomers.Where(c=>c.OrganisationId==orgId).ToListAsync();
            return customers;
        }

        public async Task<List<CreditCustomers>> fetchAllCreditCustomers(Guid orgId)
        {
            var customers = await context.CreditCustomers.Where(cr=>cr.OrganisationId==orgId).ToListAsync();
            return customers;
        }
        public async Task<CashCustomers> fetchCashCustomersById(Guid customerId,Guid orgId)
        {
            var customers = await context.CashCustomers.FirstOrDefaultAsync(x => x.Id == customerId&& x.OrganisationId==orgId);
            return customers;
        }
        public async Task<CreditCustomers> fetchCreditCustomersById(Guid customerId,Guid orgId)
        {
            var customers = await context.CreditCustomers.FirstOrDefaultAsync(x => x.Id == customerId&& x.OrganisationId==orgId);
            return customers;
        }
        public async Task<CashCustomers> fetchCashCustomerSaleDetailsById(Guid customerId,Guid orgId)
        {
            var customers = await context.CashCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .FirstOrDefaultAsync(x => x.Id == customerId&& x.OrganisationId==orgId);
            return customers;
        }
        public async Task<CreditCustomers> fetchCreditCustomerSaleDetailsById(Guid customerId,Guid orgId)
        {
            var customers = await context.CreditCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .FirstOrDefaultAsync(x => x.Id == customerId&& x.OrganisationId==orgId);
            return customers;
        }
        public async Task<List<CreditCustomers>> fetchCreditCustomerSaleDetailsByDate(DateTime fromDate, DateTime toDate,Guid orgId)
        {
            var customers = await context.CreditCustomers.Include(c => c.Sales).ThenInclude(s => s.SaleItems)
                .Include(c => c.Sales).ThenInclude(s => s.Invoices)
                .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate&&x.OrganisationId==orgId).ToListAsync();
            return customers;
        }

        public async Task<CreditCustomers> fetchCreditCusomersByMobile(string mobile,Guid orgId)
        {
            var creditCustomers = await context.CreditCustomers.FirstOrDefaultAsync(x => x.ContactNumber == mobile && x.OrganisationId == orgId);



            return creditCustomers;

        }
        public async Task<CashCustomers> fetchCashCusomersByMobile(string mobile,Guid orgId)
        {
            var Customer = await context.CashCustomers.FirstOrDefaultAsync(x => x.ContactNumber == mobile&& x.OrganisationId==orgId);


            return Customer;

        }


        public async Task AddNewCashCustomer(CreateCustomerDto customer, Guid orgId, Guid userId)
        {
            var newCustomer = new CashCustomers { Id = Guid.NewGuid(), CustomerName = customer.Name, ContactNumber = customer.PhoneNumber, OrganisationId = orgId, Email = customer.Email, CreatedBy = userId };

            await context.CashCustomers.AddAsync(newCustomer);
        }
        public async Task AddNewCrditCustomer(CreateCustomerDto customer, Guid orgId, Guid userId)
        {
            var newCustomer = new CreditCustomers { Id = Guid.NewGuid(), CustomerName = customer.Name, ContactNumber = customer.PhoneNumber, OrganisationId = orgId, Email = customer.Email, CreatedBy = userId, Place = customer.Place };

            await context.CreditCustomers.AddAsync(newCustomer);
        }
        public async Task AddNewB2BCustomers(CreateCustomerDto customer, Guid orgId, Guid userId)
        {
            var newCustomer = new CreditCustomers {Id=Guid.NewGuid(), CustomerName = customer.Name, ContactNumber = customer.PhoneNumber, OrganisationId = orgId, Email = customer.Email, CreatedBy = userId, Place = customer.Place, GstNumber = customer.GstNumber };

            await context.CreditCustomers.AddAsync(newCustomer);
        }


    }
}