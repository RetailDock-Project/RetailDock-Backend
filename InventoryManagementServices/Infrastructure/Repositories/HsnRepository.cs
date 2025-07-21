using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class HsnRepository:IHsnCodeRepository
    {
        private readonly AppDbContext _appDbContext;
        public HsnRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
       public async Task<HsnCode> AddHsn(HsnCode hsnCode)
        {
            await _appDbContext.AddAsync(hsnCode);
            await _appDbContext.SaveChangesAsync();
            return hsnCode;

        }
        public async Task<HsnCode> GetByHsnCodeAndOrg(Guid organizationId,string hsnCodeNumber )
        {
            var result= await _appDbContext.HsnCodes
                .FirstOrDefaultAsync(x => !x.IsDeleted
                                          && x.OrgnaisationId == organizationId
                                          && x.HSNCodeNumber == hsnCodeNumber);
            return result;
        }


        public async Task<List<HsnCode>> GetAllHsnCodes(Guid OrganaiztionId)
        {
            return await _appDbContext.HsnCodes.Where(x => !x.IsDeleted &&x.OrgnaisationId==OrganaiztionId).ToListAsync();

        }
       public async Task<HsnCode> GetByHsnCode(int hsnCodeId)
        {
             return await _appDbContext.HsnCodes.FirstOrDefaultAsync(x=>!x.IsDeleted&&  x.HsnCodeId== hsnCodeId);
        }
       public async Task<HsnCode> UpdateHsn(HsnCode hsnCode)
        {
            _appDbContext.Update(hsnCode);
            await _appDbContext.SaveChangesAsync();
            return hsnCode;
        }

       public async Task<bool> DeleteHsnCode(int hsnCodeId)
        {
            var existinghsnCode = await _appDbContext.HsnCodes
                .FirstOrDefaultAsync(x => x.HsnCodeId == hsnCodeId);

            if (existinghsnCode == null)
                return false;

            existinghsnCode.IsDeleted = true;
            _appDbContext.HsnCodes.Update(existinghsnCode);
            await _appDbContext.SaveChangesAsync();

            return true;
        }

        public async Task<InvoiceSummeryDTO> GetInvoiceSummary(Guid orgId, DateTime? fromDate, DateTime? toDate, InvoiceType invoiceType)
        {
            // If fromDate is null or MinValue, default to 1st day of current month
            if (!fromDate.HasValue || fromDate.Value == DateTime.MinValue)
                fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // If toDate is null or MinValue, default to today
            if (!toDate.HasValue || toDate.Value == DateTime.MinValue)
                toDate = DateTime.Today;

            // Ensure fromDate and toDate are now valid DateTime values
            var from = fromDate.Value.Date;
            var to = toDate.Value.Date;

            if (invoiceType == InvoiceType.Purchase)
            {
                var purchases = await _appDbContext.Purchases
                    .Include(p => p.PurchaseInvoice)
                    .Include(p => p.PurchaseItems)
                        .ThenInclude(pi => pi.Product)
                            .ThenInclude(prod => prod.HsnCode)
                    .Include(p => p.Supplier)
                    .Where(p => p.OrganizationId == orgId &&
                                p.Purchasedate >= from &&
                                p.Purchasedate <= to)
                    .ToListAsync();

                var invoiceDetails = purchases.Select(p => new InVoiceDetails
                {
                    InvoiceId = p.PurchaseInvoice.Id,
                    invoiceNumber = p.PurchaseInvoice.InvoiceNumber,
                    supplierorcustomerName = p.Supplier.Name,
                    salemodeorpurchasemode = "Purchase",
                    BeforeGSTValue = p.PurchaseInvoice.SubTotal,
                    GstTotal = p.PurchaseInvoice.TaxAmount,
                    IGST = p.PurchaseInvoice.IGST ?? 0,
                    CGST = p.PurchaseInvoice.CGST ?? 0,
                    UGST = p.PurchaseInvoice.UGST ?? 0,
                    TotalValue = p.PurchaseInvoice.TotalAmount
                }).ToList();

                var hsnGrouped = purchases
                    .SelectMany(p => p.PurchaseItems)
                    .GroupBy(pi => pi.Product.HsnCode)
                    .Select(g => new HsnDetails
                    {
                        HsnId = g.Key.HsnCodeId,
                        HsnNumber = g.Key.HSNCodeNumber,
                        BeforeGSTValue = g.Sum(x => x.TotalAmount - x.TaxAmount),
                        GstTotal = g.Sum(x => x.TaxAmount),
                        IGST = g.Sum(x => x.IGST ?? 0),
                        CGST = g.Sum(x => x.CGST ?? 0),
                        UGST = g.Sum(x => x.UGST ?? 0),
                        SGST = g.Sum(x => x.SGST ?? 0),
                        TotalValue = g.Sum(x => x.TotalAmount)
                    }).ToList();

                return new InvoiceSummeryDTO
                {
                    InvoiceCount = invoiceDetails.Count,
                    BeforeGSTValue = invoiceDetails.Sum(i => i.BeforeGSTValue),
                    GstTotal = invoiceDetails.Sum(i => i.GstTotal),
                    IGST = invoiceDetails.Sum(i => i.IGST),
                    CGST = invoiceDetails.Sum(i => i.CGST),
                    UGST = invoiceDetails.Sum(i => i.UGST),
                    TotalValue = invoiceDetails.Sum(i => i.TotalValue),
                    inVoiceDetails = invoiceDetails,
                    hsnDetails = hsnGrouped
                };
            }
            else if (invoiceType == InvoiceType.PurchaseReturn)
            {
                var purchaseReturns = await _appDbContext.PurchaseReturns
                    .Include(pr => pr.PurchaseReturnInvoice)
                    .Include(pr => pr.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(prod => prod.HsnCode)
                    .Include(pr => pr.Supplier)
                    .Where(pr => pr.OrganizationId == orgId &&
                                 pr.ReturnDate.ToDateTime(TimeOnly.MinValue) >= from &&
                                 pr.ReturnDate.ToDateTime(TimeOnly.MinValue) <= to)
                    .ToListAsync();

                var invoiceDetails = purchaseReturns.Select(pr => new InVoiceDetails
                {
                    InvoiceId = pr.PurchaseReturnInvoice.Id,
                    invoiceNumber = pr.PurchaseReturnInvoice.InvoiceNumber,
                    supplierorcustomerName = pr.Supplier.Name,
                    salemodeorpurchasemode = "PurchaseReturn",
                    BeforeGSTValue = pr.PurchaseReturnInvoice.SubTotal,
                    GstTotal = pr.PurchaseReturnInvoice.TaxAmount,
                    IGST = pr.PurchaseReturnInvoice.IGST ?? 0,
                    CGST = pr.PurchaseReturnInvoice.CGST ?? 0,
                    UGST = pr.PurchaseReturnInvoice.UGST ?? 0,
                    SGST = pr.PurchaseReturnInvoice.SGST ?? 0,
                    TotalValue = pr.PurchaseReturnInvoice.TotalAmount
                }).ToList();

                var hsnGrouped = purchaseReturns
                    .SelectMany(pr => pr.Items)
                    .GroupBy(i => i.Product.HsnCode)
                    .Select(g => new HsnDetails
                    {
                        HsnId = g.Key.HsnCodeId,
                        HsnNumber = g.Key.HSNCodeNumber,
                        BeforeGSTValue = g.Sum(x => x.TotalAmount - x.TaxAmount),
                        GstTotal = g.Sum(x => x.TaxAmount),
                        IGST = g.Sum(x => x.IGST ?? 0),
                        CGST = g.Sum(x => x.CGST ?? 0),
                        UGST = g.Sum(x => x.UGST ?? 0),
                        SGST = g.Sum(x => x.SGST ?? 0),
                        TotalValue = g.Sum(x => x.TotalAmount)
                    }).ToList();

                var data = JsonSerializer.Serialize(hsnGrouped);

                Console.WriteLine(data);

                return new InvoiceSummeryDTO
                {
                    InvoiceCount = invoiceDetails.Count,
                    BeforeGSTValue = invoiceDetails.Sum(i => i.BeforeGSTValue),
                    GstTotal = invoiceDetails.Sum(i => i.GstTotal),
                    IGST = invoiceDetails.Sum(i => i.IGST),
                    CGST = invoiceDetails.Sum(i => i.CGST),
                    UGST = invoiceDetails.Sum(i => i.UGST),
                    SGST= invoiceDetails.Sum(x => x.SGST),
                    TotalValue = invoiceDetails.Sum(i => i.TotalValue),
                    inVoiceDetails = invoiceDetails,
                    hsnDetails = hsnGrouped
                };
            }

            return new InvoiceSummeryDTO(); // default if nothing matches
        }




    }
}
