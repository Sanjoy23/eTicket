using Booking.API.Application.Commands;
using Booking.Domain.Repositories;
using MediatR;

namespace Booking.API.Application.Handlers
{
    public class BookingCancelledCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelSeatBookingCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public Task<Unit> Handle(CancelSeatBookingCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
