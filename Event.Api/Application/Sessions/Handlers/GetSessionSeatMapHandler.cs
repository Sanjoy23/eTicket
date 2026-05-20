using Event.API.Application.Sessions.Queries;
using Event.API.Models.DTOs;
using Event.Domain.Repositories;
using MediatR;
using System.Linq;

namespace Event.API.Application.Sessions.Handlers
{
    public class GetSessionSeatMapHandler : IRequestHandler<GetSessionSeatMapQuery, SessionSeatMapDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSessionSeatMapHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SessionSeatMapDto> Handle(GetSessionSeatMapQuery request, CancellationToken cancellationToken)
        {
            var session = await _unitOfWork.EventsSession.GetById(request.SessionId);
            if (session == null)
            {
                throw new KeyNotFoundException($"Session with ID {request.SessionId} not found.");
            }

            var seatInventories = await _unitOfWork.EventSeatInventories.GetBySessionId(request.SessionId);

            return new SessionSeatMapDto
            {
                EventSessionId = session.EventSessionId,
                EventId = session.EventId,
                VenueId = session.VenueId,
                HallId = session.HallId,
                Seats = seatInventories.Select(inv => new SeatInventoryDto
                {
                    SeatId = inv.SeatId,
                    SeatCode = inv.Seat.SeatCode,
                    RowLabel = inv.Seat.RowLabel,
                    SeatNumber = inv.Seat.SeatNumber,
                    SeatType = inv.Seat.SeatType,
                    IsAccessible = inv.Seat.IsAccessible,
                    Status = inv.Status,
                    Price = inv.Price,
                    Currency = inv.Currency,
                    BookingId = inv.BookingId
                }).ToList()
            };
        }
    }
}
