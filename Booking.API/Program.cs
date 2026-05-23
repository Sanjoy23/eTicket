using Booking.API.Interfaces;
using Booking.API.Services;
using Booking.Infrastructure.Data;
using Booking.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory_Booking")));
builder.Services.AddDIServices();
builder.Services.AddMediatR(typeof(Program));

builder.Services.AddHttpClient("EventService", client =>
{
    client.BaseAddress = new Uri("http://localhost:5111/");
});
builder.Services.AddScoped<ISeatLockService, SeatLockService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();
