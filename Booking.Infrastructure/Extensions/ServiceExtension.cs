using Booking.Domain.Repositories;
using Booking.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();
            return services;
        }
    }
}
