using Booking.API.Interfaces;

namespace Booking.API.Services
{
    public class SeatLockService(IHttpClientFactory httpClientFactory) : ISeatLockService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task LockSeatsAsync(Guid userId, Guid sessionId, List<Guid> seatIds, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("EventService");
            var request = new
            {
                SessionId = sessionId,
                UserId = userId,
                SeatIds = seatIds,
                LockDurationMinutes = 5
            };

            var response = await client.PostAsJsonAsync($"api/Seats/sessions/{sessionId}/seats/lock", request,cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"Status: {response.StatusCode}, Body: {content}");

            if (!response.IsSuccessStatusCode) { 
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"Seat lock failed. Event service returned {(int)response.StatusCode}: {error}",
                    inner: null,
                    response.StatusCode);
            }
        }
        public async Task ReleaseSeatsAsync(Guid sessionId, Guid userId, List<Guid> seatIds, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("EventService");
            var request = new
            {
                SessionId = sessionId,
                UserId = userId,
                SeatIds = seatIds
            };

            var response = await client.PostAsJsonAsync($"api/Seats/sessions/{sessionId}/seats/release", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"Seat release failed. Event service returned {(int)response.StatusCode}: {error}",
                    inner: null,
                    response.StatusCode);
            }
        }

        public async Task ConfirmSeatsAsync(Guid sessionId, Guid bookingId, Guid userId, List<Guid> seatIds, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient("EventService");
            var request = new
            {
                SessionId = sessionId,
                BookingId = bookingId,
                UserId = userId,
                SeatIds = seatIds
            };

            var response = await client.PostAsJsonAsync($"api/Seats/sessions/{sessionId}/seats/confirm", request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException(
                    $"Seat confirmation failed. Event service returned {(int)response.StatusCode}: {error}",
                    inner: null,
                    response.StatusCode);
            }
        }
    }
}
