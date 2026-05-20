using Event.API.Application.Sessions.Commands;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class SeatsConfirmedCommandHandler : IRequestHandler<ConfirmSeatsCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeatsConfirmedCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ConfirmSeatsCommand request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var inventories = await _unitOfWork.EventSeatInventories.GetBySessionIdAndSeatIds(request.SessionId,request.SeatIds);
            var inventoryList = inventories.ToList();

            if (inventoryList.Count != request.SeatIds.Count)
                throw new Exception("One or more seats were not found.");
            
            var locks = await _unitOfWork.SeatLocks.GetBySessionIdAndSeatIds(request.SessionId, request.SeatIds);
            var lockList = locks.ToList();
            if(lockList.Count != request.SeatIds.Count)
                throw new Exception("One or more seats are not locked.");

            foreach (var seatId in request.SeatIds)
            {
                var inventory = inventoryList.First(x =>  x.SeatId == seatId);
                var seatLock = lockList.First(x => x.SeatId == seatId);

                if(inventory.Status != SeatInventoryStatus.Locked)
                    throw new Exception($"Seat {seatId} is not locked.");
                if (seatLock.UserId != request.UserId)
                    throw new Exception($"Seat {seatId} is locked by another user.");
                if (seatLock.LockedUntilUtc <= now)
                    throw new Exception($"Seat {seatId} lock has expired.");

                inventory.Status = SeatInventoryStatus.Sold;
                inventory.BookingId = request.BookingId;
                inventory.SoldAtUtc = now;
            }

            foreach (var seatLock in lockList)
            {
                _unitOfWork.SeatLocks.Delete(seatLock);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
