using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Events;
using AutoMapper;
using Domain.Entities;
using OfficeOpenXml;

namespace Application.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {

            CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
            CreateMap<ProductCategory,GetProductCategoryDto>().ReverseMap();
            CreateMap<UnitOfMeasures,UnitOfMeasureDto>().ReverseMap();
            CreateMap<UnitOfMeasures, GetUnitOfMeasureDto>().ReverseMap();

            CreateMap<ProductDto, Product>()
               .ForMember(dest => dest.Images, opt => opt.Ignore())
               .ForMember(dest => dest.BarCodeImageBase64, opt => opt.Ignore());


            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Category.ProductCategoryName))
                
                .ForMember(dest => dest.UnitOfMeasures,
                    opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement))
                .ForMember(dest => dest.TaxRate,
                    opt => opt.MapFrom(src => src.HsnCode.GstRate))
                //.ForMember(dest => dest.BarCodeImageBase64,
                //    opt => opt.MapFrom(src => Convert.ToBase64String(src.BarCodeImageBase64)))
                //.ForMember(dest => dest.ProductImagesBase64,
                //       opt => opt.MapFrom(src =>
                //      src.Images.Select(img => Convert.ToBase64String(img.ImageData)).ToList()))
                //.ForMember(dest=>dest.TaxRate,opt=>opt.MapFrom(src=>src.HsnCode.GstRate))
                ;

            CreateMap<Product, GetProductDetailDto>()
                .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Category.ProductCategoryName))
                .ForMember(dest => dest.UnitOfMeasures,
                    opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement))
                .ForMember(dest => dest.BarCodeImageBase64,
                    opt => opt.MapFrom(src => Convert.ToBase64String(src.BarCodeImageBase64)))
                .ForMember(dest => dest.ProductImagesBase64,
                       opt => opt.MapFrom(src =>
                      src.Images.Select(img => new ImageData { Id= img.Id, Image= Convert.ToBase64String(img.ImageData) }).ToList()))
                .ForMember(dest => dest.TaxRate, opt => opt.MapFrom(src => src.HsnCode.GstRate))
                ;




            CreateMap<Product, ProductCreatedEvent>().ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.Gst, opt => opt.MapFrom(src => src.HsnCode.HsnCodeId)).ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement)).ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ProductCategoryName));


            CreateMap<Product,ProductBillingGetDto>()
               .ForMember(dest=>dest.UnitOfMeasures,opt=>opt.MapFrom(src=>src.UnitOfMeasures.Measurement))
               .ForMember(dest=>dest.TaxRate,opt=>opt.MapFrom(src=>src.HsnCode.GstRate))
               .ForMember(dest=>dest.ProductCategory,opt=>opt.MapFrom(src=>src.Category.ProductCategoryName))
               .ForMember(dest => dest.HsnCode, opt => opt.MapFrom(src => src.HsnCode.HSNCodeNumber));

            CreateMap<Product, SearchProductDto>();

            CreateMap<Product, ProductExportDto>()
               .ForMember(dest => dest.ProductCategory, opt => opt.MapFrom(src => src.Category.ProductCategoryName))

               .ForMember(dest => dest.UnitOfMeasures,
                   opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement))
              
               .ForMember(dest => dest.TaxRate, opt => opt.MapFrom(src => src.HsnCode.GstRate));


            CreateMap<HsnCode, HsnDto>().ReverseMap();
            CreateMap<HsnCode,UpdateHsnDto>().ReverseMap();
            CreateMap<Product, GetLowStockDTO>();
            CreateMap<AddPurchaseOrderDto, PurchaseOrder>()
              .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTime.UtcNow))
              .ForMember(dest => dest.PurchaseOrderItems, opt => opt.MapFrom(src => src.Items));

            CreateMap<AddPurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Quantity * src.RatePerPiece));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.GstRate, opt => opt.MapFrom(src => src.Product.HsnCode.GstRate))

            .ForMember(dest => dest.RatePerPiece, opt => opt.MapFrom(src => src.RatePerPiece))
            .ForMember(dest => dest.NetTotal, opt => opt.MapFrom(src => src.Quantity * src.RatePerPiece))
            .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(src =>
                (src.Quantity * src.RatePerPiece) * ((src.Product.HsnCode != null ? src.Product.HsnCode.GstRate : 0) / 100)))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src =>
                (src.Quantity * src.RatePerPiece) * (1 + ((src.Product.HsnCode != null ? src.Product.HsnCode.GstRate : 0) / 100))))
            .ForMember(dest => dest.ReceivedQuantity, opt => opt.MapFrom(src => src.ReceivedQuantity));

            CreateMap<PurchaseOrder, PurchaseOrderDetailDto>()
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        (poi.RatePerPiece * poi.Quantity) * ((poi.Product.HsnCode != null ? poi.Product.HsnCode.GstRate : 0) / 100)
                    )))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        (poi.RatePerPiece * poi.Quantity) * (1 + ((poi.Product.HsnCode != null ? poi.Product.HsnCode.GstRate : 0) / 100))
                    )))
                .ForMember(dest => dest.NetAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        poi.RatePerPiece * poi.Quantity
                    )))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PurchaseOrderItems))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier));

            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.TaxAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        (poi.RatePerPiece * poi.Quantity) * ((poi.Product.HsnCode != null ? poi.Product.HsnCode.GstRate : 0) / 100)
                    )))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        (poi.RatePerPiece * poi.Quantity) * (1 + ((poi.Product.HsnCode != null ? poi.Product.HsnCode.GstRate : 0) / 100))
                    )))
                .ForMember(dest => dest.NetAmount, opt => opt.MapFrom(
                    src => src.PurchaseOrderItems.Sum(poi =>
                        poi.RatePerPiece * poi.Quantity
                    )))
                .ForMember(dest => dest.Supplier, opt => opt.MapFrom(src => src.Supplier));



            CreateMap<Purchase,GetPurchaseDto>()
                .ForMember(dest=>dest.TotalAmount,opt=>opt.MapFrom(src=>src.PurchaseInvoice.TotalAmount))

                .ForMember(dest => dest.PurchaseInvoiceNumber, opt => opt.MapFrom(src => src.PurchaseInvoice.InvoiceNumber));


            CreateMap<Purchase, GetPurchaseDetailsDto>()
                .ForMember(dest => dest.PurchaseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest=>dest.Items,opt=>opt.MapFrom(src=>src.PurchaseItems))
                .ForMember(dest=>dest.SupplierDetails,opt=>opt.MapFrom(src=>src.Supplier))
                .ForMember(dest => dest.PurchaseInvoiceNumber, opt => opt.MapFrom(src => src.PurchaseInvoice.InvoiceNumber))
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrder.PurchaseOrderNumber));

            CreateMap<PurchaseItem, PurchaseItemDetailsDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));



            //CreateMap<PurchaseReturnInvoice, GetPurchaseReturnDto>();
            CreateMap<PurchaseReturn, GetPurchaseReturnDto>()
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.PurchaseReturnInvoice.InvoiceNumber))
                 .ForMember(dest => dest.originalInvoiceNumber, opt => opt.MapFrom(src => src.Purchase.PurchaseInvoice.InvoiceNumber))
                 .ForMember(dest=>dest.TotalAmount,opt=>opt.MapFrom(src=>src.PurchaseReturnInvoice.TotalAmount))
                 .ForMember(dest=>dest.ReturnedQuantity,opt=>opt.MapFrom(src=>src.Items.Sum(x=>x.ReturnedQuantity)));
            ;
      
            CreateMap<PurchaseReturn,GetPurchaseReturnDetailsDto>()
                .ForMember(dest => dest.InvoiceNumber, opt => opt.MapFrom(src => src.PurchaseReturnInvoice.InvoiceNumber))
                 .ForMember(dest => dest.originalInvoiceNumber, opt => opt.MapFrom(src => src.Purchase.PurchaseInvoice.InvoiceNumber))
                 .ForMember(dest => dest.GrossTotalAmount, opt => opt.MapFrom(src => src.PurchaseReturnInvoice.TotalAmount))
                 .ForMember(dest => dest.ReturnedQuantity, opt => opt.MapFrom(src => src.Items.Sum(x => x.ReturnedQuantity)))
                 .ForMember(dest=>dest.PurchaseDate,opt=>opt.MapFrom(src=>src.Purchase.Purchasedate))
                 .ForMember(dest => dest.PurchaseReturnItemsDetails, opt => opt.MapFrom(src => src.Items))
                 .ForMember(dest=>dest.SupplierDetails,opt=>opt.MapFrom(src=>src.Supplier))
            ;

            CreateMap<PurchaseReturnItem, PurchaseReturnItemsDetailsDto>()
                .ForMember(dest => dest.OriginalQuantity, opt => opt.MapFrom(src => src.PurchaseItem.Quantity))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount+src.TaxAmount))
                .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.Product.ProductName))
                .ForMember(dest=>dest.Reason,opt=>opt.MapFrom(src=>src.PurchaseReturn.Reason));
       


            CreateMap<PurchaseReturnItemDto, PurchaseReturnItem>();
            CreateMap<SupplierDto, Supplier>().ReverseMap();

            CreateMap<VoucherDto, Voucher>()
                .ForMember(dest => dest.TransactionsDebit, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionsCredit, opt => opt.Ignore());
            CreateMap<TransactionDto, Transaction>();

            CreateMap<Supplier, SupplierShortDto>();

            //CreateMap<Purchase, PurchaseItemDetailsDto>()
            //    .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.Purchase.PurchaseItems.))


        }

    }
}
