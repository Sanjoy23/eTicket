using MediatR;
using Microsoft.EntityFrameworkCore;
using ePayment.API.Infrastructure.Data;
using ePayment.API.Models;
using ePayment.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration["ConnectionStrings:DefaultConnection"],
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory_Payment")));
builder.Services
    .AddOptions<SslCommerzOptions>()
    .Bind(builder.Configuration.GetSection("SSLCommerz"))
    .Validate(options => !string.IsNullOrWhiteSpace(options.StoreId), "SSLCommerz:StoreId is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.StorePassword), "SSLCommerz:StorePassword is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.SuccessUrl), "SSLCommerz:SuccessUrl is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.FailUrl), "SSLCommerz:FailUrl is required.")
    .Validate(options => !string.IsNullOrWhiteSpace(options.CancelUrl), "SSLCommerz:CancelUrl is required.")
    .ValidateOnStart();

builder.Services.AddHttpClient<IPaymentGateway, SslCommerzPaymentGateway>();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
