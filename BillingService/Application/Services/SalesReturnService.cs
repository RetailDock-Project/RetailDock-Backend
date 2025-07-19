using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class SalesReturnService : ISaleReturnService
    {
        private readonly IMapper mapper;
        private readonly ISaleReturnRepository saleReturnRepo;
        private readonly ILogger<SalesReturnViewDto> logger;
        private readonly IAccountGrpc accountGrpc;
        private readonly IUnitOfWorkRepository unitOfWork;
        public SalesReturnService(IMapper _mapper, ISaleReturnRepository _saleReturnRepo, ILogger<SalesReturnViewDto> _logger,IAccountGrpc _accountGrpc, IUnitOfWorkRepository _unitOfWork)
        {
            mapper = _mapper;
            saleReturnRepo = _saleReturnRepo;
            logger = _logger;
            accountGrpc = _accountGrpc;
           unitOfWork = _unitOfWork;
        }
        public async Task<ResponseDto<object>> AddSalesReturn(AddSalesReturnDto salesReturn, Guid orgId, Guid userId)
        {
            try
            {
                await unitOfWork._BiginTransaction();
                var sale = await saleReturnRepo.fetchSalesByInvoice(salesReturn.SaleInvoiceNumber, orgId);

                if (sale == null)
                {
                  
                    return new ResponseDto<object> { Message = "NoSale found", StatusCode = 404 };


                }
                if(salesReturn.returnDate < sale.CreatedAt || salesReturn.returnDate >  DateTime.Now)
                {
                    return new ResponseDto<object> { Message = "change sales Return Date", StatusCode = 404 };
                }

                decimal taxableAmount =0;
                decimal taxAmount = 0;
                decimal costOfGoodsSold = 0;
             Guid debtorId=sale.CreditCustomers?.LedgerId?? sale.CashCustomers.LedgerId;

                string DebtorName= sale.CreditCustomers?.CustomerName ?? sale.CashCustomers.CustomerName;



                foreach (var returnProduct in salesReturn.ReturnItems)

                {

                   
                    var _saleItem = await saleReturnRepo.soldProductItems(sale.Id, returnProduct.ProductId);
                    if (_saleItem == null)
                    {
                        await unitOfWork._RolBackTransaction();
                        return   new ResponseDto<object> { Message = "Product not found in That sale", StatusCode = 404 };
                        
                    }
                    decimal returnItemsCount = await saleReturnRepo.getReturnedProductCount(sale.Id, returnProduct.ProductId, orgId);
                    decimal remainingQuantity = _saleItem.Quantity - returnItemsCount;
                    if (returnProduct.Quantity > remainingQuantity)
                    {
                        await unitOfWork._RolBackTransaction();
                        return new ResponseDto<object> { Message = "these product already Returned", StatusCode = 409 };
                    }

                    if (salesReturn.ReturnCondition == "Good")
                    {
                        await saleReturnRepo.addproductStock(orgId,returnProduct.ProductId,returnProduct.Quantity);
                    }

                     taxableAmount += _saleItem.UnitPrice * returnProduct.Quantity;

                    costOfGoodsSold += _saleItem.UnitCost * returnProduct.Quantity;
                    
                    decimal taxRate=_saleItem.TaxRate;

                    taxAmount += (_saleItem.UnitPrice * returnProduct.Quantity) * (taxRate / 100);
                }
                    var voucher= new Voucher { CreatedBy=userId.ToString(),OrganizationId=orgId.ToString(),Remarks=$"SR_InvoiceNumber{salesReturn.ReturnInvoiceNumber}",VoucherDate=DateTime.Now.ToString(),VoucherTypeId= "d2c28912-421a-11f0-a0c7-862ccfb05833" ,TransactionsCredit=new List<Transaction>(),TransactionsDebit=new List<Transaction>()};



                if (salesReturn.Voucher.TransactionsDebit != null && salesReturn.Voucher.TransactionsDebit.Count >= 3)
                {
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = salesReturn.Voucher.TransactionsDebit[0].LedgerId,
                        Amount = (double)taxableAmount,
                        Narration = $"SR_InvoiceNumber:{salesReturn.ReturnInvoiceNumber}"
                    });

                    // Credit for tax amount
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = salesReturn.Voucher.TransactionsDebit[1].LedgerId,
                        Amount = (double)taxAmount,
                        Narration = $"SR_InvoiceNumber:{salesReturn.ReturnInvoiceNumber}"
                    });   
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = salesReturn.Voucher.TransactionsDebit[2].LedgerId,
                        Amount = (double)costOfGoodsSold,
                        Narration = $"SR_InvoiceNumber:{salesReturn.ReturnInvoiceNumber}"
                    });
                }
              

                if (salesReturn.Voucher.TransactionsCredit!= null)
                {
                    voucher.TransactionsCredit.Add(new Transaction { Amount = (double)taxableAmount + (double)taxAmount, LedgerId = debtorId.ToString(), Narration = $"SR_InvoiceNumber:{salesReturn.ReturnInvoiceNumber}" });


                        voucher.TransactionsCredit.Add(new Transaction { Amount = (double)costOfGoodsSold, LedgerId = salesReturn.Voucher.TransactionsCredit[0].LedgerId, Narration = $"SR_InvoiceNumber:{salesReturn.ReturnInvoiceNumber}" });

                }
            

             
             
                Guid saleId =sale.Id;
                GST_Type gst_Type = sale.GST_Type;
                if (sale.SalesType == "B2B")
                {
                    await saleReturnRepo.addNewB2BSalesReturn(salesReturn, saleId, orgId, userId,gst_Type);

                }
                if (sale.SalesType == "B2C")
                {
                    await saleReturnRepo.addNewB2CSalesReturn(salesReturn, saleId, orgId, userId,gst_Type);

                }

                var addLedger = await accountGrpc.updateSaleAccounts(voucher);
                logger.LogInformation("Response from adding saleReturn:{@Response}", addLedger);

                if (addLedger.StatusCode != 200)
                {
                    await unitOfWork._RolBackTransaction();


                    return new ResponseDto<object> { Message = "Error in AccountingService", StatusCode = 200 };

                }
                if (addLedger.StatusCode == 200)
                {
                    await unitOfWork._CommitTransaction();
                    await saleReturnRepo.SaveChanges();

           return  new ResponseDto<object> { Message = "New sales return is created", StatusCode = 201 };
                   
                }
       
                return new ResponseDto<object> { Message = "error from saleReturn ", StatusCode = 400 };
            }
            catch (Exception ex)
            {
                await unitOfWork._RolBackTransaction();
           
                logger.LogError(ex, "error from addind new salesReturn  ");
                return new ResponseDto<object> { Message ="internal Server Error", StatusCode = 500 };
            }

        }
        public async Task<ResponseDto<decimal>> getReturnedProductCount(Guid saleId,Guid productId,Guid orgId)
        {
            try
            {
                decimal returnedProductCount =await saleReturnRepo.getReturnedProductCount(saleId, productId, orgId);
                return new ResponseDto<decimal> { Data=returnedProductCount,Message= "successfullyFetched   alreadyreturned product count",StatusCode=200 };


            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching  alreadyreturned product count");
                return new ResponseDto<decimal>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }
public async Task<ResponseDto<SalesReturnTaxReportDto>> GetSalesReturnTaxReport(DateTime? fromDate, DateTime? toDate, Guid orgId,Guid userId)
        {
            try
            {
                DateTime thisTime = DateTime.Now;
                DateTime _fromDate = fromDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                bool fullData = true;
                DateTime finalToDate=toDate ?? DateTime.Now;

                var saleReturn = await saleReturnRepo.GetSalesReturnDetailsBydate(_fromDate, finalToDate, fullData, orgId,userId);
                if (saleReturn == null)
                {
                    return new ResponseDto<SalesReturnTaxReportDto>
                    {

                        Message = "no salesReturn is found between that date",
                        StatusCode = 404
                    };
                }

                var mappedSaleReturn = mapper.Map<List<SalesReturnViewDto>>(saleReturn);
                var hsnTaxReport = saleReturn.SelectMany(sr => sr.SalesReturnItems).GroupBy(sri => sri.HSNCodeNumber).Select(hsn => new HsnReturnTaxReportDto { HSNCode = hsn.Key, CGST = hsn.Sum(x => x.CGST), IGST = hsn.Sum(x => x.IGST), SGST = hsn.Sum(x => x.SGST), UGST = hsn.Sum(x => x.UGST), TotalTaxableAmount = hsn.Sum(x => x.TaxableAmount) }).ToList();

                return new ResponseDto<SalesReturnTaxReportDto> { Data=new SalesReturnTaxReportDto { HsnReturnTaxReport=hsnTaxReport,SalesReturnView=mappedSaleReturn},Message="sales Return Tax Report Fetched successFully" ,StatusCode=200 };



            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all salesReturn TaxReport");
                return new ResponseDto<SalesReturnTaxReportDto>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }
        public async Task<ResponseDto<List<SalesReturnViewDto>>> GetSalesReturnByDate(DateTime fromDate, DateTime? toDate,bool?fullData, Guid orgId,Guid userId)
        {
            try
            {
                DateTime finalToDate = toDate ?? DateTime.Now;
                bool _fullData=fullData ?? false;
                var saleReturn = await saleReturnRepo.GetSalesReturnDetailsBydate(fromDate, finalToDate,_fullData, orgId,userId);
                if (saleReturn == null)
                {
                    return new ResponseDto<List<SalesReturnViewDto>>
                    {

                        Message = "no salesReturn is found between that date",
                        StatusCode = 404
                    };
                }
                var mapped = mapper.Map<List<SalesReturnViewDto>>(saleReturn );
                return new ResponseDto<List<SalesReturnViewDto>>
                {
                    Data = mapped,
                    Message = "SalesReturn fetched successfully",
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error from fetching all salesReturn details by date");
                return new ResponseDto<List<SalesReturnViewDto>>
                {
                    Message = "Internal Server Error",
                    StatusCode = 500
                };
            }
        }


        public async Task<ResponseDto<List<SalesReturnViewDto>>> GetAllSalesReturnDetails(Guid orgId,Guid userId,bool? isFullData,int? skip ,int? take)
        {
            try
            {
                bool fullData = isFullData ?? false;

                var totalSalesReturn = await saleReturnRepo.fetchAllSalesReturn(orgId,userId,fullData,skip,take);
                if (!totalSalesReturn.Any())
                {
                    return new ResponseDto<List<SalesReturnViewDto>>
                    {
                        Message = "Your salesReturn is empty.",
                        StatusCode = 200
                    };
                }

                var mapped= mapper.Map<List<SalesReturnViewDto>>(totalSalesReturn);
                return new ResponseDto<List<SalesReturnViewDto>> {Data=mapped, Message = "Fetch All SalesReturn details successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching All salesReturn details ");
                return new ResponseDto<List<SalesReturnViewDto>> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
        public async Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsById(Guid saleId, Guid orgId)
        {

            try
            {
                var salesReturn = await saleReturnRepo.GetSalesReturnDetailsById(saleId, orgId);
                if (salesReturn == null)
                {
                    return new ResponseDto<SalesReturnViewDto> { Message = "no such IdsalesReturn found", StatusCode = 200 };
                }
                var mapped = mapper.Map<SalesReturnViewDto>(salesReturn);
                return new ResponseDto<SalesReturnViewDto> { Data = mapped, Message = "Fetch SalesReturn details by Id successfully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching  salesReturn details By Id");

                return new ResponseDto<SalesReturnViewDto> { Message = "internal Server Error ", StatusCode = 500 };
            }

        }
        public async Task<ResponseDto<SalesReturnViewDto>> GetSalesReturnDetailsByInvoice(string invoiceNumber, Guid orgId)
        {
            try
            {
             
                    var sales = await saleReturnRepo.GetSalesReturnDetailsByInvoice(invoiceNumber, orgId);
               
                    var mapped = mapper.Map<SalesReturnViewDto>(sales);
                    return new ResponseDto<SalesReturnViewDto> { Data = mapped, Message = "Fetch  Sales Return details By Invoice successfully", StatusCode = 200 };
            

            
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching salesReturn details By Invoice");
                return new ResponseDto<SalesReturnViewDto> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }


      public async  Task<ResponseDto<string>> GenerateB2BReturnInvoiceNumber(Guid orgId)
        {
            try
            {

                var invoiceNum=await saleReturnRepo.GenerateB2BReturnInvoiceNumber(orgId);
                return new ResponseDto<string> { Data = invoiceNum, Message = "B2B return invoice fetched successFully", StatusCode = 200 };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching B2B salesReturn Invoice");
                return new ResponseDto<string> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }
       public async Task<ResponseDto<string>> GenerateB2CReturnInvoiceNumber(Guid orgId)
        {
            try
            {

                var invoiceNum =await saleReturnRepo.GenerateB2CReturnInvoiceNumber(orgId);
                return  new ResponseDto<string>{ Data =invoiceNum,Message="B2C return invoice fetched successFully",StatusCode=200};

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error from fetching B2C salesReturn details By Invoice");
                return new ResponseDto<string> { Message = "internal Server Error ", StatusCode = 500 };
            }
        }


    }

}
