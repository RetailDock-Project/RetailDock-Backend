using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dto;
using Application.DTOs;
using Application.Interfaces.Grpc_Interface;
using Application.Interfaces.Repository_Interfaces;
using Application.Interfaces.Service_Interfaces;
using AutoMapper;
using Common.ResponseDto;
using Domain.Entites;
using Microsoft.Extensions.Logging;


namespace Application.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository saleRepo;
        private readonly ILogger<SaleService> logger;
        private readonly IMapper mapper;
        private readonly IAccountGrpc accountGrpc;
        private readonly IUnitOfWorkRepository unitOfWork;
 
        public SaleService(ISaleRepository _saleRepo, ILogger<SaleService> _logger, IMapper _mapper, IAccountGrpc _accountGrpc,IUnitOfWorkRepository _unitOfWork)
        {
            logger = _logger;
            saleRepo = _saleRepo;
            mapper = _mapper;
            accountGrpc = _accountGrpc;
            unitOfWork = _unitOfWork;

        } 
        public async Task<ResponseDto<object>> CashReceivedFromDebtor(Guid debtorsId, decimal receivedAmount, decimal currentBalance, Guid orgId)
        {
            try
            {
         
                await saleRepo.CashReceived(debtorsId, receivedAmount, currentBalance, orgId);
                await saleRepo.SaveChanges();
                return new ResponseDto<object> { Message = "updated sale Invoice amount", StatusCode = 200 };
            }catch (Exception ex)
            {
                logger.LogError(ex, "error updating sale Invoice recievedAmount");
                return new ResponseDto<object> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        //public async Task<ResponseDto<object>> AddNewSale(SalesAddDto sales, Guid orgId, Guid userId)
        //{
        //    try
        //    {
        //        await unitOfWork._BiginTransaction();

        //        var voucher = new Voucher { CreatedBy = userId.ToString(), OrganizationId = orgId.ToString(), Remarks = sales.SaleVoucher.Remarks, VoucherTypeId = "a5bea1e0-421a-11f0-a0c7-862ccfb05833", VoucherDate = DateTime.Now.ToString() ,TransactionsDebit = new List<Transaction>(),
        //            TransactionsCredit = new List<Transaction>()
        //        };
        //        decimal taxAmount = 0;
        //        decimal taxableAmount = 0;
        //        decimal costOfGoodsSold= 0;
        //        foreach (var product in sales.SaleItems)
        //        {
        //            var filteredProduct = await saleRepo.GetProductById(product.ProductId, orgId);

        //            if(filteredProduct == null)
        //            {

        //                return new ResponseDto<object> { StatusCode = 400, Message = "no product found" };
        //            }

        //            if (product.UnitPrice > filteredProduct.MRP  )
        //            {

        //                return new ResponseDto<object> { StatusCode = 304, Message = "moreThan marketPrice" };
        //            }

        //            decimal taxRate = filteredProduct.HsnCode.GstRate;
        //            taxableAmount += product.Quantity * product.UnitPrice;
        //            taxAmount += ((product.Quantity * product.UnitPrice) - product.DiscountAmount) * (taxRate / 100);
        //            costOfGoodsSold += product.Quantity * filteredProduct.CostPrice;


        //        }

        //        //var respond=await accountGrpc.updateSaleAccounts()
        //        var allIds = new CreateSaleIdsDto
        //        {
        //            InvoiceId = Guid.NewGuid(),
        //            OrganisationId = orgId,
        //            SaleId = Guid.NewGuid(),
        //            UserId = userId,
        //            voucherNumer = responseFromAccounts.Data

        //        };

        //        ResponseDto<object> result;

        //        if (sales.PaymentType == PaymentMode.Credit || sales.creditCustomer)
        //        {
        //            result = await saleRepo.AddNewCreditSale(sales, allIds);


        //            // check for any error response from repository
        //            if (result.StatusCode != 201)
        //            {
        //                return result;
        //            }

        //            var creditCustomer = await saleRepo.GetCreditCustomers(sales.MobileNum, orgId);
        //            if (creditCustomer == null)
        //            {
        //                return new ResponseDto<object> { StatusCode = 404, Message = "no customer found" };
        //            }
        //            if (sales.SaleVoucher.TransactionsDebit != null)
        //            {
        //                voucher.TransactionsDebit.Add(new Transaction { Amount = (double)taxableAmount + (double)taxAmount, LedgerId = creditCustomer.LedgerId.ToString(), Narration = $"CashSaleDoneTo{creditCustomer.CustomerName}" });

        //                voucher.TransactionsDebit.Add(new Transaction { Amount = (double)costOfGoodsSold, LedgerId = sales.SaleVoucher.TransactionsDebit[0].LedgerId, Narration = sales.SaleVoucher.TransactionsDebit[0].Narration ?? $"Sale - costOfGoodsSold" });
        //            }


        //        }
        //        else
        //        {
        //            result = await saleRepo.AddNewCashSale(sales, allIds);

        //            logger.LogInformation("new cash sale added:{@Result}", result);

        //            if (result.StatusCode != 201)
        //            {
        //                return result;
        //            }
        //            var cashCustomer = await saleRepo.GetCashCustomers(sales.MobileNum, orgId);
        //            if (cashCustomer == null)
        //            {

        //                return new ResponseDto<object> { StatusCode = 404, Message = "no customer found" };
        //            }
        //            if (sales.SaleVoucher.TransactionsDebit != null )
        //            {
        //                voucher.TransactionsDebit.Add( new Transaction { Amount = (double)taxableAmount + (double)taxAmount, LedgerId = cashCustomer.LedgerId.ToString(), Narration = $"CashSaleDoneTo{cashCustomer.CustomerName}" });

        //                voucher.TransactionsDebit.Add( new Transaction { Amount = (double)costOfGoodsSold, LedgerId = sales.SaleVoucher.TransactionsDebit[0].LedgerId, Narration = sales.SaleVoucher.TransactionsDebit[0].Narration ?? $"Sale - costOfGoodsSold" });

        //            }



        //        }

        //        if (sales.SaleVoucher.TransactionsCredit != null && sales.SaleVoucher.TransactionsCredit.Count >= 3)
        //        {
        //            // Credit for taxable amount (e.g., goods value)
        //            voucher.TransactionsCredit.Add(new Transaction
        //            {
        //                LedgerId = sales.SaleVoucher.TransactionsCredit[0].LedgerId,
        //                Amount = (double)taxableAmount,
        //                Narration = sales.SaleVoucher.TransactionsCredit[0].Narration ?? $"Sale - saleAmount"
        //            });

        //            // Credit for tax amount
        //            voucher.TransactionsCredit.Add(new Transaction
        //            {
        //                LedgerId = sales.SaleVoucher.TransactionsCredit[1].LedgerId,
        //                Amount = (double)taxAmount,
        //                Narration = sales.SaleVoucher.TransactionsCredit[1].Narration?? "Sale - OuputTax"
        //            });
        //            voucher.TransactionsCredit.Add(new Transaction
        //            {
        //                LedgerId = sales.SaleVoucher.TransactionsCredit[2].LedgerId,
        //                Amount = (double)costOfGoodsSold,
        //                Narration =  sales.SaleVoucher.TransactionsCredit[2].Narration ?? "Sale - SaleInventory A/c "
        //            });

        //        }

        //        var responseFromAccounts =await accountGrpc.updateSaleAccounts(voucher);
        //        logger.LogInformation("logging from new sale voucher:{@Response}", responseFromAccounts);
        //        if (responseFromAccounts.StatusCode != 200)
        //        {

        //            await unitOfWork._RolBackTransaction();
        //            return result;
        //        }
        //        if (responseFromAccounts.StatusCode == 200)
        //        {
        // await unitOfWork._CommitTransaction();
        //            await saleRepo.SaveChanges();

        //            return result;
        //        }       

        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        await unitOfWork._RolBackTransaction();
        //        logger.LogError(ex, "Error while adding new sale");
        //        return new ResponseDto<object> { StatusCode = 500, Message = "Internal Server Error" };
        //    }
        //}

        public async Task<ResponseDto<object>> AddNewSale(SalesAddDto sales, Guid orgId, Guid userId)
        {
            try
            {
                await unitOfWork._BiginTransaction();
                string invoiceNumber = "";
                decimal taxAmount = 0;
                decimal taxableAmount = 0;
                decimal costOfGoodsSold = 0;

                var voucher = new Voucher
                {
                    CreatedBy = userId.ToString(),
                    OrganizationId = orgId.ToString(),
                    Remarks = $"{sales.InvoiceNumber}",
                    VoucherTypeId = "a5bea1e0-421a-11f0-a0c7-862ccfb05833",
                    VoucherDate = DateTime.Now.ToString(),
                    TransactionsDebit = new List<Transaction>(),
                    TransactionsCredit = new List<Transaction>()
                };

                foreach (var product in sales.SaleItems)
                {
                    var filteredProduct = await saleRepo.GetProductById(product.ProductId, orgId);
                    if (filteredProduct == null)
                    {

                        await unitOfWork._RolBackTransaction();
                        return new ResponseDto<object> { StatusCode = 400, Message = "No product found" };
                    }
                        

                    if (product.UnitPrice > filteredProduct.MRP)
                    {

                        await unitOfWork._RolBackTransaction();
                        return new ResponseDto<object> { StatusCode = 304, Message = "Price exceeds MRP" };
                    }
                        

                    decimal taxRate = filteredProduct.HsnCode.GstRate;
                    taxableAmount += product.Quantity * product.UnitPrice;
                    taxAmount += ((product.Quantity * product.UnitPrice) - product.DiscountAmount) * (taxRate / 100);
                    costOfGoodsSold += product.Quantity * filteredProduct.CostPrice;
                }

                // Debit Side
                Guid ledgerId;
                string customerName;

                if (sales.PaymentType == PaymentMode.Credit || (sales.PaymentType == PaymentMode.Cash && sales.creditCustomer))
                {
                    var creditCustomer = await saleRepo.GetCreditCustomers(sales.MobileNum, orgId);
                    if (creditCustomer == null || creditCustomer.LedgerId == Guid.Empty)
                        return new ResponseDto<object> { StatusCode = 404, Message = "Credit customer not found" };
                    customerName= creditCustomer.CustomerName;
                    ledgerId = creditCustomer.LedgerId;

                }
                else
                {
                    var cashCustomer = await saleRepo.GetCashCustomers(sales.MobileNum, orgId);
                    if (cashCustomer == null || cashCustomer.LedgerId == Guid.Empty)
                        return new ResponseDto<object> { StatusCode = 404, Message = "Cash customer not found" };
                    customerName=cashCustomer.CustomerName;
                    ledgerId = cashCustomer.LedgerId;

                }

                // Now you have a single variable: ledgerId




                if (ledgerId == null )
                    return new ResponseDto<object> { StatusCode = 404, Message = "Customer not found" };

           
                if (sales.SaleVoucher.TransactionsDebit?.Count > 0)
                {

                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        Amount = (double)(taxableAmount + taxAmount),
                        LedgerId = ledgerId.ToString(),
                        Narration = $"InvoiceNumber:{sales.InvoiceNumber}"
                    });
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        Amount = (double)costOfGoodsSold,
                        LedgerId = sales.SaleVoucher.TransactionsDebit[0].LedgerId,
                        Narration = $"InvoiceNumber:{sales.InvoiceNumber}"
                    });
              

                }

                // Credit Side
                if (sales.SaleVoucher.TransactionsCredit?.Count >= 3)
                {
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        Amount = (double)taxableAmount,
                        LedgerId = sales.SaleVoucher.TransactionsCredit[0].LedgerId,
                        Narration = $"InvoiceNumber:{sales.InvoiceNumber}"
                    });
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        Amount = (double)taxAmount,
                        LedgerId = sales.SaleVoucher.TransactionsCredit[1].LedgerId,
                        Narration = $"InvoiceNumber:{sales.InvoiceNumber}"
                    });
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        Amount = (double)costOfGoodsSold,
                        LedgerId = sales.SaleVoucher.TransactionsCredit[2].LedgerId,
                        Narration = $"InvoiceNumber:{sales.InvoiceNumber}"
                    });
                }

                // 🔁 gRPC call should happen ONLY ONCE here
              

                var allIds = new CreateSaleIdsDto
                {
                    InvoiceId = Guid.NewGuid(),
                    OrganisationId = orgId,
                    SaleId = Guid.NewGuid(),
                    UserId = userId,
           
                };

                // Save Sale
                ResponseDto<object> result;
                result = (sales.PaymentType == PaymentMode.Credit || sales.creditCustomer)
                    ? await saleRepo.AddNewCreditSale(sales, allIds)
                    : await saleRepo.AddNewCashSale(sales, allIds);


      
                var responseFromAccounts = await accountGrpc.updateSaleAccounts(voucher);
                if (responseFromAccounts.StatusCode != 200 || responseFromAccounts.Data == null)
                {
                    await unitOfWork._RolBackTransaction();
                    return new ResponseDto<object> { StatusCode = 500, Message = "Accounting service failed" };
                }
                await saleRepo.SaveChanges();
                await unitOfWork._CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                await unitOfWork._RolBackTransaction();
                logger.LogError(ex, "Error while adding new sale");
                return new ResponseDto<object> { StatusCode = 500, Message = "Internal Server Error" };
            }
        }

        public async Task<ResponseDto<List<SalesResponseDto>>>  GetDebtorsSalesDetails(Guid debtorId, Guid orgId)
        {
            try
            {
                var sale=await saleRepo.GetDebtorsSales(debtorId, orgId);
                if (sale == null)
                {
                    return new ResponseDto<List<SalesResponseDto>>
                    {

                        Message = "no sales is found between that date",
                        StatusCode = 404
                    };
                }
                var mapped = mapper.Map<List<SalesResponseDto>>(sale);
                return new ResponseDto<List<SalesResponseDto>>
                {
                    Data = mapped,
                    Message = "Sales fetched successfully",
                    StatusCode = 200
                };
            }
            catch( Exception ex)
            {
                logger.LogError(ex, "error from fetching sales details By DebtorId");
                return new ResponseDto<List<SalesResponseDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }


        public async Task<ResponseDto<SaleWithTaxReportDto>> getSalesDetailsWithTaxReport(DateTime? fromDate, DateTime? toDate, Guid orgId, Guid userId,  int? skip, int? take)
        {
            try
            {


                DateTime thisMonth = DateTime.Now;
                DateTime startFromDate = fromDate ?? new DateTime(thisMonth.Year, thisMonth.Month, 1);

                DateTime finalToDate = toDate ?? DateTime.Now;
                bool fullData =  true;
                var sale = await saleRepo.GetSaleDetailsByDate(startFromDate, finalToDate, orgId, userId, fullData, skip, take);
                if (sale == null)
                {
                    return new ResponseDto<SaleWithTaxReportDto>
                    {

                        Message = "no sales is found between that date",
                        StatusCode = 404
                    };
                }
                var mappedSales = mapper.Map<List<SalesResponseDto>>(sale);

                var taxReport = sale.SelectMany(s => s.SaleItems).GroupBy(si => si.HSNCodeNumber).Select(hsn => new HsnTaxReportDto { HSNCode=hsn.Key,TotalTaxableAmount=hsn.Sum(si=>si.TaxableAmount),CGST=hsn.Sum(si=>si.CGST), SGST = hsn.Sum(si => si.SGST), IGST = hsn.Sum(si => si.IGST), UGST = hsn.Sum(si => si.UGST) }).ToList();

                return new ResponseDto<SaleWithTaxReportDto>
                {
                    Data = new SaleWithTaxReportDto { SalesDetails = mappedSales, HsnTaxReport = taxReport },
                    Message = "no sales is found between that date",
                    StatusCode = 404
                };



            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all sales details WithTaxreport ");
                return new ResponseDto<SaleWithTaxReportDto>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }
        public async Task<ResponseDto<List<SalesResponseDto>>> GetSalesByDate(DateTime? fromDate, DateTime? toDate, Guid orgId,Guid userId,bool? isFulldata,int? skip,int? take)
        {
            try
            {
                DateTime thisMonth = DateTime.Now;
                DateTime startFromDate = fromDate ?? new DateTime(thisMonth.Year, thisMonth.Month, 1);

                DateTime finalToDate = toDate ?? DateTime.Now;
                bool fullData = isFulldata ?? false;

                var sale = await saleRepo.GetSaleDetailsByDate(startFromDate, finalToDate, orgId,userId,fullData,skip,take);
                if (sale == null)
                {
                    return new ResponseDto<List<SalesResponseDto>>
                    {

                        Message = "no sales is found between that date",
                        StatusCode = 404
                    };
                }
                var mapped = mapper.Map<List<SalesResponseDto>>(sale);
                return new ResponseDto<List<SalesResponseDto>>
                {
                    Data = mapped,
                    Message = "Sales fetched successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all sales details by date");
                return new ResponseDto<List<SalesResponseDto>>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }


        public async Task<ResponseDto<List<SalesResponseDto>>> GetAllSalesDetails(Guid orgId, Guid userId, bool? isFullData, int? skip, int? take)
        {
            try
            {

                bool fullData=isFullData ?? false;  
                var totalSales = await saleRepo.GetAllSalesDetails(orgId,userId,fullData,skip,take);
                if (!totalSales.Any())
                {
                    return new ResponseDto<List<SalesResponseDto>>
                    {
                        Message = "Your sales is empty",
                        StatusCode = 200
                    };
                }

                var mapped = mapper.Map<List<SalesResponseDto>>(totalSales);

                return new ResponseDto<List<SalesResponseDto>> { Data = mapped, Message = "Fetch All Sales details successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching All sales details ");
                return new ResponseDto<List<SalesResponseDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        public async Task<ResponseDto<SalesResponseDto>> GetSalesDetailsById(Guid saleId, Guid orgId)
        {

            try
            {
                var sales = await saleRepo.GetSalesDetailsById(saleId, orgId);
                if (sales == null)
                {
                    return new ResponseDto<SalesResponseDto> { Message = "no such Idsales found", StatusCode = 200 };
                }
                var mapped = mapper.Map<SalesResponseDto>(sales);
                return new ResponseDto<SalesResponseDto> { Data = mapped, Message = "Fetch Sales details by Id successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching  sales details By Id");

                return new ResponseDto<SalesResponseDto> { Message = "internal Server Error ", StatusCode = 500 };
            }

        }
        public async Task<ResponseDto<SalesResponseDto>> GetSalesDetailsByInvoice(string invoiceNumber, Guid orgId)
        {
            try
            {
                if (invoiceNumber.Contains("B2B"))
                {
                    var sales = await saleRepo.GetB2BSalesDetailsByInvoice(invoiceNumber, orgId);
                    if (sales == null)
                    {
                        return new ResponseDto<SalesResponseDto> { Message = "no such invoiceId Sales found", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesResponseDto>(sales);
                    return new ResponseDto<SalesResponseDto> { Data = mapped, Message = "Fetch B2B Sales details By Invoice successfully", StatusCode = 200 };
                }
                if (invoiceNumber.Contains("B2C"))
                {
                    var sales = await saleRepo.GetB2CSalesDetailsByInvoice(invoiceNumber, orgId);
                    if (sales == null)
                    {
                        return new ResponseDto<SalesResponseDto> { Message = "no such invoiceId Sales found", StatusCode = 200 };
                    }
                    var mapped = mapper.Map<SalesResponseDto>(sales);
                    return new ResponseDto<SalesResponseDto> { Data = mapped, Message = "Fetch B2C Sales details By Invoice successfully", StatusCode = 200 };
                }

                return new ResponseDto<SalesResponseDto> { Message = "no such invoice found", StatusCode = 400 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching sales details By Invoice");
                return new ResponseDto<SalesResponseDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }

      public async  Task<ResponseDto<string>> GenerateB2BInvoiceNumber(Guid orgId)
        {
            try
            {
                string invoiceNum=await saleRepo.GenerateB2BInvoiceNumber(orgId);
                return new ResponseDto<string> { Data=invoiceNum,Message = "next B2B sale Invoice fetched successFully ", StatusCode = 200 };


            }
            catch(Exception ex)
            {
                logger.LogError(ex, "error from fetching B2B sales  Invoice");
                return new ResponseDto<string> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }   public async  Task<ResponseDto<string>> GenerateB2CInvoiceNumber(Guid orgId)
        {
            try
            {
                string invoiceNum=await saleRepo.GenerateB2CInvoiceNumber(orgId);
                return new ResponseDto<string> { Data=invoiceNum,Message = "next B2C sale Invoice fetched successFully ", StatusCode = 200 };


            }
            catch(Exception ex)
            {
                logger.LogError(ex, "error from fetching B2C sales  Invoice");
                return new ResponseDto<string> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
    }
}
