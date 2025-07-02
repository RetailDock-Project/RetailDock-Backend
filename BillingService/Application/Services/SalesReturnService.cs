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
        public SalesReturnService(IMapper _mapper, ISaleReturnRepository _saleReturnRepo, ILogger<SalesReturnViewDto> _logger,IAccountGrpc _accountGrpc)
        {
            mapper = _mapper;
            saleReturnRepo = _saleReturnRepo;
            logger= _logger;
            accountGrpc = _accountGrpc;

        }
        public async Task<ResponseDto<object>> AddSalesReturn(AddSalesReturnDto salesReturn, Guid orgId, Guid userId)
        {
            try
            {


                var sale= await saleReturnRepo.fetchSalesByInvoice(salesReturn.SaleInvoiceNumber,orgId);
       

                if (sale == null)
                {
                    return new ResponseDto<object> { Message = "NoSale found", StatusCode = 404 };


                }
                decimal taxableAmount =0;
                decimal taxAmount = 0;
                foreach (var returnProduct in salesReturn.ReturnItems)
                {
                    var _saleItem = await saleReturnRepo.soldProductItems(sale.Id, returnProduct.ProductId);

                     taxableAmount += _saleItem.UnitPrice * returnProduct.Quantity;
                    decimal taxRate=_saleItem.TaxRate;
                    taxAmount += (_saleItem.UnitPrice * returnProduct.Quantity) * (taxRate / 100);
                }
                    var voucher= new Voucher { CreatedBy=userId.ToString(),OrganizationId=orgId.ToString(),Remarks=salesReturn.Voucher.Remarks,VoucherDate=DateTime.Now.ToString(),VoucherTypeId= "d2c28912-421a-11f0-a0c7-862ccfb05833" };

                voucher.TransactionsCredit.AddRange(salesReturn.Voucher.TransactionsCredit.Select(cr=>new Transaction { Amount=((double)taxableAmount+(double)taxAmount),LedgerId=cr.LedgerId,Narration="salesReturn"}));
                if(salesReturn.Voucher.TransactionsDebit !=null && salesReturn.Voucher.TransactionsDebit.Count > 2)
                {
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = salesReturn.Voucher.TransactionsDebit[0].LedgerId,
                        Amount = (double)taxableAmount,
                        Narration = $"SaleReturn - Taxable "
                    });

                    // Credit for tax amount
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = salesReturn.Voucher.TransactionsDebit[1].LedgerId,
                        Amount = (double)taxAmount,
                        Narration = $"SaleReturn - Tax"
                    });
                }
                var addLedger = await accountGrpc.updateSaleAccounts(voucher);
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

                await saleReturnRepo.SaveChanges();
                return new ResponseDto<object> { Message = "New sales return is created", StatusCode = 201 }; 

            }
            catch (Exception ex)
            {
                return new ResponseDto<object> { Message = ex.Message, StatusCode = 500 };
            }

        }
        public async Task<ResponseDto<List<SalesReturnViewDto>>> GetSalesReturnByDate(DateTime fromDate, DateTime? toDate, Guid orgId)
        {
            try
            {
                DateTime finalToDate = toDate ?? DateTime.Now;

                var saleReturn = await saleReturnRepo.GetSalesReturnDetailsBydate(fromDate, finalToDate, orgId);
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


        public async Task<ResponseDto<List<SalesReturnViewDto>>> GetAllSalesReturnDetails(Guid orgId)
        {
            try
            {
                var totalSalesReturn = await saleReturnRepo.fetchAllSalesReturn(orgId);
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





    }

}
