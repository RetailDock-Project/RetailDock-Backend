using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Helpers;
using Application.Interfaces;
using Application.Interfaces.IServices;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<Responses<string>> AddPurchaseOrderAsync(Guid orgnaizationId,Guid userId, AddPurchaseOrderDto dto);
        Task<Responses<List<PurchaseOrderDto>>> GetAllOrdersAsync(Guid orgnaizationId);
        Task<Responses<PurchaseOrderDetailDto>> GetOrderByIdAsync(Guid id);
        //Task<Responses<string>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto);
        Task<Responses<string>> DeleteOrderAsync(Guid id);
        Task<byte[]> ExportPurchaseOrderPdfBytesAsync(Guid id,Guid orgId);
        Task<Responses<List<PurchaseOrderDto>>> GetAllOrdersAsync(
    Guid orgId,
    string? searchString,
    string? status,
    DateTime? startDate,
    DateTime? endDate,
    int? pageNumber,
    int? pageSize);

        Task<Responses<object>> GetOrderStatsAsync(Guid orgId);
        Task<Responses<string>> UpdatePurchaseOrderAsync(Guid orgId, Guid userId, UpdatePurchaseOrderDto dto);    }
    public class PurchaseOrderService:IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IMapper _mapper;
        private readonly IInvoiceNumberGenerator invoiceNumberGenerator;
        private readonly IOrganizationService organizationService;

        public PurchaseOrderService(IPurchaseOrderRepository repo, IMapper mapper, IInvoiceNumberGenerator _invoiceNumberGenerator, IOrganizationService _organizationService)
        {
            _repo = repo;
            _mapper = mapper;
            invoiceNumberGenerator = _invoiceNumberGenerator;
            organizationService = _organizationService;
        }
        public async Task<Responses<string>> AddPurchaseOrderAsync(Guid orgnaizationId,Guid userId, AddPurchaseOrderDto dto)
        {
            try
            {
                var lastPurchaseOrderNumber = await _repo.GetLastPurchaseOrderNumber(orgnaizationId);
                var newPoNumber = await invoiceNumberGenerator.GenerateInvoiceNumber(lastPurchaseOrderNumber, "PO");
                var order = new PurchaseOrder
                {
                    OrganizationId = orgnaizationId,
                    PurchaseOrderNumber= newPoNumber,
                    PurchaseOrderId = Guid.NewGuid(),
                    SupplierId = dto.SupplierId,
                    CreatedBy = userId,
                    OrderDate = dto.OrderDate ?? DateTime.UtcNow,
                    OrderStatus = "Pending",
                    GrossTotalAmount = dto.Items.Sum(x => x.Quantity * x.RatePerPiece),
                    PurchaseOrderItems = dto.Items.Select(x => new PurchaseOrderItem
                    {
                        PurchaseOrderItemId = Guid.NewGuid(),
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        RatePerPiece = x.RatePerPiece,
                        TotalAmount = x.Quantity * x.RatePerPiece
                    }).ToList()
                };

                await _repo.AddPurchaseOrderAsync(order);
                return new Responses<string> { StatusCode = 201, Message = "Order Created", Data = order.PurchaseOrderId.ToString() };
            }
            catch (Exception ex)
            {
                return new Responses<string> { StatusCode = 500, Message = ex.Message };
            }
        }
        public async Task<Responses<List<PurchaseOrderDto>>> GetAllOrdersAsync(Guid orgnaizationId)
        {
            var orders = await _repo.GetAllPurchaseOrdersAsync(orgnaizationId);
            var result = _mapper.Map<List<PurchaseOrderDto>>(orders);
            return new Responses<List<PurchaseOrderDto>> {Message="PurchaseOrder Fetched", StatusCode = 200, Data = result };
        }
        public async Task<Responses<PurchaseOrderDetailDto>> GetOrderByIdAsync(Guid id)
        {
            var order = await _repo.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                return new Responses<PurchaseOrderDetailDto> { StatusCode = 404, Message = "Order Not Found" };

            var result = _mapper.Map<PurchaseOrderDetailDto>(order);
            return new Responses<PurchaseOrderDetailDto> {Message="PurchaseOrder Fetched", StatusCode = 200, Data = result };
        }
        public async Task<Responses<string>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto)
        {
            var order = await _repo.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                return new Responses<string> { StatusCode = 404, Message = "Order Not Found" };

            order.OrderStatus = dto.OrderStatus;
            await _repo.UpdatePurchaseOrderAsync(order);
            return new Responses<string> { StatusCode = 200, Message = "Status Updated" };
        }
        public async Task<Responses<string>> DeleteOrderAsync(Guid id)
        {
            var deleted = await _repo.DeletePurchaseOrderAsync(id);
            if (!deleted)
                return new Responses<string> { StatusCode = 404, Message = "Order Not Found" };

            return new Responses<string> { StatusCode = 200, Message = "Order Deleted" };
        }


        public async Task<byte[]> ExportPurchaseOrderPdfBytesAsync(Guid id,Guid orgId)
        {
            var order = await _repo.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                throw new Exception("Purchase Order not found");
            var organizationDatail= await organizationService.GetOrganizationByIdAsync(orgId);
            var orderDto = _mapper.Map<PurchaseOrderDetailDto>(order);
            return PurchaseOrderPdfGeneratorHelper.GeneratePdf(orderDto, organizationDatail);
        }


        public async Task<Responses<List<PurchaseOrderDto>>> GetAllOrdersAsync(
    Guid orgId,
    string? searchString,
    string? status,
    DateTime? startDate,
    DateTime? endDate,
    int? pageNumber,
    int? pageSize)
        {
            var orders = await _repo.GetAllPurchaseOrdersAsync(orgId, searchString, status, startDate, endDate, pageNumber, pageSize);
            var result = _mapper.Map<List<PurchaseOrderDto>>(orders);
            return new Responses<List<PurchaseOrderDto>> { Message = "Purchase Orders Fetched", StatusCode = 200, Data = result };
        }

        public async Task<Responses<object>> GetOrderStatsAsync(Guid orgId)
        {
            var stats = await _repo.GetPurchaseOrderStatsAsync(orgId);
            return new Responses<object>
            {
                StatusCode = 200,
                Message = "Purchase Order Stats Fetched",
                Data = stats
            };
        }


        public async Task<Responses<string>> UpdatePurchaseOrderAsync(Guid orgId, Guid userId, UpdatePurchaseOrderDto dto)
        {
            var existingOrder = await _repo.GetByIdAsync(dto.PurchaseOrderId);

            if (existingOrder == null)
            {
                return new Responses<string>
                {
                    Message = "Purchase Order not found",
                    StatusCode = 404,
                    Data = null
                };
            }

            existingOrder.SupplierId = dto.SupplierId;
            existingOrder.OrderDate = dto.OrderDate ?? existingOrder.OrderDate;
            existingOrder.UpdatedAt = DateTime.UtcNow;
            existingOrder.UpdatedBy = userId;

            var updatedItems = new List<PurchaseOrderItem>();

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.PurchaseOrderItemId != null && itemDto.PurchaseOrderItemId != Guid.Empty)
                {
                    // Update existing item
                    var existingItem = existingOrder.PurchaseOrderItems
                        .FirstOrDefault(i => i.PurchaseOrderItemId == itemDto.PurchaseOrderItemId);

                    if (existingItem != null)
                    {
                        existingItem.ProductId = itemDto.ProductId;
                        existingItem.Quantity = itemDto.Quantity;
                        existingItem.RatePerPiece = itemDto.RatePerPiece;
                        existingItem.TotalAmount = itemDto.Quantity * itemDto.RatePerPiece;
                    }
                }
                else
                {
                    // Add new item
                    updatedItems.Add(new PurchaseOrderItem
                    {
                        PurchaseOrderItemId = Guid.NewGuid(),
                        PurchaseOrderId = existingOrder.PurchaseOrderId,
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        RatePerPiece = itemDto.RatePerPiece,
                        TotalAmount = itemDto.Quantity * itemDto.RatePerPiece
                    });
                }
            }

            if (updatedItems.Any())
            {
                await _repo.AddItemsAsync(updatedItems);
            }

            await _repo.UpdateAsync(existingOrder);

            return new Responses<string>
            {
                Message = "Purchase Order updated successfully",
                StatusCode = 200,
                Data = null
            };
        }







    }
}
