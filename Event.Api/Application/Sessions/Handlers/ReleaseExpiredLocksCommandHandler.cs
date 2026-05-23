using Event.API.Application.Sessions.Commands;
using Event.Domain.Enums;
using Event.Domain.Repositories;
using MediatR;

namespace Event.API.Application.Sessions.Handlers
{
    public class ReleaseExpiredLocksCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReleaseExpiredLocksCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Unit> Handle(ReleaseExpiredLocksCommand request, CancellationToken cancellationToken)
        {
            var expiredLocks = await _unitOfWork.SeatLocks.GetExpiredLocks(DateTime.UtcNow);

            foreach(var locks in expiredLocks)
            {
                var inventory = await _unitOfWork.EventSeatInventories.GetById(locks.Id);
                if (inventory != null && inventory.Status == SeatInventoryStatus.Locked) { 
                    inventory.Status = SeatInventoryStatus.Available;
                    _unitOfWork.EventSeatInventories.Update(inventory);
                }
                _unitOfWork.SeatLocks.Delete(locks);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
