using System;
using System.Data;
using System.Text.Json.Serialization;
using Application.AutoMapper;
using BillingService.Extension;
using Infrastructure.BillingContext;
using Microsoft.EntityFrameworkCore;
using LedgerGrpc;
using PurchaseGrpc;
using QuestPDF.Infrastructure;
using Serilog;
using Application.Interfaces.Grpc_Interface;
using Infrastructure.Grpc_Client;


var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Services.AddGrpcClient<PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7117"); // URL of gRPC server
});
builder.Services.AddGrpcClient<LedgerGrpc.LedgerService.LedgerServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7117"); // Update this to correct Inventory Service URL
});

builder.Services.AddScoped<IAddLedger, Add_LedgerGrpc>();
//builder.Services.AddScoped<IAccountGrpc,AccountGrpc_Client>();
builder.Host.UseSerilog();       
// Add services to the container.
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddControllers();
builder.Services.addApplicationService();
//builder.Services.AddScoped<ProductConsumerService>();


// Set license type for QuestPDF
QuestPDF.Settings.License = LicenseType.Community;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddDbContext<BillingDbContext>(option => option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
// In Program.cs (or your DbContext configuration)
//builder.Services.AddDbContext<BillingDbContext>(options =>
//{
//    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//    var serverVersion = ServerVersion.AutoDetect(connectionString);
//    options.UseMySql(connectionString, serverVersion); // Note: "UseMySql" (not UseMySQL)
//});
var app = builder.Build();

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
