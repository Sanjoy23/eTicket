
using Microsoft.EntityFrameworkCore;
using Booking.Infrastructure.Data;
using Booking.API.Services;
using Booking.API.Interfaces;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<EventDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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