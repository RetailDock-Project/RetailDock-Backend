using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repository_Interfaces;
using Common.ResponseDto;
using Domain.Entites;
using Infrastructure.BillingContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop.Infrastructure;
using MySqlX.XDevAPI.Relational;

namespace Infrastructure.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly BillingDbContext context;
        private readonly ILogger<SaleRepository> logger;

        public SaleRepository(BillingDbContext _context, ILogger<SaleRepository> _logger)
        {
            context = _context;
            logger = _logger;
        }
        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }
        public async Task AddnewB2CSaleInvoices(List<SaleItems> saleItems, CreateSaleIdsDto allIdsDto)
        {
            try
            {

                var invoiceNumber = await GenerateB2CInvoiceNumber(allIdsDto.OrganisationId);

                var taxable = saleItems.Sum(si => si.TaxableAmount);
                var totalDiscount = saleItems.Sum(si => si.DiscountAmount);
                var totalIGST = saleItems.Sum(si => si.IGST);
                var totalCGST = saleItems.Sum(si => si.CGST);
                var totalSGST = saleItems.Sum(si => si.SGST);
                var totalUGST = saleItems.Sum(si => si.UGST);
                var totalAmt = saleItems.Sum(si => si.TotalAmount);

                var newInvoice = new SalesInvoice
                {
                    Id = allIdsDto.InvoiceId,
                    B2CInvoiceNumber = invoiceNumber,

                    OrganisationId = allIdsDto.OrganisationId,
                    TotalCGST = totalCGST,
                    TotalSGST = totalSGST,
                    TotalIGST = totalIGST,
                    TotalUGST = totalUGST,
                    TaxableAmount = taxable,
                    DiscountAmount = totalDiscount,

                    TotalAmount = totalAmt
                };

                await context.SalesInvoices.AddAsync(newInvoice);


            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "error from adding new invoices");

                throw;
            }
        }
        public async Task AddnewB2BSaleInvoices(List<SaleItems> saleItems, CreateSaleIdsDto allIdsDto)
        {
            try
            {

                var invoiceNumber = await GenerateB2BInvoiceNumber(allIdsDto.OrganisationId);


                var taxable = saleItems.Sum(si => si.TaxableAmount);
                var totalDiscount = saleItems.Sum(si => si.DiscountAmount);
                var totalIGST = saleItems.Sum(si => si.IGST);
                var totalCGST = saleItems.Sum(si => si.CGST);
                var totalSGST = saleItems.Sum(si => si.SGST);
                var totalUGST = saleItems.Sum(si => si.UGST);
                var totalAmt = saleItems.Sum(si => si.TotalAmount);

                var newInvoice = new SalesInvoice
                {
                    Id = allIdsDto.InvoiceId,
                    B2BInvoiceNumber = invoiceNumber,
                    TotalCGST = totalCGST,
                    TotalSGST = totalSGST,
                    TotalIGST = totalIGST,
                    TotalUGST = totalUGST,
                    OrganisationId = allIdsDto.OrganisationId,

                    TaxableAmount = taxable,
                    DiscountAmount = totalDiscount,

                    TotalAmount = totalAmt
                };

                await context.SalesInvoices.AddAsync(newInvoice);


            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "error from adding new invoices");

                throw;
            }
        }
        public async Task<ResponseDto<object>> AddNewCreditSale(SalesAddDto sales, CreateSaleIdsDto allIdsDto)
        {
            //fetch hsncode and taxrate from productId

            var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var creditCustomerId = await context.CreditCustomers.Where(x => x.ContactNumber == sales.MobileNum).Select(x => x.Id).FirstOrDefaultAsync();
                if(creditCustomerId==null)
                {
                    
                        return new ResponseDto<object> { StatusCode = 404, Message = "customer not found,please add new customer" };
                    
                }
      
                List<SaleItems> inMemorySale = new List<SaleItems>();
                foreach (var item in sales.SaleItems)
                {
                    decimal taxRate = 11;
                    int hsnCode = 12345;
                    decimal unitCost = 110;
                    decimal taxableAmount = item.Quantity * item.UnitPrice;

                    decimal taxAmount = (taxableAmount - item.DiscountAmount) * (taxRate / 100);

                    decimal _SGST = taxAmount / 2;
                    decimal _CGST = taxAmount / 2;
                    decimal _UGST = taxAmount / 2;

                    decimal totalAmount = (taxableAmount - item.DiscountAmount) + (taxAmount);
                    SaleItems newItems = new SaleItems();
                    if (sales.GST_Type == GST_Type.SGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity,UnitId=item.UnitId,UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, SGST = _SGST,UnitCost=unitCost };
                    }
                    if (sales.GST_Type == GST_Type.UGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, UnitId = item.UnitId ,TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount,TaxableAmount = taxableAmount, CGST = _CGST, UGST = _UGST,UnitCost=unitCost };
                    }
                    if (sales.GST_Type == GST_Type.IGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, UnitId = item.UnitId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, IGST = taxAmount ,UnitCost=unitCost};
                    }


                    inMemorySale.Add(newItems);
                };

                await context.SaleItems.AddRangeAsync(inMemorySale);
                var totalUnitCost = inMemorySale.Sum(x => x.UnitCost);
                var newSale = new Sales { Id = allIdsDto.SaleId, InvoiceId = allIdsDto.InvoiceId, DebtorsId = creditCustomerId, PaymentType = sales.PaymentType, CreatedBy = allIdsDto.UserId, OrganisationId = allIdsDto.OrganisationId, DueDate = sales.DueDate, Narration = sales.Text, GST_Type = sales.GST_Type,TotalUnitCost=totalUnitCost };
                await context.Sales.AddAsync(newSale);
                if (sales.SalesMode ==SalesMode.B2C)
                {
                    await AddnewB2CSaleInvoices(inMemorySale, allIdsDto);
                }
                if (sales.SalesMode == SalesMode.B2B)
                {
                    await AddnewB2BSaleInvoices(inMemorySale, allIdsDto);
                }


                await transaction.CommitAsync();
                return new ResponseDto<object> { StatusCode = 201, Message = "new credit sale is created" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex.Message, "error from adding new sales");

                throw;
            }
        }
        public async Task<ResponseDto<object>> AddNewCashSale(SalesAddDto sales, CreateSaleIdsDto allIdsDto)
        {
            //fetch hsncode and taxrate from productId

            var transaction = await context.Database.BeginTransactionAsync();
            try
            {

                var cashCustomerId= await context.CashCustomers.Where(x => x.ContactNumber == sales.MobileNum).Select(x=>x.Id).FirstOrDefaultAsync();

                if (cashCustomerId == null)
                {
                    return new ResponseDto<object> { StatusCode = 404, Message = "customer not found,please add new customer" };
                }
               
                List<SaleItems> inMemorySale = new List<SaleItems>();
                foreach (var item in sales.SaleItems)
                {
                    decimal taxRate = 11;
                    int hsnCode =12345;
                    decimal unitCost = 110;
                    decimal taxableAmount = item.Quantity * item.UnitPrice;

                    decimal taxAmount = (taxableAmount - item.DiscountAmount) * (taxRate / 100);

                    decimal _SGST = taxAmount / 2;
                    decimal _CGST = taxAmount / 2;
                    decimal _UGST = taxAmount / 2;

                    decimal totalAmount = (taxableAmount - item.DiscountAmount) + (taxAmount);
                    SaleItems newItems = new SaleItems();
                    if (sales.GST_Type == GST_Type.SGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, SGST = _SGST, UnitId = item.UnitId ,UnitCost=unitCost};
                    }
                    if (sales.GST_Type == GST_Type.UGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, UGST = _UGST, UnitId = item.UnitId,UnitCost=unitCost };
                    }
                    if (sales.GST_Type == GST_Type.IGST)
                    {
                        newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, IGST = taxAmount, UnitId = item.UnitId,UnitCost=unitCost };
                    }



                    inMemorySale.Add(newItems);
                };

                await context.SaleItems.AddRangeAsync(inMemorySale);
                var totalUnitCost=inMemorySale.Sum(x => x.UnitCost);    
                var newSale = new Sales { Id = allIdsDto.SaleId, InvoiceId = allIdsDto.InvoiceId, CashCustomerId = cashCustomerId, PaymentType = sales.PaymentType, CreatedBy = allIdsDto.UserId, OrganisationId = allIdsDto.OrganisationId, GST_Type = sales.GST_Type,TotalUnitCost=totalUnitCost };

                await context.Sales.AddAsync(newSale);

                await AddnewB2CSaleInvoices(inMemorySale, allIdsDto);




                await transaction.CommitAsync();
                return new ResponseDto<object> { StatusCode = 201, Message = "new cash sale is created" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex.Message, "error from adding new sales");

                throw;
            }
        }
        public async Task<List<Sales>> GetAllSalesDetails(Guid orgId)
        {
            try
            {
                var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Where(s=>s.OrganisationId==orgId).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).ToListAsync();
          
             
                //var sales = await context.Sales.Where(x => x.OrganisationId == orgId).ToListAsync();
                return sales;



            }
            catch (Exception ex)
            {

                logger.LogError(ex.Message, "error from fetching all  sales details");

                throw;
            }
        }
        public async Task<Sales> GetSalesDetailsById(Guid saleId,Guid orgId)
        {

            try
            {
                var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Id == saleId && x.OrganisationId==orgId);

                return  sales;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error from fetching  sales details By Id");

                throw;
            }
        }
        public async Task<Sales> GetB2CSalesDetailsByInvoice(string invoiceNum,Guid orgId)
        {

            try
            {
                var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Invoices.B2CInvoiceNumber == invoiceNum&& x.OrganisationId==orgId);

                return sales;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error from fetching  sales details By Invoice");

                throw;
            }
        }
        public async Task<Sales> GetB2BSalesDetailsByInvoice(string invoiceNum,Guid orgId)
        {

            try
            {
                var sales =await  context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Invoices.B2BInvoiceNumber == invoiceNum&& x.OrganisationId==orgId);

                return  sales;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error from fetching  sales details By Invoice");

                throw;
            }
        }
        public async Task<List<Sales>> GetSaleDetailsByDate(DateTime fromDate, DateTime toDate,Guid orgId)
        {

            try
            {
                var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate&& x.OrganisationId==orgId).OrderBy(x=>x.CreatedAt).ToListAsync();

                return sales;

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "error from fetching  sales details By Date");

                throw;
            }
        }


        public async Task<string> GenerateB2CInvoiceNumber(Guid orgId)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int year = currentDate.Year;
            string prefix = year.ToString();

            var lastInvoice = await context.SalesInvoices
                .Where(si => si.B2CInvoiceNumber.StartsWith(prefix + "B2C") && si.OrganisationId == orgId)
                .OrderByDescending(x => x.B2CInvoiceNumber)
                .Select(x => x.B2CInvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastInvoice) && lastInvoice.Length > prefix.Length + 3)
            {
                string lastsequence = lastInvoice.Substring(prefix.Length + 3);
                if (int.TryParse(lastsequence, out int lastseq))
                {
                    nextSequence = lastseq + 1;
                }
            }

            string fullInvoice = $"{prefix}B2C{nextSequence.ToString("D3")}";
            return fullInvoice;
        }



        public async Task<string> GenerateB2BInvoiceNumber(Guid orgId)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int year = currentDate.Year;
            string prefix = year.ToString();

            var lastInvoice = await context.SalesInvoices
                .Where(si => si.B2BInvoiceNumber.StartsWith(prefix + "B2B")&& si.OrganisationId==orgId)
                .OrderByDescending(x => x.B2BInvoiceNumber)
                .Select(x => x.B2BInvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastInvoice) && lastInvoice.Length > prefix.Length + 3)
            {
                string lastsequence = lastInvoice.Substring(prefix.Length + 3);
                if (int.TryParse(lastsequence, out int lastseq))
                {
                    nextSequence = lastseq + 1;
                }
            }

            string fullInvoice = $"{prefix}B2B{nextSequence.ToString("D3")}";
            return fullInvoice;
        }


        //string prefix = month.ToString("D2") + year.ToString();  // e.g., "052025"
        //long prefixNumber = long.Parse(prefix);                  // Convert "052025" → 52025


        //var lastInvoice = context.SalesInvoices
        //    .Where(x => x.InvoiceNumber.ToString().StartsWith(prefix))
        //    .OrderByDescending(x => x.InvoiceNumber)
        //    .Select(x => x.InvoiceNumber)
        //    .FirstOrDefault();

        //int nextSequence = 1;

        //if (lastInvoice != 0)
        //{
        //    // Extract the last 3 digits of the invoice number
        //    string lastSeqStr = lastInvoice.ToString().Substring(prefix.Length);
        //    if (int.TryParse(lastSeqStr, out int lastSeq))
        //    {
        //        nextSequence = lastSeq + 1;
        //    }
        //}

        //// Combine prefix and sequence
        //string fullInvoice = $"{prefix}{nextSequence.ToString("D3")}";  // e.g., "052025002"
        //return long.Parse(fullInvoice); // Return as long to support large numbers


    }

}

