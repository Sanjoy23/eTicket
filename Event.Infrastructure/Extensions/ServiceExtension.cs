using Event.Domain.Repositories;
using Event.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Event.Infrastructure.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services) {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IVenueRepository, VenueRepository>();
            services.AddScoped<IEventSessionRepository, EventSessionRepository>();
            services.AddScoped<IEventSeatInventoryRepository, EventSeatInventoryRepository>();
            services.AddScoped<ISeatLockRepository, SeatLockRepository>();
            return services;
        }
    }
}
