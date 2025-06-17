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
                .ForMember(dest => dest.BarCodeImageBase64,
                    opt => opt.MapFrom(src => Convert.ToBase64String(src.BarCodeImageBase64)))
                .ForMember(dest => dest.ProductImagesBase64,
                       opt => opt.MapFrom(src =>
                      src.Images.Select(img => Convert.ToBase64String(img.ImageData)).ToList()))
                .ForMember(dest=>dest.TaxRate,opt=>opt.MapFrom(src=>src.HsnCode.GstRate));




            CreateMap<Product, ProductCreatedEvent>().ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.Gst, opt => opt.MapFrom(src => src.HsnCode.HsnCodeId)).ForMember(dest => dest.UnitOfMeasure, opt => opt.MapFrom(src => src.UnitOfMeasures.Measurement)).ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ProductCategoryName));


            CreateMap<Product,ProductBillingGetDto>()
               .ForMember(dest=>dest.UnitOfMeasures,opt=>opt.MapFrom(src=>src.UnitOfMeasures.Measurement))
               .ForMember(dest=>dest.TaxRate,opt=>opt.MapFrom(src=>src.HsnCode.GstRate))
               .ForMember(dest=>dest.ProductCategory,opt=>opt.MapFrom(src=>src.Category.ProductCategoryName));

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
              .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => "Pending"))
              .ForMember(dest => dest.PurchaseOrderItems, opt => opt.MapFrom(src => src.Items));

            CreateMap<AddPurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Quantity * src.RatePerPiece));

            CreateMap<PurchaseOrder, PurchaseOrderDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.PurchaseOrderItems))
                .ForMember(dest=>dest.SupplierName,opt=>opt.MapFrom(src=>src.Supplier.Name));

            CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.Quantity * src.RatePerPiece));

            CreateMap<Purchase,GetPurchaseDto>()
                .ForMember(dest=>dest.TotalAmount,opt=>opt.MapFrom(src=>src.PurchaseInvoice.TotalAmount))

                .ForMember(dest => dest.PurchaseInvoiceNumber, opt => opt.MapFrom(src => src.PurchaseInvoice.InvoiceNumber));

                ;


            CreateMap<Purchase, GetPurchaseDetailsDto>()
                .ForMember(dest => dest.PurchaseId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest=>dest.Items,opt=>opt.MapFrom(src=>src.PurchaseItems))
                .ForMember(dest=>dest.SupplierDetails,opt=>opt.MapFrom(src=>src.Supplier));

            CreateMap<PurchaseItem, PurchaseItemDetailsDto>()
                .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.Product.ProductName));

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



            //CreateMap<Purchase, PurchaseItemDetailsDto>()
            //    .ForMember(dest=>dest.ProductName,opt=>opt.MapFrom(src=>src.Purchase.PurchaseItems.))


        }

    }
}
