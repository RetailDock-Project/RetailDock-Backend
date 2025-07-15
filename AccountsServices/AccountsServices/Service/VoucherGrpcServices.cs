using Application.DTO;
using Application.Interfaces.IServices;
using Grpc.Core;
using PurchaseGrpc;

public class VoucherGrpcService : PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceBase
{
    private readonly ILogger<VoucherGrpcService> _logger;
    private readonly IVoucherService _voucherService;

    public VoucherGrpcService(ILogger<VoucherGrpcService> logger, IVoucherService voucherService)
    {
        _logger = logger;
        _voucherService = voucherService;
    }

    public override async Task<ApiResponses> AddVoucherEntry(AddVoucherRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation("Received AddVoucherEntry request for org: {OrgId}", request.OrganizationId);

            var transactionsDebit = request.TransactionsDebit
                .Select(x => new TransactionsDTO
                {
                    LedgerId = Guid.Parse(x.LedgerId),
                    Amount = Convert.ToDecimal(x.Amount),
                    Narration = x.Narration
                }).ToList();

            var transactionsCredit = request.TransactionsCredit
                .Select(x => new TransactionsDTO
                {
                    LedgerId = Guid.Parse(x.LedgerId),
                    Amount = Convert.ToDecimal(x.Amount),
                    Narration = x.Narration
                }).ToList();
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");

            Console.WriteLine(transactionsDebit.Count); 
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");

            Console.WriteLine(transactionsCredit.Count);
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");
            Console.WriteLine("\n");

            var addVoucherDTO = new AddVouchersDTO
            {
                VoucherTypeId = Guid.Parse(request.VoucherTypeId),
                VoucherDate = DateTime.Parse(request.VoucherDate),
                Remarks = request.Remarks,
                TransactionsDebit = transactionsDebit,
                TransactionsCredit = transactionsCredit
            };

            var result = await _voucherService.AddVoucherEntrys(
                Guid.Parse(request.OrganizationId),
                Guid.Parse(request.CreatedBy),
                addVoucherDTO);

            return  new ApiResponses
            {
                StatusCode = result.StatusCode,
                Message = result.Message,
                Data = result.Data?.ToString() //
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in AddVoucherEntry");
            return new ApiResponses
            {
                StatusCode = 500,
                Message = "Internal Server Error",
                Data = ""
            };
        }
    }
}
