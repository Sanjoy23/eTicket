using Event.Domain.Entities.Events;

namespace Event.Infrastructure.Specifications
{
    public class SessionsWithVenuesAndHallsSpecification : Specification<EventSession>
    {
        public SessionsWithVenuesAndHallsSpecification(SessionSpecParams sessionParams) 
            : base(x =>(sessionParams.HallId.HasValue || sessionParams.HallId == x.HallId) &&
            (sessionParams.VenueId.HasValue || sessionParams.VenueId == x.VenueId))
        {
            AddInclude(x => x.Venue);
            AddInclude(x => x.Hall);
            ApplyPaging(sessionParams.PageSize * (sessionParams.PageIndex - 1), sessionParams.PageSize);
        }

        public SessionsWithVenuesAndHallsSpecification(Guid id) : base(x => x.EventSessionId == id)
        {
            AddInclude(x => x.Venue);
            AddInclude(x => x.Hall);
        }
    }
}
