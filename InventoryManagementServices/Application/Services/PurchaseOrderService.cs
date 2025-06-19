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

namespace Application.Services
{
    public interface IPurchaseOrderService
    {
        Task<Responses<string>> AddPurchaseOrderAsync(Guid orgnaizationId, AddPurchaseOrderDto dto);
        Task<Responses<List<PurchaseOrderDto>>> GetAllOrdersAsync(Guid orgnaizationId);
        Task<Responses<PurchaseOrderDto>> GetOrderByIdAsync(Guid id);
        Task<Responses<string>> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto dto);
        Task<Responses<string>> DeleteOrderAsync(Guid id);
        Task<byte[]> ExportPurchaseOrderPdfBytesAsync(Guid id);
    }
    public class PurchaseOrderService:IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IMapper _mapper;
        private readonly IInvoiceNumberGenerator invoiceNumberGenerator;

        public PurchaseOrderService(IPurchaseOrderRepository repo, IMapper mapper, IInvoiceNumberGenerator _invoiceNumberGenerator)
        {
            _repo = repo;
            _mapper = mapper;
            invoiceNumberGenerator = _invoiceNumberGenerator;
        }
        public async Task<Responses<string>> AddPurchaseOrderAsync(Guid orgnaizationId, AddPurchaseOrderDto dto)
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
                    CreatedBy = dto.CreatedBy,
                    OrderDate = DateTime.UtcNow,
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
        public async Task<Responses<PurchaseOrderDto>> GetOrderByIdAsync(Guid id)
        {
            var order = await _repo.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                return new Responses<PurchaseOrderDto> { StatusCode = 404, Message = "Order Not Found" };

            var result = _mapper.Map<PurchaseOrderDto>(order);
            return new Responses<PurchaseOrderDto> {Message="PurchaseOrder Fetched", StatusCode = 200, Data = result };
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


        public async Task<byte[]> ExportPurchaseOrderPdfBytesAsync(Guid id)
        {
            var order = await _repo.GetPurchaseOrderByIdAsync(id);
            if (order == null)
                throw new Exception("Purchase Order not found");

            var orderDto = _mapper.Map<PurchaseOrderDto>(order);
            return PurchaseOrderPdfGeneratorHelper.GeneratePdf(orderDto);
        }

    }
}
