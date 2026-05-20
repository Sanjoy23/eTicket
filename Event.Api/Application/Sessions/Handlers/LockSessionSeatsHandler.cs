using Event.API.Application.Sessions.Commands;
using Event.API.Models.DTOs;
using Event.Domain.Entities.Events;
using Event.Domain.Entities.Seating;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class LockSessionSeatsHandler : IRequestHandler<LockSessionSeatsCommand, SeatLockResultDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LockSessionSeatsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SeatLockResultDto> Handle(LockSessionSeatsCommand request, CancellationToken cancellationToken)
        {
            if (request.SessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required.");

            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId is required.");

            var seatIds = request.SeatIds?.Distinct().ToArray() ?? Array.Empty<Guid>();
            if (seatIds.Length == 0)
                throw new ArgumentException("At least one seat id must be provided.");

            if (request.LockDurationMinutes <= 0)
                throw new ArgumentException("LockDurationMinutes must be greater than zero.");

            var session = await _unitOfWork.EventsSession.GetById(request.SessionId);
            if (session == null)
                throw new KeyNotFoundException($"Session with ID {request.SessionId} not found.");

            if (session.Status == SessionStatus.Cancelled)
                throw new InvalidOperationException("Cannot lock seats for a cancelled session.");

            var inventories = (await _unitOfWork.EventSeatInventories.GetBySessionIdAndSeatIds(request.SessionId, seatIds)).ToList();
            if (inventories.Count != seatIds.Length)
                throw new InvalidOperationException("One or more requested seats do not belong to this session.");

            var utcNow = DateTime.UtcNow;
            var activeLocks = (await _unitOfWork.SeatLocks.GetActiveLocks(request.SessionId, seatIds, utcNow)).ToList();
            var conflictingLock = activeLocks.FirstOrDefault(lockRecord => lockRecord.UserId != request.UserId);
            if (conflictingLock != null)
                throw new InvalidOperationException("One or more requested seats are already locked by another user.");

            var existingUserLockIds = activeLocks
                .Where(lockRecord => lockRecord.UserId == request.UserId)
                .Select(lockRecord => lockRecord.SeatId)
                .ToHashSet();

            var lockedUntilUtc = utcNow.AddMinutes(request.LockDurationMinutes);

            foreach (var inventory in inventories)
            {
                if (inventory.Status != SeatInventoryStatus.Available && inventory.Status != SeatInventoryStatus.Locked)
                {
                    throw new InvalidOperationException($"Seat {inventory.SeatId} is not available for locking.");
                }

                inventory.Status = SeatInventoryStatus.Locked;
                _unitOfWork.EventSeatInventories.Update(inventory);

                if (existingUserLockIds.Contains(inventory.SeatId))
                {
                    var existingLock = activeLocks.First(lockRecord => lockRecord.UserId == request.UserId && lockRecord.SeatId == inventory.SeatId);
                    existingLock.LockedUntilUtc = lockedUntilUtc;
                }
                else
                {
                    await _unitOfWork.SeatLocks.Add(new SeatLock
                    {
                        Id = Guid.NewGuid(),
                        EventSessionId = request.SessionId,
                        SeatId = inventory.SeatId,
                        UserId = request.UserId,
                        LockedUntilUtc = lockedUntilUtc,
                        CreatedAtUtc = utcNow
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SeatLockResultDto
            {
                EventSessionId = request.SessionId,
                UserId = request.UserId,
                LockedUntilUtc = lockedUntilUtc,
                SeatIds = inventories.Select(inv => inv.SeatId).ToList()
            };
        }
    }
}
