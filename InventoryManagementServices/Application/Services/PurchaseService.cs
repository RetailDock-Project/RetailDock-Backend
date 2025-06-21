using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;

namespace Application.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository purchaseRepo;
        private readonly ILogger<PurchaseService> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IInvoiceNumberGenerator invoiceNumberGenerator;
        private readonly IAccountGrpcService accountGrpcService;


        public PurchaseService(IPurchaseRepository _purchaseRepo, ILogger<PurchaseService> _logger, IUnitOfWork _unitOfWork,IMapper _mapper, IInvoiceNumberGenerator _invoiceNumberGenerator, IAccountGrpcService _accountGrpcService) {
            purchaseRepo = _purchaseRepo;
            logger = _logger;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
             invoiceNumberGenerator= _invoiceNumberGenerator;
            accountGrpcService=_accountGrpcService;

        }
        public async Task<Responses<object>> AddPurchase(PurchaseAddDto newPurchase, Guid orgId, Guid userId)
        {

            try
            {

                if (orgId == Guid.Empty || userId == Guid.Empty)
                {
                    return new Responses<object> { StatusCode = 400, Message = "Error in adding purchase" };
                }

                string json = JsonSerializer.Serialize(newPurchase, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);

                var purchaseItems = new List<PurchaseItem>();
                var purchaseId = Guid.NewGuid();
                var document = new Document();
                decimal taxAmount = 0m;


                //if (newPurchase.SupplierInvoice != null)
                //{

                //    var documentBase64 = ImageHelper.ConvertToBase64(newPurchase.SupplierInvoice);
                //    document.Id=Guid.NewGuid();
                //    document.DocumentData = documentBase64;
                //    document.FileNote = "Supplier Invoice";
                //    document.FileName = newPurchase.SupplierInvoice.FileName;
                //    document.ContentType = newPurchase.SupplierInvoice.ContentType;
                //}

                foreach (var product in newPurchase.purchaseItems)
                {


                    var item = await purchaseRepo.GetProductById(product.ProductId);
                    if (item == null)
                    {
                        return new Responses<object> { StatusCode = 404, Message = $"product with id-{product.ProductId} not found" };

                    }
                    else if (newPurchase.PurchaseOrderId != null) {

                        var purchaseOrderProductDetail = await purchaseRepo.GetProductPurchaseOrder(product.ProductId, newPurchase.PurchaseOrderId);
                        if (purchaseOrderProductDetail.Quantity < product.Quantity) { 
                            return new Responses<object> { StatusCode = 400, Message = $"product with id-{product.ProductId} quantity is greater than purchase order quantity" };
                        }
                    }
                    

                    var gstRate = item.HsnCode.GstRate;

                    //var gstRate = 11m;
                    Console.WriteLine("this is gst rate");
                    Console.WriteLine(gstRate);
                    var itemAmount = product.Quantity * product.RatePerPiece;

                    var itemDiscount = (product.Discount / 100) * itemAmount;
                    var itemTotal = itemAmount - itemDiscount;
                    var productTax = 0m;
                    decimal cgst = 0m;
                    decimal sgst = 0m;
                    decimal igst = 0m;
                    decimal ugst = 0m;


                    if (newPurchase.GstType == GstTypes.UGST_CGST)
                    {


                        cgst = itemTotal * (gstRate / 200);
                        ugst = itemTotal * (gstRate / 200);
                        productTax = cgst + ugst;
                    }

                    else if (newPurchase.GstType == GstTypes.CGST_SGST)
                    {
                        cgst = itemTotal * (gstRate / 200);
                        sgst = itemTotal * (gstRate / 200);
                        productTax = cgst + sgst;
                    }

                    else
                    {
                        igst = itemTotal * (gstRate / 100);
                        productTax = igst;
                    }


                    taxAmount += productTax;

                    purchaseItems.Add(new PurchaseItem
                    {
                        PurchaseId = purchaseId,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        RatePerPiece = product.RatePerPiece,
                        Discount = product.Discount,
                        TaxAmount = productTax,
                        CGST = cgst,
                        SGST = sgst,
                        IGST = igst,
                        UGST = ugst,
                        TotalAmount = itemTotal + productTax
                    });
                }
                var subTotal = newPurchase.purchaseItems.Sum(x => x.RatePerPiece * x.Quantity);



                var lastPurchaseInvoiceNumber = await purchaseRepo.GetLastPurchaseInvoiceNumber(orgId) ;
                var invoicenumber= await invoiceNumberGenerator.GenerateInvoiceNumber(lastPurchaseInvoiceNumber,"P");


                var purchaseInvoice = new PurchaseInvoice { 
                    Id=Guid.NewGuid(),
                    InvoiceNumber = invoicenumber,
                    SubTotal = subTotal,
                    TaxAmount = taxAmount,
                    TotalAmount = subTotal + taxAmount,
                    GstType = newPurchase.GstType,
                    IGST = purchaseItems.Sum(x => x.IGST),
                    CGST = purchaseItems.Sum(x => x.CGST),
                    SGST = purchaseItems.Sum(x => x.SGST),
                    UGST = purchaseItems.Sum(x => x.UGST)
                };

                var purchase = new Purchase
                {
                    Id = purchaseId,
                    PurchaseOrderId= newPurchase.PurchaseOrderId,
                    PurchaseInvoiceId = purchaseInvoice.Id,
                    SupplierId=newPurchase.SupplierId,
                    Purchasedate=newPurchase.Purchasedate,
                    SupplierInvoiceNumber=newPurchase.SupplierInvoiceNumber,
                    DueDate=newPurchase.DueDate,
                    CreatedBy=userId,
                    OrganizationId=orgId,
                    //DocumentId=document.Id,

                };

                var voucher = mapper.Map<Voucher>(newPurchase.Voucher);
                voucher.VoucherTypeId = "fbgdh";
                if (newPurchase.Voucher.TransactionsDebit != null)
                {
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsDebit[0].LedgerId,
                        Amount = (double)subTotal,
                        Narration = newPurchase.Voucher.TransactionsDebit[0].Narration,
                    });
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsDebit[1].LedgerId,
                        Amount = (double)taxAmount,
                        Narration = newPurchase.Voucher.TransactionsDebit[1].Narration,
                    });
                    
                }

                if (newPurchase.Voucher.TransactionsCredit != null)
                {
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsCredit[0].LedgerId,
                        Amount = (double)subTotal + (double)taxAmount,
                        Narration = newPurchase.Voucher.TransactionsCredit[0].Narration,
                    });
                }




                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    await purchaseRepo.AddPurchase(purchase);
                    //if (newPurchase.SupplierInvoice != null) {
                    //    await purchaseRepo.AddDocument(document);

                    //}



                    await purchaseRepo.AddPurchaseItems(purchaseItems);
                    foreach (var purchaseItem in purchaseItems)
                    {
                        await purchaseRepo.UpdateStocksAndUnitPrice(purchaseItem,userId);
                    }

                    await purchaseRepo.AddPurchaseInvoice(purchaseInvoice);

                    var response=await accountGrpcService.UpdatePurchaseUccounts(voucher);
                    if (response.StatusCode != 200) {
                        await transaction.RollbackAsync();
                        return new Responses<object> { StatusCode = 400, Message = $"Error in adding purchase" };

                    }

                    await unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }

                return new Responses<object> { StatusCode = 200, Message = "Purchase Created Successfully" };
            }


            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error in adding purchase");
                return new Responses<object> { StatusCode = 500, Message = "Error in adding purchase" };
            }




        }
        public async Task<Responses<List<GetPurchaseDto>>> GetAllPurchases(Guid organaizationId)
        {
            try
            {
                var purchases = await purchaseRepo.GetAllPurchase(organaizationId);
                if (purchases.Count == 0)
                {
                    return new Responses<List<GetPurchaseDto>>
                    {
                        StatusCode = 400,
                        Message = "No purchases on this organaization"
                    };
                }
                var mappedPurchase = mapper.Map<List<GetPurchaseDto>>(purchases);
                return new Responses<List<GetPurchaseDto>>
                {
                    StatusCode = 200,
                    Message = "Purchases Fetched succesfully",
                    Data = mappedPurchase


                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error in fetching Pruchases");
                return new Responses<List<GetPurchaseDto>>
                {
                    StatusCode = 500,
                    Message = "Error in fetching Purchases"
                };
            }

        }
        public async Task<Responses<GetPurchaseDetailsDto>> GetPurchaseDetails(Guid purchaseId)
        {
            try
            {
                var purchases = await purchaseRepo.GetPurchaseById(purchaseId);
                if (purchases == null)
                {
                    return new Responses<GetPurchaseDetailsDto>
                    {
                        Message = "There is no Purchases in this PurchaseId",
                        StatusCode = 400
                    };
                }
                var mappedpurchase = mapper.Map<GetPurchaseDetailsDto>(purchases);
                return new Responses<GetPurchaseDetailsDto>
                {
                    StatusCode = 200,
                    Message = "Purchase Details Fetched",
                    Data = mappedpurchase
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error in fetching PurchaseDetails");
                return new Responses<GetPurchaseDetailsDto>
                {
                    StatusCode = 500,
                    Message = "Eror in fetching purchaseDetails "
                };


            }
        }


        public async Task<Responses<object>> AddPurchaseReturn(PurchaseReturnDto newPurchaseReturn, Guid userId, Guid orgId)
        {

            try
            {


                if (userId == Guid.Empty)
                {
                    return new Responses<object>
                    {
                        StatusCode = 500,
                        Message = "Invalid user"
                    };

                }



                var originalPurchase = await purchaseRepo.GetPurchaseById(newPurchaseReturn.OriginalPurchaseId);
                if (originalPurchase == null)
                {
                    return new Responses<object>
                    {
                        StatusCode = 404,
                        Message = "Original purchase not found"
                    };
                }

                var originalInvoice = await purchaseRepo.GetPurchaseInvoiceById(originalPurchase.PurchaseInvoiceId);
                if (originalInvoice == null)
                {
                    return new Responses<object>
                    {
                        StatusCode = 404,
                        Message = "Original purchase invoice not found"
                    };
                }

                var lastPurchaseReturnInvoiceNumber = await purchaseRepo.GetLastPurchaseReturnInvoiceNumber(orgId);
                var returnInvoiceNumber = await invoiceNumberGenerator.GenerateInvoiceNumber(lastPurchaseReturnInvoiceNumber, "PR");


                var returnInvoice = new PurchaseReturnInvoice
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = returnInvoiceNumber,
                    OriginalPurchaseInvoiceId = originalInvoice.Id,
                    GstType = originalInvoice.GstType,
                    ReturnDate = newPurchaseReturn.ReturnDate.ToDateTime(TimeOnly.MinValue),
                    CreatedAt = DateTime.UtcNow,

                };

                var purchaseReturn = new PurchaseReturn
                {
                    Id = Guid.NewGuid(),
                    OriginalPurchaseId = newPurchaseReturn.OriginalPurchaseId,
                    OrganizationId = orgId,
                    ReturnDate = newPurchaseReturn.ReturnDate,
                    PurchaseReturnInvoiceId = returnInvoice.Id,
                    SupplierId = newPurchaseReturn.SupplierId,
                    Reason = newPurchaseReturn.Reason,
                    Notes = newPurchaseReturn.Notes,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<PurchaseReturnItem>()
                };

                foreach (var itemDto in newPurchaseReturn.Items)
                {
                    var product = await purchaseRepo.GetProductById(itemDto.ProductId);
                    if (product == null)
                    {
                        return new Responses<object>
                        {
                            StatusCode = 404,
                            Message = $"Product not found: {itemDto.ProductId}"
                        };
                    }

                    var originalItem = await purchaseRepo.GetPurchaseItemById(itemDto.OriginalPurchaseItemId);
                    if (originalItem == null || originalItem.PurchaseId != newPurchaseReturn.OriginalPurchaseId)
                    {
                        return new Responses<object>
                        {
                            StatusCode = 400,
                            Message = $"Invalid purchase item: {itemDto.OriginalPurchaseItemId}"
                        };
                    }

                    var totalPreviouslyReturnedQty = await purchaseRepo.GetTotalReturnedQuantity(itemDto.OriginalPurchaseItemId);
                    if (itemDto.ReturnedQuantity + totalPreviouslyReturnedQty > originalItem.Quantity)
                        if (itemDto.ReturnedQuantity > originalItem.Quantity)
                        {

                            return new Responses<object>
                            {
                                StatusCode = 400,
                                Message = $"Return quantity ({itemDto.ReturnedQuantity}) exceeds original quantity ({originalItem.Quantity}) for product: {product.ProductName}"
                            };

                        }

                    var productTax = 0m;
                    decimal cgst = 0m;
                    decimal sgst = 0m;
                    decimal igst = 0m;
                    decimal ugst = 0m;

                    var returnItemAmount = originalItem.RatePerPiece * itemDto.ReturnedQuantity;
                    var itemDiscount = (originalItem.Discount / 100) * returnItemAmount;
                    var taxableAmount = returnItemAmount - itemDiscount;

                    var gstRate = product.HsnCode.GstRate;

                    if (originalInvoice.GstType == GstTypes.UGST_CGST)
                    {

                        cgst = taxableAmount * (gstRate / 200);
                        ugst = taxableAmount * (gstRate / 200);
                        productTax = cgst + ugst;
                    }

                    else if (originalInvoice.GstType == GstTypes.CGST_SGST)
                    {
                        cgst = taxableAmount * (gstRate / 200);
                        sgst = taxableAmount * (gstRate / 200);
                        productTax = cgst + sgst;
                    }

                    else
                    {
                        igst = taxableAmount * (gstRate / 100);
                        productTax = igst;
                    }

                    var returnItem = new PurchaseReturnItem
                    {
                        Id = Guid.NewGuid(),
                        PurchaseReturnId = purchaseReturn.Id,
                        OriginalPurchaseItemId = itemDto.OriginalPurchaseItemId,
                        ProductId = itemDto.ProductId,
                        ReturnedQuantity = itemDto.ReturnedQuantity,
                        TotalAmount = productTax + taxableAmount,
                        TaxAmount = productTax,
                        SGST = sgst,
                        CGST = cgst,
                        UGST = ugst,
                        IGST = igst,
                        Discount = itemDiscount
                    };



                    purchaseReturn.Items.Add(returnItem);

                    returnInvoice.SubTotal += returnItemAmount;
                    returnInvoice.TaxAmount += productTax;
                    returnInvoice.TotalAmount = returnInvoice.SubTotal + returnInvoice.TaxAmount;

                }
                returnInvoice.CGST = purchaseReturn.Items.Sum(x => x.CGST);
                returnInvoice.IGST = purchaseReturn.Items.Sum(x => x.IGST);
                returnInvoice.UGST = purchaseReturn.Items.Sum(x => x.UGST);
                returnInvoice.SGST = purchaseReturn.Items.Sum(x => x.SGST);



                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    foreach (var item in purchaseReturn.Items)
                    {
                        await purchaseRepo.UpdateProductStock(item);

                    }

                    await purchaseRepo.CreatePurchaseReturnInvoice(returnInvoice);
                    await purchaseRepo.CreatePurchaseReturn(purchaseReturn);

                    await unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }




                //var response = _mapper.Map<PurchaseReturnResponse>(purchaseReturn);
                //return ServiceResult<PurchaseReturnResponse>.Success(response);
                return new Responses<object>
                {
                    StatusCode = 200,
                    Message = "Purchase return added successfully"
                };


            }
            catch (Exception ex)
            {
                return new Responses<object>
                {
                    StatusCode = 500,
                    Message = "Error in creating purchase return"
                };
            }

        }
        public async Task<Responses<List<GetPurchaseReturnDto>>> GetAllPurchaseReturn(Guid organaizationId)
        {
            try
            {
                var purchaseReturn = await purchaseRepo.GetAllPurchaseReturn(organaizationId);
                if (purchaseReturn == null)
                {
                    return new Responses<List<GetPurchaseReturnDto>>
                    {
                        StatusCode = 400,
                        Message = "There is no purchaseReturn in this orgnaization"
                    };
                }
                var mapped = mapper.Map<List<GetPurchaseReturnDto>>(purchaseReturn);
                return new Responses<List<GetPurchaseReturnDto>>
                {
                    StatusCode = 200,
                    Message = "Purchase Return Fetched Succefully",
                    Data = mapped
                };


            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error in Fetching PurchaseReturn Details");
                return new Responses<List<GetPurchaseReturnDto>>
                {
                    Message = ex.Message,
                    StatusCode = 500,
                };

            }
        }

        public async Task<Responses<GetPurchaseReturnDetailsDto>> GetPurchaseReturn(Guid PurchaseReturnId)
        {
            try
            {
                var PurchaseReturns = await purchaseRepo.getPurchaseReturn(PurchaseReturnId);
                if (PurchaseReturns == null)
                {
                    return new Responses<GetPurchaseReturnDetailsDto>
                    {
                        StatusCode = 400,
                        Message = "The PurchaseReturn Id is not Found"
                    };
                }
                var mapped = mapper.Map<GetPurchaseReturnDetailsDto>(PurchaseReturns);
                return new Responses<GetPurchaseReturnDetailsDto>
                {
                    StatusCode = 200,
                    Message = "Purchase Return Fetched Succesfully",
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error in fetching PurchaseReturns");
                return new Responses<GetPurchaseReturnDetailsDto>
                {
                    Message = ex.Message,
                    StatusCode = 500,
                };
            }
        }



        public async Task<Responses<object>> ExportPurchases(Guid organizationId)
        {
            try
            {
                var purchases = await purchaseRepo.GetAllPurchase(organizationId); // Get data
                var result = mapper.Map<List<GetPurchaseDto>>(purchases); // Map if needed

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Purchases");

                // Set headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Purchase Date";
                worksheet.Cells[1, 3].Value = "Total Amount";
                worksheet.Cells[1, 4].Value = "Supplier Invoice Number";
                worksheet.Cells[1, 5].Value = "Purchase Invoice Number";

                int row = 2;

                foreach (var purchase in result)
                {
                    worksheet.Cells[row, 1].Value = purchase.Id.ToString();
                    worksheet.Cells[row, 2].Value = purchase.Purchasedate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = purchase.TotalAmount;
                    worksheet.Cells[row, 4].Value = purchase.SupplierInvoiceNumber ?? "";
                    worksheet.Cells[row, 5].Value = purchase.PurchaseInvoiceNumber;

                    row++;
                }

                worksheet.Cells.AutoFitColumns(); // Auto size columns

                var excelBytes = await package.GetAsByteArrayAsync();

                return new Responses<object>
                {
                    StatusCode = 200,
                    Message = "Purchases exported successfully",
                    Data = new
                    {
                        file = excelBytes,
                        fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName = "PurchaseList.xlsx"
                    }
                };
            }
            catch (Exception ex)
            {
                return new Responses<object>
                {
                    StatusCode = 500,
                    Message = "Error while exporting purchases"
                };
            }
        }

    }
}


