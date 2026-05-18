using Event.API.Application.Sessions.Commands;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class ReleaseSessionSeatsHandler : IRequestHandler<ReleaseSessionSeatsCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReleaseSessionSeatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(ReleaseSessionSeatsCommand request, CancellationToken cancellationToken)
        {
            if (request.SessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required.");

            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required.");

            var seatIds = request.SeatIds?.Distinct().ToArray() ?? Array.Empty<Guid>();
            if (seatIds.Length == 0)
                throw new ArgumentException("At least one seat id must be provided.");

            var session = await _unitOfWork.EventsSession.GetById(request.SessionId);
            if (session == null)
                throw new KeyNotFoundException($"Session with ID {request.SessionId} not found.");

            var locks = (await _unitOfWork.SeatLocks.GetActiveLocksForUser(request.SessionId, request.UserId, seatIds, DateTime.UtcNow)).ToList();
            if (locks.Count != seatIds.Length)
                throw new InvalidOperationException("One or more requested seats are not locked by this user.");

            var inventories = (await _unitOfWork.EventsSeatInventory.GetBySessionIdAndSeatIds(request.SessionId, seatIds)).ToList();
            if (inventories.Count != seatIds.Length)
                throw new InvalidOperationException("One or more requested seats do not belong to this session.");

            foreach (var lockRecord in locks)
            {
                _unitOfWork.SeatLocks.Delete(lockRecord);
            }

            foreach (var inventory in inventories)
            {
                if (inventory.Status == SeatInventoryStatus.Locked)
                {
                    inventory.Status = SeatInventoryStatus.Available;
                    _unitOfWork.EventsSeatInventory.Update(inventory);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
