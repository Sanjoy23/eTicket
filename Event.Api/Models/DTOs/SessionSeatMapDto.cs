using Event.Domain.Enums;
using System.Collections.Generic;

namespace Event.API.Models.DTOs
{
    public record SeatInventoryDto
    {
        public Guid SeatId { get; init; }
        public string SeatCode { get; init; } = default!;
        public string RowLabel { get; init; } = default!;
        public int SeatNumber { get; init; }
        public SeatType SeatType { get; init; }
        public bool IsAccessible { get; init; }
        public SeatInventoryStatus Status { get; init; }
        public decimal Price { get; init; }
        public string Currency { get; init; } = default!;
        public Guid? BookingId { get; init; }
    }

    public record SessionSeatMapDto
    {
        public Guid EventSessionId { get; init; }
        public Guid EventId { get; init; }
        public Guid VenueId { get; init; }
        public Guid HallId { get; init; }
        public IEnumerable<SeatInventoryDto> Seats { get; init; } = Array.Empty<SeatInventoryDto>();
    }
}
