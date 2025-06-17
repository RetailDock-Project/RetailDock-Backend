using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Serilog.Core;

namespace Infrastructure.Repositories
{
    public class SaleReturnRepository : ISaleReturnRepository
    {
        private readonly BillingDbContext context;


        public SaleReturnRepository(BillingDbContext _context)
        {
            context = _context;

        }
        public async Task SaveChanges()
        {
            await context.SaveChangesAsync();
        }

        public async Task AddnewB2CSaleInvoices(List<SalesReturnItems> returnItems, Guid returnId, Guid returnInvoiceId, Guid orgId,PaymentMode paymentMode)

        {
            try
            {

                var invoiceNumber = await GenerateB2CReturnInvoiceNumber(orgId);

                var taxable = returnItems.Sum(si => si.TaxableAmount);
           
                var total_IGST = returnItems.Sum(si => si.IGST);
                var total_CGST = returnItems.Sum(si => si.CGST);
                var total_SGST = returnItems.Sum(si => si.SGST);
                var total_UGST = returnItems.Sum(si => si.UGST);
                var totalAmt = returnItems.Sum(si => si.TotalAmount);

                var newInvoice = new SalesReturnInvoice
                {
                    Id = returnInvoiceId,
                    B2CReturnInvoiceNumber = invoiceNumber,
                    TotalCGST = total_IGST,
                    TotalIGST = total_CGST,
                    TotalSGST = total_SGST,
                    TotalUGST = total_UGST,
                    PaymentMode=paymentMode,

                    OrganisationId = orgId,


                    TaxableAmount = taxable,

                    TotalAmount = totalAmt
                };

                await context.SalesReturnInvoice.AddAsync(newInvoice);


            }
            catch (Exception ex)
            {



                throw new Exception("Error while adding new B2C sales return", ex);
            }
        }
        public async Task AddnewB2BSaleInvoices(List<SalesReturnItems> returnItems, Guid returnId, Guid returnInvoiceId, Guid orgId,PaymentMode paymentMode)
        {
            try
            {

                var invoiceNumber = await GenerateB2BReturnInvoiceNumber(orgId);

                var taxable = returnItems.Sum(si => si.TaxableAmount);
              
                var total_IGST = returnItems.Sum(si => si.IGST);
                var total_CGST = returnItems.Sum(si => si.CGST);
                var total_SGST = returnItems.Sum(si => si.SGST);
                var total_UGST = returnItems.Sum(si => si.UGST);
                var totalAmt = returnItems.Sum(si => si.TotalAmount);


                var newInvoice = new SalesReturnInvoice
                {
                    Id = returnInvoiceId,
                    B2BReturnInvoiceNumber = invoiceNumber,
                    OrganisationId = orgId,
                    
                    TotalCGST = total_IGST,
                    TotalIGST = total_CGST,
                    TotalSGST = total_SGST,
                    TotalUGST = total_UGST,
                    PaymentMode=paymentMode,
                    TaxableAmount = taxable,

                    TotalAmount = totalAmt
                };

                await context.SalesReturnInvoice.AddAsync(newInvoice);


            }
            catch (Exception ex)
            {



                throw new Exception("Error while adding new B2B sales return", ex);
            }
        }
        public async Task<Sales> fetchSalesByInvoice(string invoiceNum, Guid orgId)
        {


            var sales = await context.Sales.Include(s => s.SaleItems).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x =>( x.Invoices.B2CInvoiceNumber == invoiceNum||x.Invoices.B2BInvoiceNumber==invoiceNum) && x.OrganisationId == orgId);

            return sales;

        }



        public async Task addNewB2BSalesReturn(AddSalesReturnDto salesReturn, Guid saleId, Guid orgId, Guid userId, GST_Type gst_Type)
        {
            {
                var transaction = await context.Database.BeginTransactionAsync();


                try
                {


                    Guid returnInvoiceId = Guid.NewGuid();
                    Guid returnId = Guid.NewGuid();




                    List<SalesReturnItems> inMemoryReturnSaleItems = new List<SalesReturnItems>();
                    foreach (var returnProduct in salesReturn.ReturnItems)
                    {
                        var _saleItem = await context.SaleItems.Include(x => x.Sales).ThenInclude(x => x.Invoices).FirstOrDefaultAsync(x => x.SaleId == saleId && x.ProductId == returnProduct.ProductId);

                        decimal taxableAmount = _saleItem.UnitPrice * returnProduct.Quantity;
                        decimal taxAmount = taxableAmount * (_saleItem.TaxRate / 100);

                        decimal CGST = taxAmount / 2;
                        decimal SGST = taxAmount / 2;
                        decimal UGST = taxAmount / 2;


                        decimal totalAmount = taxableAmount + taxAmount;



                        SalesReturnItems returnItem = new SalesReturnItems();

                        if (gst_Type == GST_Type.SGST)
                        {
                            returnItem = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity, UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, CGST = CGST, SGST = SGST };
                        }
                        if (gst_Type == GST_Type.UGST)
                        {
                            returnItem = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity, UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, CGST = CGST, UGST = UGST };
                        }
                        if (gst_Type == GST_Type.IGST)
                        {
                            returnItem = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity, UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, IGST = taxAmount };

                        }
                        inMemoryReturnSaleItems.Add(returnItem);
                        await context.SalesReturnItems.AddAsync(returnItem);
                    }
                    await context.SalesReturnItems.AddRangeAsync(inMemoryReturnSaleItems);

                    PaymentMode paymentMode = salesReturn.returnPayment;
                    await AddnewB2BSaleInvoices(inMemoryReturnSaleItems, returnId, returnInvoiceId, orgId,paymentMode);

                    var totalUnitCost = inMemoryReturnSaleItems.Sum(items => items.UnitCost);

                    var _salesReturn = new SalesReturn { Id = returnId, CreatedBy = userId, TotalUnitCost = totalUnitCost, Notes = salesReturn.Text, OrganisationId = orgId, ReturnInvoiceId = returnInvoiceId, SaleId = saleId };

                    await context.SalesReturn.AddAsync(_salesReturn);


                    await transaction.CommitAsync();
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    // Log the error
                    throw new Exception("Error while adding new sales return", ex);
                }
            }
        }

        public async Task addNewB2CSalesReturn(AddSalesReturnDto salesReturn, Guid saleId, Guid orgId, Guid userId, GST_Type gst_Type)
        {
            {
                var transaction = await context.Database.BeginTransactionAsync();

                try
                {


                    Guid returnInvoiceId = Guid.NewGuid();
                    Guid returnId = Guid.NewGuid();



                    List<SalesReturnItems> inMemoryReturnSaleItems = new List<SalesReturnItems>();

                    foreach (var returnProduct in salesReturn.ReturnItems)
                    {
                        var _saleItem = await context.SaleItems.Include(x => x.Sales).ThenInclude(x => x.Invoices).FirstOrDefaultAsync(x => x.SaleId == saleId && x.ProductId == returnProduct.ProductId);

                        decimal taxableAmount = _saleItem.UnitPrice * returnProduct.Quantity;

                        decimal taxAmount = taxableAmount * (_saleItem.TaxRate / 100);

                        decimal CGST = taxAmount / 2;
                        decimal SGST = taxAmount / 2;
                        decimal UGST = taxAmount / 2;

                        SalesReturnItems returnItems = new SalesReturnItems();
                        decimal totalAmount = taxableAmount + taxAmount;
                        if (gst_Type == GST_Type.SGST)
                        {
                            returnItems = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity, UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, CGST = CGST, SGST = SGST };
                        }
                        if (gst_Type == GST_Type.UGST)
                        {
                            returnItems = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity, UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, CGST = CGST, UGST = UGST };
                        }
                        if (gst_Type == GST_Type.IGST)
                        {
                            returnItems = new SalesReturnItems { ProductId = returnProduct.ProductId, Quantity = returnProduct.Quantity,  UnitCost = _saleItem.UnitCost, HSNCodeNumber = _saleItem.HSNCodeNumber, UnitPrice = _saleItem.UnitPrice, ReturnId = returnId, TaxRate = _saleItem.TaxRate, TaxableAmount = taxableAmount, TotalAmount = totalAmount, IGST = taxAmount };
                        }

                        inMemoryReturnSaleItems.Add(returnItems);

                        await context.SalesReturnItems.AddAsync(returnItems);
                    }
                    PaymentMode paymentMode=salesReturn.returnPayment;
                    await AddnewB2CSaleInvoices(inMemoryReturnSaleItems, returnId, returnInvoiceId, orgId,paymentMode);

                    var totalUnitCost = inMemoryReturnSaleItems.Sum(items => items.UnitCost);

                    var _salesReturn = new SalesReturn { Id = returnId, CreatedBy = userId, TotalUnitCost = totalUnitCost, Notes = salesReturn.Text, OrganisationId = orgId, ReturnInvoiceId = returnInvoiceId, SaleId = saleId };
                    await context.SalesReturn.AddAsync(_salesReturn);

                    await transaction.CommitAsync();
                }

                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    throw new Exception("Error while adding new sales return", ex);
                }
            }
        }


        public async Task<List<SalesReturn>> fetchAllSalesReturn(Guid orgId)
        {
            return await context.SalesReturn.Include(sr => sr.ReturnInvoice).Include(sr => sr.SalesReturnItems).Include(sr => sr.Sales).ThenInclude(s => s.CashCustomers).Include(sr => sr.Sales).ThenInclude(s => s.CreditCustomers).Where(sr => sr.OrganisationId == orgId).ToListAsync();
        }
        public async Task<SalesReturn> GetSalesReturnDetailsById(Guid returnId, Guid orgId)
        {
            return await context.SalesReturn.Include(sr => sr.ReturnInvoice).Include(sr => sr.SalesReturnItems).Include(sr => sr.Sales).ThenInclude(s => s.CashCustomers).Include(sr => sr.Sales).ThenInclude(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Id == returnId && x.OrganisationId == orgId);

        }
        public async Task<SalesReturn> GetSalesReturnDetailsByInvoice(string invoiceNum, Guid orgId)
        {
            return await context.SalesReturn.Include(sr => sr.ReturnInvoice).Include(sr => sr.SalesReturnItems).Include(sr => sr.Sales).ThenInclude(s => s.CashCustomers).Include(sr => sr.Sales).ThenInclude(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.ReturnInvoice.B2CReturnInvoiceNumber == invoiceNum || x.ReturnInvoice.B2BReturnInvoiceNumber == invoiceNum && x.OrganisationId == orgId);

        }
        public async Task<List<SalesReturn>> GetSalesReturnDetailsBydate(DateTime fromDate, DateTime? toDate, Guid orgId)
        {
            return await context.SalesReturn.Include(sr => sr.ReturnInvoice).Include(sr => sr.SalesReturnItems).Include(sr => sr.Sales).ThenInclude(s => s.CashCustomers).Include(sr => sr.Sales).ThenInclude(s => s.CreditCustomers).Where(x => x.ReturnDate >= fromDate && x.ReturnDate <= toDate && x.OrganisationId == orgId).ToListAsync();

        }





        public async Task<string> GenerateB2BReturnInvoiceNumber(Guid orgId)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int year = currentDate.Year;
            string prefix = year.ToString();

            var lastInvoice = await context.SalesReturnInvoice
                .Where(si => si.B2BReturnInvoiceNumber.StartsWith("SR" + prefix + "B2B") && si.OrganisationId == orgId)
                .OrderByDescending(x => x.B2BReturnInvoiceNumber)
                .Select(x => x.B2BReturnInvoiceNumber)
                .FirstOrDefaultAsync();

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastInvoice) && lastInvoice.Length > prefix.Length + 5)
            {

                string lastsequence = lastInvoice.Substring(prefix.Length + 5);
                if (int.TryParse(lastsequence, out int lastseq))
                {
                    nextSequence = lastseq + 1;
                }
            }

            string fullInvoice = $"SR{prefix}B2B{nextSequence.ToString("D3")}";
            return fullInvoice;
        }


        public async Task<string> GenerateB2CReturnInvoiceNumber(Guid orgId)
        {
            DateTime currentDate = DateTime.Now;
            int year = currentDate.Year;
            string prefix = year.ToString();
            var lastInvoice = await context.SalesReturnInvoice.Where(sri => sri.B2CReturnInvoiceNumber.StartsWith("SR" + prefix + "B2C") && sri.OrganisationId == orgId).OrderByDescending(x => x.B2CReturnInvoiceNumber).Select(sri => sri.B2CReturnInvoiceNumber).FirstOrDefaultAsync();

            int nextSequence = 1;
            if (!string.IsNullOrEmpty(lastInvoice) && lastInvoice.Length > prefix.Length + 5)
            {
                string lastSequence = lastInvoice.Substring(prefix.Length + 5);
                if (int.TryParse(lastSequence, out int lastseq))
                {
                    nextSequence = lastseq + 1;
                }
            }
            string fullInvoice = $"SR{prefix}B2C{nextSequence.ToString("D3")}";
            return fullInvoice;
        }


    }
}
