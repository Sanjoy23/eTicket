using Event.API.Application.Sessions.Commands;
using MediatR;

namespace Event.API.BackgroundJobs
{
    public class SeatLockExpiryJob(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(2));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ReleaseExpiredLocksCommand(), stoppingToken);
            }
        }
        public override void Dispose()
        {
            _timer.Dispose();
            GC.SuppressFinalize(this);
            base.Dispose();
        }
    }
}
