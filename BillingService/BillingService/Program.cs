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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Infrastructure.Grpc_Client;
using BillingService.ActionFillter;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // ✅ Set your frontend URL
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // ✅ Required for withCredentials
    });
});



builder.Services.AddGrpcClient<PurchaseGrpc.VoucherGrpcService.VoucherGrpcServiceClient>(o =>
{
    o.Address = new Uri("https://localhost:7117"); // URL of gRPC server
});


builder.Services.AddGrpcClient<LedgerGrpc.LedgerService.LedgerServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7117"); // Update this to correct Inventory Service URL
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BillingService API", Version = "v1" });

    // 1. Bearer token in Authorization header (for Postman, Flutter, etc.)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // 2. Cookie-based token (for browser sessions)
    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Name = "accessToken",
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Description = "JWT token stored in the 'accessToken' cookie"
    });

    // Enable both in security requirement
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "cookieAuth"
                            }
                        },
                        Array.Empty<string>()
                    }
                });



});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);





builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // 1. Check Authorization header
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    context.Token = authHeader["Bearer ".Length..]; // Get token after "Bearer "
                }

                // 2. Fallback to cookie if Authorization header is not present
                if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("accessToken"))
                {
                    context.Token = context.Request.Cookies["accessToken"];
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters =new TokenValidationParameters
        {
            ValidateIssuer = false, // set to true and configure if you have issuer
            ValidateAudience = false, // set to true and configure if you have audience
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero, // no token time leeway
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });

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
builder.Services.AddControllers(options =>
{
    // ✅ Global filter for checking OrgId and UserId
    options.Filters.Add<RequireOrg_UserAttribute>();
})
.AddJsonOptions(options =>
{
    // ✅ Enum as strings in JSON
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
app.UseCors("AllowAll");
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
