using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Dto;
using Application.Helpers;
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
        private readonly IPurchaseOrderRepository purchaseOrderRepo;
        private readonly IOrganizationService organizationService;



        public PurchaseService(IPurchaseRepository _purchaseRepo, ILogger<PurchaseService> _logger, IUnitOfWork _unitOfWork,IMapper _mapper, IInvoiceNumberGenerator _invoiceNumberGenerator, IAccountGrpcService _accountGrpcService, IPurchaseOrderRepository _purchaseOrderRepo, IOrganizationService _organizationService) {
            purchaseRepo = _purchaseRepo;
            logger = _logger;
            unitOfWork = _unitOfWork;
            mapper = _mapper;
             invoiceNumberGenerator= _invoiceNumberGenerator;
            accountGrpcService=_accountGrpcService;
            purchaseOrderRepo = _purchaseOrderRepo;
            organizationService = _organizationService;


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
                decimal taxableAmount = 0m;
                decimal taxAmount = 0m;
                 decimal purchaseQty= 0m;
                decimal purchaseOrderQty = 0m;


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
                        if (purchaseOrderProductDetail == null)
                        {
                            return new Responses<object>
                            {
                                StatusCode = 404,
                                Message = $"Purchase order item for product id-{product.ProductId} not found"
                            };
                        }

                        if ((purchaseOrderProductDetail.Quantity - purchaseOrderProductDetail.ReceivedQuantity) < product.Quantity)
                        {
                            return new Responses<object> { StatusCode = 400, Message = $"product with id-{product.ProductId} quantity is greater than remaining purchase order quantity" };
                        }

                        //purchaseOrderQty += purchaseOrderProductDetail.Quantity - purchaseOrderProductDetail.ReceivedQuantity;
                        purchaseOrderProductDetail.ReceivedQuantity += product.Quantity;
                        //purchaseQty += product.Quantity;
                    }


                    var gstRate = item.HsnCode.GstRate;

                    //var gstRate = 11m;
                    Console.WriteLine("this is gst rate");
                    Console.WriteLine(gstRate);
                    decimal itemAmount = product.Quantity * product.RatePerPiece;
                    decimal itemDiscount = (product.Discount??0 / 100) * itemAmount;
                    decimal itemTotal = itemAmount - itemDiscount;
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
                    taxableAmount += itemTotal;
                    purchaseItems.Add(new PurchaseItem
                    {
                        PurchaseId = purchaseId,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        RatePerPiece = product.RatePerPiece,
                        Discount = product.Discount ?? 0,
                        TaxAmount = productTax,
                        CGST = cgst,
                        SGST = sgst,
                        IGST = igst,
                        UGST = ugst,
                        TotalAmount = itemTotal + productTax
                    });
                }
                //var subTotal = newPurchase.purchaseItems.Sum(x => x.RatePerPiece * x.Quantity);



                var lastPurchaseInvoiceNumber = await purchaseRepo.GetLastPurchaseInvoiceNumber(orgId) ;
                var invoicenumber= await invoiceNumberGenerator.GenerateInvoiceNumber(lastPurchaseInvoiceNumber,"P");


                var purchaseInvoice = new PurchaseInvoice { 
                    Id=Guid.NewGuid(),
                    InvoiceNumber = invoicenumber,
                    SubTotal = taxableAmount,
                    TaxAmount = taxAmount,
                    TotalAmount = taxableAmount + taxAmount,
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
                voucher.VoucherTypeId = "a5bf213f-421a-11f0-a0c7-862ccfb05833";
                voucher.OrganizationId = orgId.ToString();
                voucher.CreatedBy = userId.ToString();

                voucher.TransactionsDebit = new List<Transaction>();
                voucher.TransactionsCredit = new List<Transaction>();

                // ✅ Now safely add
                if (newPurchase.Voucher.TransactionsDebit?.Count >= 2)
                {
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsDebit[0].LedgerId,
                        Amount = (double)taxableAmount,
                        Narration = newPurchase.Voucher.TransactionsDebit[0].Narration
                    });

                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsDebit[1].LedgerId,
                        Amount = (double)taxAmount,
                        Narration = newPurchase.Voucher.TransactionsDebit[1].Narration
                    });
                }

                if (newPurchase.Voucher.TransactionsCredit?.Count >= 1)
                {
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        LedgerId = newPurchase.Voucher.TransactionsCredit[0].LedgerId,
                        Amount = (double)taxableAmount + (double)taxAmount,
                        Narration = newPurchase.Voucher.TransactionsCredit[0].Narration
                    });
                }

                var jsonData = JsonSerializer.Serialize(voucher, new JsonSerializerOptions
                {
                    WriteIndented = true // makes it look pretty
                });
                Console.WriteLine(jsonData);

                



                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    await purchaseRepo.AddPurchase(purchase);
                    //if (newPurchase.SupplierInvoice != null) {
                    //    await purchaseRepo.AddDocument(document);

                    //}

                    if (newPurchase.PurchaseOrderId != null)
                    {
                        var purchaseOrder = await purchaseOrderRepo.GetPurchaseOrderByIdAsync(newPurchase.PurchaseOrderId);

                        decimal totalQty = purchaseOrder.PurchaseOrderItems.Sum(poi => poi.Quantity);
                        decimal receivedQty = purchaseOrder.PurchaseOrderItems.Sum(poi => poi.ReceivedQuantity);

                        if (totalQty == receivedQty)
                            purchaseOrder.OrderStatus = "Complete";
                        else if (receivedQty > 0)
                            purchaseOrder.OrderStatus = "Partial";
                        else
                            purchaseOrder.OrderStatus = "Pending";
                    }

                    await purchaseRepo.AddPurchaseItems(purchaseItems);
                    foreach (var purchaseItem in purchaseItems)
                    {
                        await purchaseRepo.UpdateStocksAndUnitPrice(purchaseItem,userId);
                    }

                    await purchaseRepo.AddPurchaseInvoice(purchaseInvoice);

                    var response=await accountGrpcService.UpdatePurchaseUccounts(voucher);
                    var responsedata=JsonSerializer.Serialize(response);
                    Console.WriteLine(responsedata);
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
        public async Task<Responses<List<GetPurchaseDto>>> GetAllPurchases(Guid organaizationId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var purchases = await purchaseRepo.GetAllPurchase(organaizationId, fromDate, toDate);
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
            var data = JsonSerializer.Serialize(newPurchaseReturn);
            Console.WriteLine("\n");
            Console.WriteLine("\n");

            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");


            Console.WriteLine(data);
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

                    returnInvoice.SubTotal += taxableAmount;
                    returnInvoice.TaxAmount += productTax;
                    returnInvoice.TotalAmount = returnInvoice.SubTotal + returnInvoice.TaxAmount;

                }
                returnInvoice.CGST = purchaseReturn.Items.Sum(x => x.CGST);
                returnInvoice.IGST = purchaseReturn.Items.Sum(x => x.IGST);
                returnInvoice.UGST = purchaseReturn.Items.Sum(x => x.UGST);
                returnInvoice.SGST = purchaseReturn.Items.Sum(x => x.SGST);


                var voucher = mapper.Map<Voucher>(newPurchaseReturn.Voucher);
                voucher.VoucherTypeId = "d2c2b36e-421a-11f0-a0c7-862ccfb05833";
                voucher.OrganizationId = orgId.ToString();
                voucher.CreatedBy = userId.ToString();

                voucher.TransactionsDebit = new List<Transaction>();
                voucher.TransactionsCredit = new List<Transaction>();

                // ✅ Now safely add
                if (newPurchaseReturn.Voucher.TransactionsDebit?.Count >= 1)
                {
                    voucher.TransactionsCredit.Add(new Transaction
                    {
                        LedgerId = newPurchaseReturn.Voucher.TransactionsDebit[0].LedgerId,
                        Amount = (double)returnInvoice.SubTotal + (double)returnInvoice.TaxAmount,
                        Narration = newPurchaseReturn.Voucher.TransactionsDebit[0].Narration
                    });
                    Console.WriteLine(newPurchaseReturn.Voucher.TransactionsDebit[0].LedgerId);
                    Console.WriteLine((double)returnInvoice.SubTotal + (double)returnInvoice.TaxAmount);
                    Console.WriteLine(newPurchaseReturn.Voucher.TransactionsDebit[0].Narration);


                }
                Console.WriteLine("\n");
                Console.WriteLine("\n");

                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                var trCredit = JsonSerializer.Serialize(voucher.TransactionsDebit);
                Console.WriteLine(trCredit);
 

                if (newPurchaseReturn.Voucher.TransactionsCredit?.Count >= 2)
                {
                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchaseReturn.Voucher.TransactionsCredit[0].LedgerId,
                        Amount = (double)returnInvoice.SubTotal,
                        Narration = newPurchaseReturn.Voucher.TransactionsCredit[0].Narration
                    });

                    voucher.TransactionsDebit.Add(new Transaction
                    {
                        LedgerId = newPurchaseReturn.Voucher.TransactionsCredit[1].LedgerId,
                        Amount = (double)returnInvoice.TaxAmount,
                        Narration = newPurchaseReturn.Voucher.TransactionsCredit[1].Narration
                    });
                }
                Console.WriteLine("\n");
                Console.WriteLine("\n");

                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                Console.WriteLine("\n");
                var trDebit = JsonSerializer.Serialize(voucher.TransactionsCredit);
                Console.WriteLine(trCredit);


                using var transaction = await unitOfWork.BeginTransactionAsync();
                try
                {
                    foreach (var item in purchaseReturn.Items)
                    {
                        await purchaseRepo.UpdateProductStock(item);

                    }

                    await purchaseRepo.CreatePurchaseReturnInvoice(returnInvoice);
                    await purchaseRepo.CreatePurchaseReturn(purchaseReturn);
                    var response = await accountGrpcService.UpdatePurchaseUccounts(voucher);
                    if (response.StatusCode != 200)
                    {
                        await transaction.RollbackAsync();
                        return new Responses<object> { StatusCode = 400, Message = $"Error in adding purchase return" };

                    }
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



        public async Task<byte[]?> ExportPurchases(Guid organizationId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var purchases = await purchaseRepo.GetAllPurchase(organizationId, fromDate, toDate);
                var result = mapper.Map<List<GetPurchaseDto>>(purchases);

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Purchases");

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

                worksheet.Cells.AutoFitColumns();
                return await package.GetAsByteArrayAsync();
            }
            catch
            {
                return null; // Handle error in controller
            }
        }


        public async Task<Responses<List<GetPurchaseDto>>> GetPurchasesAsync(
    Guid organizationId,
    string? searchTerm,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize)
        {
            try
            {
                var purchases = await purchaseRepo.GetPurchases(
                    organizationId, searchTerm, fromDate, toDate, pageNumber, pageSize);

                if (purchases.Count == 0)
                {
                    return new Responses<List<GetPurchaseDto>>
                    {
                        StatusCode = 400,
                        Message = "No purchases found"
                    };
                }

                var mappedPurchase = mapper.Map<List<GetPurchaseDto>>(purchases);
                return new Responses<List<GetPurchaseDto>>
                {
                    StatusCode = 200,
                    Message = "Purchases fetched successfully",
                    Data = mappedPurchase
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in fetching purchases");
                return new Responses<List<GetPurchaseDto>>
                {
                    StatusCode = 500,
                    Message = "Error in fetching purchases"
                };
            }
        }


        public async Task<Responses<List<GetPurchaseReturnDto>>> GetPurchaseReturnsFilterAsync(
    Guid organizationId,
    string? search,
    DateTime? fromDate,
    DateTime? toDate,
    int? pageNumber,
    int? pageSize)
        {
            try
            {
                var purchaseReturns = await purchaseRepo.GetPurchaseReturnsFilterAsync(organizationId, search, fromDate, toDate, pageNumber, pageSize);

                if (purchaseReturns == null || !purchaseReturns.Any())
                {
                    return new Responses<List<GetPurchaseReturnDto>>
                    {
                        StatusCode = 404,
                        Message = "No purchase returns found."
                    };
                }

                var mapped = mapper.Map<List<GetPurchaseReturnDto>>(purchaseReturns);

                return new Responses<List<GetPurchaseReturnDto>>
                {
                    StatusCode = 200,
                    Message = "Purchase returns fetched successfully.",
                    Data = mapped
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in fetching purchase return data.");
                return new Responses<List<GetPurchaseReturnDto>>
                {
                    StatusCode = 500,
                    Message = "Internal server error: " + ex.Message
                };
            }
        }

        public async Task<Responses<List<RecentInventoryTransactionDto>>> GetLastWeekTransactions(Guid organizationId)
        {
            try
            {
                var fromDate = DateTime.UtcNow.AddDays(-7);
                var toDate = DateTime.UtcNow;

                var purchases = await purchaseRepo.GetRecentPurchases(organizationId, fromDate, toDate);
                var returns = await purchaseRepo.GetRecentReturns(organizationId, fromDate, toDate);

                var results = new List<RecentInventoryTransactionDto>();

                results.AddRange(purchases.Select(p => new RecentInventoryTransactionDto
                {
                    Id= p.Id,
                    Type = "Purchase",
                    SupplierOrSource = p.Supplier?.Name ?? "Unknown",
                    ItemCount = p.PurchaseItems?.Count ?? 0,
                    Date = p.CreatedAt,
                    Amount = p.PurchaseInvoice.TotalAmount,
                }));

                results.AddRange(returns.Select(r => new RecentInventoryTransactionDto
                {
                    Id= r.Id,
                    Type = "Return",
                    SupplierOrSource = r.Supplier?.Name ?? "Unknown",
                    ItemCount = r.Items?.Count ?? 0,
                    Date = r.CreatedAt,
                    Amount = r.PurchaseReturnInvoice.TotalAmount
                }));

                var sorted = results.OrderByDescending(x => x.Date).ToList();

                return new Responses<List<RecentInventoryTransactionDto>>
                {
                    StatusCode = 200,
                    Message = "Last 7 days of inventory transactions fetched successfully",
                    Data = sorted
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error fetching recent inventory transactions");

                return new Responses<List<RecentInventoryTransactionDto>>
                {
                    StatusCode = 500,
                    Message = "An error occurred while fetching recent inventory transactions"
                };
            }
        }


        public async Task<DownloadPdfResult> DownloadPurchasePdfAsync(Guid purchaseId, Guid orgId)
        {
            var purchaseResult = await GetPurchaseDetails(purchaseId);
            if (purchaseResult == null)
                return DownloadPdfResult.Failure("Purchase not found");

            var organization = await organizationService.GetOrganizationByIdAsync(orgId);
            if (organization == null)
                return DownloadPdfResult.Failure("Organization details not found");

            var pdfBytes = PurchasePdfGenerator.GeneratePurchasePdf(purchaseResult.Data, organization);
            var fileName = $"Purchase_{purchaseId.ToString().Substring(0, 6)}.pdf";

            return DownloadPdfResult.Success(pdfBytes, fileName);
        }

    }
}


