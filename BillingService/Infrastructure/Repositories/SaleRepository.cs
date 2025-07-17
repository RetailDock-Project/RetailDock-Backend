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


        public async Task<CashCustomers> GetCashCustomers(string phoneNUmber, Guid OrgId)
        {
            return await context.CashCustomers.FirstOrDefaultAsync(x => x.OrganisationId == OrgId && x.ContactNumber == phoneNUmber);
        }
        public async Task<CreditCustomers> GetCreditCustomers(string phoneNUmber, Guid OrgId)
        {
            return await context.CreditCustomers.FirstOrDefaultAsync(x => x.OrganisationId == OrgId && x.ContactNumber == phoneNUmber);
        }
        public async Task<List<Sales>> GetDebtorsSales(Guid debtorId, Guid orgId)
        {
            return await context.Sales.Include(s => s.Invoices).Include(s => s.SaleItems).Where(s => s.DebtorsId == debtorId).ToListAsync();
        }
        public async Task<Product> GetProductById(Guid productId, Guid orgId)



        {
            return await context.Products
                .Include(p => p.UnitOfMeasures)
                .Include(p => p.Category)
                .Include(p => p.HsnCode)
                .FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted && p.OrgnizationId == orgId);
        }


        public async Task<HsnCode> GetHsnCode(int HsnCodeId)

        {

            return await context.HsnCodes.FirstOrDefaultAsync(hsn => hsn.HsnCodeId == HsnCodeId);

        }

        public async Task AddnewB2CSaleInvoices(List<SaleItems> saleItems, CreateSaleIdsDto allIdsDto, DateTime dueDate, decimal recievedAmount)
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
                DueDate = dueDate,
                RecievedAmount = recievedAmount,

                OrganisationId = allIdsDto.OrganisationId,
                TotalCGST = totalCGST,
                TotalSGST = totalSGST,
                TotalIGST = totalIGST,
                TotalUGST = totalUGST,
                TaxableAmount = taxable,
                DiscountAmount = totalDiscount,

                TotalAmount = totalAmt,
                CreatedAt = DateTime.UtcNow
            };

            await context.SalesInvoices.AddAsync(newInvoice);


        }
        
        public async Task AddnewB2BSaleInvoices(List<SaleItems> saleItems, CreateSaleIdsDto allIdsDto, DateTime dueDate, decimal recievedAmount)
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
                RecievedAmount = recievedAmount,
                DueDate = dueDate,

                TaxableAmount = taxable,
                DiscountAmount = totalDiscount,

                TotalAmount = totalAmt,
                CreatedAt = DateTime.UtcNow
            };

            await context.SalesInvoices.AddAsync(newInvoice);



        }
        public async Task<ResponseDto<object>> AddNewCreditSale(SalesAddDto sales, CreateSaleIdsDto allIdsDto)
        {


            var creditCustomer = await GetCreditCustomers(sales.MobileNum, allIdsDto.OrganisationId);
            if (creditCustomer == null)
            {

                return new ResponseDto<object> { StatusCode = 404, Message = "customer not found,please add new customer" };

            }

            DateTime dueDate = sales.DueDate ?? DateTime.UtcNow.AddDays(30);

            List<SaleItems> inMemorySale = new List<SaleItems>();


            foreach (var item in sales.SaleItems)
            {

                var filteredProduct = await GetProductById(item.ProductId, allIdsDto.OrganisationId);

                if (filteredProduct == null)
                {
                    return new ResponseDto<object> { StatusCode = 404, Message = "product not found," };
                }

                filteredProduct.Stock -= item.Quantity;


                decimal taxRate = filteredProduct.HsnCode.GstRate;
                string hsnCode = filteredProduct.HsnCode.HSNCodeNumber;
                decimal unitCost = filteredProduct.CostPrice;
                int unitId = filteredProduct.UnitOfMeasuresId;

                decimal taxableAmount = item.Quantity * item.UnitPrice;

                decimal taxAmount = (taxableAmount - item.DiscountAmount) * (taxRate / 100);

                decimal _SGST = taxAmount / 2;
                decimal _CGST = taxAmount / 2;
                decimal _UGST = taxAmount / 2;

                decimal totalAmount = (taxableAmount - item.DiscountAmount) + (taxAmount);


                SaleItems newItems = new SaleItems();


                if (sales.GST_Type == GST_Type.SGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitId = unitId, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, SGST = _SGST, UnitCost = unitCost };
                }
                if (sales.GST_Type == GST_Type.UGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, UnitId = unitId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, UGST = _UGST, UnitCost = unitCost };
                }
                if (sales.GST_Type == GST_Type.IGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, UnitId = unitId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, IGST = taxAmount, UnitCost = unitCost };
                }

                inMemorySale.Add(newItems);
                context.Products.Update(filteredProduct);
            };

            await context.SaleItems.AddRangeAsync(inMemorySale);
            var totalUnitCost = inMemorySale.Sum(x => x.UnitCost);
            var newSale = new Sales { Id = allIdsDto.SaleId, InvoiceId = allIdsDto.InvoiceId, DebtorsId = creditCustomer.Id, PaymentType = sales.PaymentType, CreatedBy = allIdsDto.UserId, OrganisationId = allIdsDto.OrganisationId, Narration = sales.Text, GST_Type = sales.GST_Type, TotalUnitCost = totalUnitCost, SalesType = sales.SalesMode.ToString() };


            await context.Sales.AddAsync(newSale);


            if (sales.SalesMode == SalesMode.B2C)
            {
                await AddnewB2CSaleInvoices(inMemorySale, allIdsDto, dueDate, 0);
            }
            if (sales.SalesMode == SalesMode.B2B)
            {
                await AddnewB2BSaleInvoices(inMemorySale, allIdsDto, dueDate, 0);
            }


            //var respond = accountClient.  (new updateStockRequest { OrganisationId = allIdsDto.OrganisationId.ToString(), Increase = false, ProductId = product.ProductId.ToString(), Quantity = (int)product.Quantity, UserId = allIdsDto.UserId.ToString() });
            //if (respond.Success == false)
            //{
            //    transaction.Rollback();
            //    return new ResponseDto<object> { StatusCode = 400, Message = respond.Message };
            //}




            return new ResponseDto<object> { StatusCode = 201, Message = "new credit sale is created" };

        }
        public async Task<ResponseDto<object>> AddNewCashSale(SalesAddDto sales, CreateSaleIdsDto allIdsDto)
        {
            //fetch hsncode and taxrate from productId



            var cashCustomer = await GetCashCustomers(sales.MobileNum, allIdsDto.OrganisationId);

            if (cashCustomer == null)
            {
                return new ResponseDto<object> { StatusCode = 404, Message = "customer not found,please add new customer" };
            }
            //var response = await _grpcClient.GetProductsByOrganizationAsync(new OrganizationRequest { OrganizationId = allIdsDto.OrganisationId.ToString() });
            DateTime dueDate = DateTime.Now;
            List<SaleItems> inMemorySale = new List<SaleItems>();
            foreach (var item in sales.SaleItems)
            {
                var filteredProduct = await GetProductById(item.ProductId, allIdsDto.OrganisationId);

                if (filteredProduct == null)
                {
                    return new ResponseDto<object> { StatusCode = 404, Message = "product not found," };
                }




                filteredProduct.Stock -= item.Quantity;

                decimal taxRate = filteredProduct.HsnCode.GstRate;
                string hsnCode = filteredProduct.HsnCode.HSNCodeNumber;
                decimal unitCost = filteredProduct.CostPrice;
                int unitId = filteredProduct.UnitOfMeasuresId;

                decimal taxableAmount = item.Quantity * item.UnitPrice;

                decimal taxAmount = (taxableAmount - item.DiscountAmount) * (taxRate / 100);

                decimal _SGST = taxAmount / 2;
                decimal _CGST = taxAmount / 2;
                decimal _UGST = taxAmount / 2;

                decimal totalAmount = (taxableAmount - item.DiscountAmount) + (taxAmount);
                SaleItems newItems = new SaleItems();
                if (sales.GST_Type == GST_Type.SGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, SGST = _SGST, UnitId = unitId, UnitCost = unitCost };
                }
                if (sales.GST_Type == GST_Type.UGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, CGST = _CGST, UGST = _UGST, UnitId = unitId, UnitCost = unitCost };
                }
                if (sales.GST_Type == GST_Type.IGST)
                {
                    newItems = new SaleItems { ProductId = item.ProductId, DiscountAmount = item.DiscountAmount, Quantity = item.Quantity, UnitPrice = item.UnitPrice, SaleId = allIdsDto.SaleId, TaxRate = taxRate, HSNCodeNumber = hsnCode, TotalAmount = totalAmount, TaxableAmount = taxableAmount, IGST = taxAmount, UnitId = unitId, UnitCost = unitCost };
                }



                inMemorySale.Add(newItems);
                context.Products.Update(filteredProduct);

            };

            await context.SaleItems.AddRangeAsync(inMemorySale);
            var totalUnitCost = inMemorySale.Sum(x => x.UnitCost);
            var newSale = new Sales { Id = allIdsDto.SaleId, InvoiceId = allIdsDto.InvoiceId, CashCustomerId = cashCustomer.Id, PaymentType = sales.PaymentType, CreatedBy = allIdsDto.UserId, OrganisationId = allIdsDto.OrganisationId, GST_Type = sales.GST_Type, TotalUnitCost = totalUnitCost };

            await context.Sales.AddAsync(newSale);

            await AddnewB2CSaleInvoices(inMemorySale, allIdsDto, dueDate, inMemorySale.Sum(x => x.TotalAmount));

            //foreach (var product in inMemorySale)
            //{ 
            //var respond = stockclient.updateProductStock(new updateStockRequest { OrganisationId = allIdsDto.OrganisationId.ToString(), Increase = false, ProductId = product.ProductId.ToString(), Quantity = (int)product.Quantity, UserId = allIdsDto.UserId.ToString() });
            //if (respond.Success == false)
            //{
            //    transaction.Rollback();
            //    return new ResponseDto<object> { StatusCode = 400, Message = respond.Message };
            //}
            //}





            return new ResponseDto<object> { StatusCode = 201, Message = "new cash sale is created" };
        
        }
        public async Task<List<Sales>> GetAllSalesDetails(Guid orgId, Guid userId, bool isFullData, int? skip, int? take)
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
          
        

        public async Task<Sales> GetSalesDetailsById(Guid saleId, Guid orgId)
        {

        
                var sales = await context.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Products).Include(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Id == saleId && x.OrganisationId == orgId);

                return sales;
        
        }
        public async Task<Sales> GetB2CSalesDetailsByInvoice(string invoiceNum, Guid orgId)
        {

        
                var sales = await context.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Products).Include(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Invoices.B2CInvoiceNumber == invoiceNum && x.OrganisationId == orgId);

                return sales;

            
        
        }
        public async Task<Sales> GetB2BSalesDetailsByInvoice(string invoiceNum, Guid orgId)
        {

            
                var sales = await context.Sales.Include(s => s.SaleItems).ThenInclude(si => si.Products).Include(s => s.SaleItems).ThenInclude(si => si.UnitOfMeasures).Include(s => s.Invoices).Include(s => s.CashCustomers).Include(s => s.CreditCustomers).FirstOrDefaultAsync(x => x.Invoices.B2BInvoiceNumber == invoiceNum && x.OrganisationId == orgId);

                return sales;

            
    
        }
        public async Task<List<Sales>> GetSaleDetailsByDate(DateTime fromDate, DateTime toDate, Guid orgId, Guid userId, bool fullData, int? skip, int? take)
        {
            
                var query = context.Sales
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.Products)
                    .Include(s => s.SaleItems)
                        .ThenInclude(si => si.UnitOfMeasures)
                    .Include(s => s.Invoices)
                    .Include(s => s.CashCustomers)
                    .Include(s => s.CreditCustomers)
                    .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate && s.OrganisationId == orgId)
                    .AsQueryable();

                // Filter by userId if fullData is false
                if (!fullData)
                {
                    query = query.Where(s => s.CreatedBy == userId);
                }

                // Order by latest created
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




        public async Task CashReceived(Guid debtorsId, decimal receivedAmount, decimal currentBalance, Guid orgId)
        {
            DateTime currentDate = DateTime.UtcNow;

            var sales = await GetDebtorsSales(debtorsId, orgId);

            // Get only pending invoices
            var pendingInvoices = sales
                .Where(s => s.Invoices.DueDate <= currentDate && s.Invoices.RecievedAmount <= s.Invoices.TotalAmount && s.OrganisationId == orgId)
                .OrderBy(s => s.CreatedAt)
                .Select(s => s.Invoices)
                .ToList();

            foreach (var invoice in pendingInvoices)
            {
                decimal remainingDue = invoice.TotalAmount - invoice.RecievedAmount;

                // Step 1: Apply from balance
                if (currentBalance >= remainingDue)
                {
                    invoice.RecievedAmount += remainingDue;
                    currentBalance -= remainingDue;

                    continue;
                }
                else
                {
                    invoice.RecievedAmount += currentBalance;
                    remainingDue -= currentBalance;
                    currentBalance = 0;
                }


                if (receivedAmount >= remainingDue)
                {
                    invoice.RecievedAmount += remainingDue;
                    receivedAmount -= remainingDue;

                }
                else
                {
                    invoice.RecievedAmount += receivedAmount;
                    receivedAmount = 0;

                    break;
                }
            }

        }

       

        // Save to DB (make sure your repo context is tracking these)


        public async Task<string> GenerateB2CInvoiceNumber(Guid orgId)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
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
                .Where(si => si.B2BInvoiceNumber.StartsWith(prefix + "B2B") && si.OrganisationId == orgId)
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

