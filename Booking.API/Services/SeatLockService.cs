using Booking.API.Interfaces;

namespace Booking.API.Services
{
    public class SeatLockService : ISeatLockService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SeatLockService(IHttpClientFactory  httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task LockSeatsAsync(Guid userId, Guid sessionId, Guid bookingId, List<Guid> seatIds, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("EventService");
            var request = new
            {
                UserId = userId,
                BookingId = bookingId,
                SessionId = sessionId,
                SeatIds = seatIds,
            };

            var response = await client.PostAsJsonAsync($"api/sessions/{sessionId:guid}/seats/lock", request,cancellationToken);

            if (!response.IsSuccessStatusCode) { 
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new Exception($"Seat lock failed{error}");
            }
        }
    }
}
