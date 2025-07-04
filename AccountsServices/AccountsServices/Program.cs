using System.Data;
using AccountsService.Extensions;
using AccountsServices.Service;
using Application.Interfaces.IServices;
using Application.Services.AccountsService;
using Infrastructure.DapperContext;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Services.AddScoped<DapperConection>();


builder.Host.UseSerilog(); 
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
});
builder.Services.AddScoped<IVoucherService, VoucherService>();


var app = builder.Build();

app.MapGrpcService<AccountsGroupgRPCService>();
app.MapGrpcService<LedgerGrpcService>();
app.MapGrpcService<VoucherGrpcService>();


app.MapGet("/", () => "Use a gRPC client to communicate with gRPC endpoints.");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
