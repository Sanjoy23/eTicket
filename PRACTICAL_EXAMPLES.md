# Event Ticketing System - Practical Implementation Examples

## Example 1: Creating an Event (Command Pattern)

### Step 1: Define the Command (Application Layer)

**Location**: `Event.Api/Application/Commands/CreateEventCommand.cs`

```csharp
namespace Event.Api.Application.Commands;

public class CreateEventCommand : IRequest<int>
{
    public string EventName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSeats { get; set; }
    public int VenueId { get; set; }
    public EventType EventType { get; set; }
}
```

**What is this?**
- A Data Transfer Object (DTO) that represents a REQUEST
- Contains ONLY the data needed to create an event
- The `IRequest<int>` means: "This command returns an int (EventId)"

---

### Step 2: Define the Handler (Application Layer)

**Location**: `Event.Api/Application/Commands/CreateEventCommandHandler.cs`

```csharp
namespace Event.Api.Application.Commands;

public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    private readonly IEventRepository _eventRepository;
    private readonly IVenueRepository _venueRepository;
    private readonly ILogger<CreateEventCommandHandler> _logger;

    // Dependencies injected via constructor
    public CreateEventCommandHandler(
        IEventRepository eventRepository,
        IVenueRepository venueRepository,
        ILogger<CreateEventCommandHandler> logger)
    {
        _eventRepository = eventRepository;
        _venueRepository = venueRepository;
        _logger = logger;
    }

    public async Task<int> Handle(
        CreateEventCommand request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Creating event: {EventName} on {StartDate}",
            request.EventName, request.StartDate);

        // Step 1: Fetch the venue (check it exists)
        var venue = await _venueRepository.GetAsync(request.VenueId);
        if (venue == null)
            throw new VenueNotFoundException(request.VenueId);

        // Step 2: Create the Event aggregate (Domain logic)
        var newEvent = new Event(
            eventName: request.EventName,
            description: request.Description,
            startDate: request.StartDate,
            endDate: request.EndDate,
            totalSeats: request.TotalSeats,
            venue: venue,
            eventType: request.EventType
        );

        // Step 3: Apply any domain logic
        newEvent.SetEventStatus(EventStatus.Upcoming);

        // Step 4: Save to repository
        _eventRepository.Add(newEvent);

        // Step 5: Save all changes (triggers domain events)
        await _eventRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        _logger.LogInformation(
            "Event created successfully. EventId: {EventId}",
            newEvent.EventId);

        // Step 6: Return the ID
        return newEvent.EventId;
    }
}
```

**What is this?**
- Executes the CreateEventCommand
- Orchestrates between domain and repository
- Enforces business rules before saving
- Returns the created EventId

---

### Step 3: Add Validation (Application Layer)

**Location**: `Event.Api/Application/Commands/CreateEventCommandValidator.cs`

```csharp
namespace Event.Api.Application.Commands.Validations;

public class CreateEventCommandValidator : AbstractValidator<CreateEventCommand>
{
    public CreateEventCommandValidator()
    {
        RuleFor(cmd => cmd.EventName)
            .NotEmpty().WithMessage("Event name is required")
            .MinimumLength(3).WithMessage("Event name must be at least 3 characters");

        RuleFor(cmd => cmd.StartDate)
            .NotEmpty().WithMessage("Start date is required")
            .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future");

        RuleFor(cmd => cmd.EndDate)
            .GreaterThan(cmd => cmd.StartDate).WithMessage("End date must be after start date");

        RuleFor(cmd => cmd.TotalSeats)
            .GreaterThan(0).WithMessage("Total seats must be greater than 0")
            .LessThanOrEqualTo(10000).WithMessage("Total seats cannot exceed 10000");

        RuleFor(cmd => cmd.VenueId)
            .GreaterThan(0).WithMessage("Venue ID is required");
    }
}
```

**What is this?**
- Validates input BEFORE the handler runs
- Uses FluentValidation library
- Catches invalid data early
- Returns clear error messages to client

---

### Step 4: Define the API Endpoint (Presentation Layer)

**Location**: `Event.Api/Apis/EventsApi.cs`

```csharp
namespace Event.Api.Apis;

public static class EventsApi
{
    public static void MapEventsApiV1(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/v1/events")
            .WithName("Events")
            .WithOpenApi();

        group.MapPost("/", CreateEventAsync)
            .WithName("Create Event")
            .WithDescription("Creates a new event")
            .Produces<int>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{eventId}", GetEventAsync)
            .WithName("Get Event")
            .WithDescription("Gets event details by ID")
            .Produces<EventDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{eventId}", UpdateEventAsync)
            .WithName("Update Event")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);
    }

    // POST /api/v1/events
    private static async Task<IResult> CreateEventAsync(
        CreateEventRequest request,
        [AsParameters] EventServices services,
        CancellationToken cancellationToken)
    {
        // Map request DTO to command
        var command = new CreateEventCommand
        {
            EventName = request.EventName,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalSeats = request.TotalSeats,
            VenueId = request.VenueId,
            EventType = request.EventType
        };

        // Send command through mediator (which runs validation + handler)
        var eventId = await services.Mediator.Send(command, cancellationToken);

        // Return 201 Created with the new resource URI
        return Results.Created($"/api/v1/events/{eventId}", eventId);
    }

    // GET /api/v1/events/{eventId}
    private static async Task<IResult> GetEventAsync(
        int eventId,
        [AsParameters] EventServices services)
    {
        try
        {
            var eventDto = await services.Queries.GetEventByIdAsync(eventId);
            return Results.Ok(eventDto);
        }
        catch (EventNotFoundException)
        {
            return Results.NotFound();
        }
    }

    // PUT /api/v1/events/{eventId}
    private static async Task<IResult> UpdateEventAsync(
        int eventId,
        UpdateEventRequest request,
        [AsParameters] EventServices services,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEventCommand
        {
            EventId = eventId,
            EventName = request.EventName,
            Description = request.Description,
            TotalSeats = request.TotalSeats
        };

        var result = await services.Mediator.Send(command, cancellationToken);

        if (!result)
            return Results.BadRequest("Failed to update event");

        return Results.NoContent();
    }
}

// Services bundle - provides dependencies via [AsParameters]
public class EventServices(
    IMediator mediator,
    IEventQueries queries,
    ILogger<EventServices> logger)
{
    public IMediator Mediator { get; } = mediator;
    public IEventQueries Queries { get; } = queries;
    public ILogger<EventServices> Logger { get; } = logger;
}

// Request DTOs - what client sends
public class CreateEventRequest
{
    public string EventName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSeats { get; set; }
    public int VenueId { get; set; }
    public EventType EventType { get; set; }
}

public class UpdateEventRequest
{
    public string EventName { get; set; }
    public string Description { get; set; }
    public int TotalSeats { get; set; }
}
```

**What is this?**
- Defines HTTP endpoints
- Maps HTTP verbs to handler functions
- Converts HTTP requests to commands
- Uses Mediator to send commands
- Returns appropriate HTTP status codes

---

## Example 2: Getting Event Details (Query Pattern)

### Step 1: Define the Query (Application Layer)

**Location**: `Event.Api/Application/Queries/GetEventByIdQuery.cs`

```csharp
namespace Event.Api.Application.Queries;

public class GetEventByIdQuery : IRequest<EventDto>
{
    public int EventId { get; set; }
}
```

**What is this?**
- Represents a read request
- The `IRequest<EventDto>` means: "This query returns an EventDto"

---

### Step 2: Define the Handler (Application Layer)

**Location**: `Event.Api/Application/Queries/GetEventByIdQueryHandler.cs`

```csharp
namespace Event.Api.Application.Queries;

public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventQueries _queries;
    private readonly ILogger<GetEventByIdQueryHandler> _logger;

    public GetEventByIdQueryHandler(
        IEventQueries queries,
        ILogger<GetEventByIdQueryHandler> logger)
    {
        _queries = queries;
        _logger = logger;
    }

    public async Task<EventDto> Handle(
        GetEventByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching event: {EventId}", request.EventId);

        var eventDto = await _queries.GetEventByIdAsync(request.EventId);

        if (eventDto == null)
            throw new EventNotFoundException(request.EventId);

        return eventDto;
    }
}
```

**What is this?**
- Queries don't modify state, just read
- No validation behavior needed
- No transaction needed
- Optimized for fast reads

---

### Step 3: Define the Query Service (Infrastructure Layer)

**Location**: `Event.Infrastructure/Queries/EventQueries.cs`

```csharp
namespace Event.Infrastructure.Queries;

public interface IEventQueries
{
    Task<EventDto> GetEventByIdAsync(int eventId);
    Task<IEnumerable<EventSummaryDto>> GetAllEventsAsync();
    Task<IEnumerable<EventSummaryDto>> GetUpcomingEventsAsync();
}

public class EventQueries : IEventQueries
{
    private readonly EventDbContext _context;
    private readonly IMapper _mapper;

    public EventQueries(EventDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<EventDto> GetEventByIdAsync(int eventId)
    {
        // Optimized query for reading
        var @event = await _context.Events
            .Include(e => e.Venue)
            .Include(e => e.EventPerformers)
                .ThenInclude(ep => ep.Performer)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        return _mapper.Map<EventDto>(@event);
    }

    public async Task<IEnumerable<EventSummaryDto>> GetAllEventsAsync()
    {
        var events = await _context.Events
            .AsNoTracking()  // Read-only, no tracking
            .OrderByDescending(e => e.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EventSummaryDto>>(events);
    }

    public async Task<IEnumerable<EventSummaryDto>> GetUpcomingEventsAsync()
    {
        var now = DateTime.UtcNow;
        var events = await _context.Events
            .AsNoTracking()
            .Where(e => e.StartDate > now && e.Status == EventStatus.Upcoming)
            .OrderBy(e => e.StartDate)
            .ToListAsync();

        return _mapper.Map<IEnumerable<EventSummaryDto>>(events);
    }
}
```

**What is this?**
- Optimized queries for reading data
- Uses `.AsNoTracking()` for performance (no change tracking)
- Returns DTOs directly (not aggregates)
- Can use SQL joins, projections, etc.

---

### Step 4: Define DTOs (Application Layer)

**Location**: `Event.Api/Application/Queries/EventDto.cs`

```csharp
namespace Event.Api.Application.Queries;

// Full event details
public class EventDto
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public EventStatus Status { get; set; }
    public VenueDto Venue { get; set; }
    public IEnumerable<PerformerDto> Performers { get; set; }
}

// Summary for lists
public class EventSummaryDto
{
    public int EventId { get; set; }
    public string EventName { get; set; }
    public DateTime StartDate { get; set; }
    public int AvailableSeats { get; set; }
}

public class VenueDto
{
    public int VenueId { get; set; }
    public string VenueName { get; set; }
    public string Location { get; set; }
}

public class PerformerDto
{
    public int PerformerId { get; set; }
    public string PerformerName { get; set; }
}
```

**What is this?**
- Data Transfer Objects (DTOs) for responses
- Different from domain entities
- Optimized for API responses
- Only include data that clients need

---

## Example 3: Domain Aggregate (Core Business Logic)

**Location**: `Event.Domain/AggregatesModel/EventAggregate/Event.cs`

```csharp
namespace Event.Domain.AggregatesModel.EventAggregate;

public class Event : Entity, IAggregateRoot
{
    public string EventName { get; private set; }
    public string Description { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int TotalSeats { get; private set; }
    public int BookedSeats { get; private set; }
    public EventStatus Status { get; private set; }
    public Venue Venue { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Child entities (only access through aggregate)
    private readonly List<Ticket> _tickets;
    public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    // Domain events
    private List<INotification> _domainEvents = new();
    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public Event() { }

    // Factory method
    public static Event Create(
        string eventName,
        string description,
        DateTime startDate,
        DateTime endDate,
        int totalSeats,
        Venue venue)
    {
        // Validate business rules
        if (startDate >= endDate)
            throw new DomainException("Start date must be before end date");

        if (totalSeats <= 0)
            throw new DomainException("Total seats must be greater than 0");

        if (startDate < DateTime.UtcNow)
            throw new DomainException("Event cannot be in the past");

        var @event = new Event
        {
            EventName = eventName,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            TotalSeats = totalSeats,
            Venue = venue,
            Status = EventStatus.Upcoming,
            CreatedAt = DateTime.UtcNow,
            BookedSeats = 0,
            _tickets = new List<Ticket>()
        };

        // Publish domain event
        @event.AddDomainEvent(new EventCreatedDomainEvent(
            @event.Id, @event.EventName, @event.StartDate));

        return @event;
    }

    // Business logic: Book a ticket
    public void BookTicket(int ticketNumber, string customerId)
    {
        if (Status != EventStatus.Upcoming)
            throw new DomainException("Can only book tickets for upcoming events");

        var availableSeats = TotalSeats - BookedSeats;
        if (availableSeats <= 0)
            throw new DomainException("No seats available");

        var ticket = _tickets.FirstOrDefault(t => t.TicketNumber == ticketNumber);
        if (ticket == null)
            throw new DomainException($"Ticket {ticketNumber} not found");

        if (ticket.IsBooked)
            throw new DomainException($"Ticket {ticketNumber} is already booked");

        // Book the ticket
        ticket.Book(customerId);
        BookedSeats++;

        // Publish domain event
        AddDomainEvent(new TicketBookedDomainEvent(
            this.Id, ticketNumber, customerId));
    }

    // Business logic: Cancel event
    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            throw new DomainException("Event is already cancelled");

        Status = EventStatus.Cancelled;

        // Release all booked tickets
        foreach (var ticket in _tickets.Where(t => t.IsBooked))
        {
            ticket.Release();
        }

        BookedSeats = 0;

        // Publish domain event
        AddDomainEvent(new EventCancelledDomainEvent(this.Id, EventName));
    }

    public int GetAvailableSeats()
    {
        return TotalSeats - BookedSeats;
    }

    public void AddDomainEvent(INotification eventItem)
    {
        _domainEvents = _domainEvents ?? new List<INotification>();
        _domainEvents.Add(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}
```

**What is this?**
- The core business object (Aggregate Root)
- Contains ONLY business logic
- Has NO dependencies on database or HTTP
- Methods enforce business rules
- Publishes domain events when things happen

---

## Example 4: Repository Implementation (Infrastructure)

**Location**: `Event.Infrastructure/Repositories/EventRepository.cs`

```csharp
namespace Event.Infrastructure.Repositories;

public interface IEventRepository : IRepository<Event>
{
    void Add(Event @event);
    void Update(Event @event);
    Task<Event> GetAsync(int eventId);
    Task<IEnumerable<Event>> GetAllAsync();
}

public class EventRepository : IEventRepository
{
    private readonly EventDbContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public EventRepository(EventDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Add(Event @event)
    {
        _context.Events.Add(@event);
    }

    public void Update(Event @event)
    {
        _context.Entry(@event).State = EntityState.Modified;
    }

    public async Task<Event> GetAsync(int eventId)
    {
        var @event = await _context.Events
            .Include(e => e.Venue)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        return @event;
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        return await _context.Events
            .Include(e => e.Venue)
            .ToListAsync();
    }
}
```

**What is this?**
- Implements the repository interface from domain
- Hides EF Core details
- Can be swapped for another implementation
- Uses DbContext to persist aggregates

---

## Example 5: Dependency Injection Setup (Program.cs)

**Location**: `Event.Api/Program.cs`

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to DI container
var services = builder.Services;

// Database
services.AddDbContext<EventDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories (Infrastructure)
services.AddScoped<IEventRepository, EventRepository>();
services.AddScoped<IVenueRepository, VenueRepository>();

// Queries (Infrastructure)
services.AddScoped<IEventQueries, EventQueries>();

// MediatR (Application)
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

    // Add behavior pipeline
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});

// Validators
services.AddValidatorsFromAssemblyContaining(typeof(Program));

// AutoMapper
services.AddAutoMapper(typeof(Program));

// Logging
services.AddLogging();

var app = builder.Build();

// Map endpoints
var events = app.MapGroup("")
    .WithOpenApi();

events.MapEventsApiV1();

app.Run();
```

**What is this?**
- Central configuration of dependencies
- Wires everything together
- Container knows how to create each object
- Handlers get injected with dependencies automatically

---

## Complete Request Flow Example

```
HTTP POST /api/v1/events
{
  "eventName": "Concert 2024",
  "description": "Amazing concert",
  "startDate": "2024-12-15T19:00:00",
  "endDate": "2024-12-15T23:00:00",
  "totalSeats": 500,
  "venueId": 1,
  "eventType": "Concert"
}

1. EventsApi.CreateEventAsync() receives the request
   ↓
2. Maps CreateEventRequest → CreateEventCommand
   ↓
3. services.Mediator.Send(command)
   ↓
4. MediatR Pipeline starts:
   
   a. ValidatorBehavior checks if:
      - EventName is not empty
      - StartDate is in future
      - EndDate > StartDate
      - TotalSeats > 0
      ✓ All pass
   
   b. LoggingBehavior logs:
      "Creating event: Concert 2024"
   
   c. TransactionBehavior:
      - Begins DB transaction
   
   d. CreateEventCommandHandler.Handle() executes:
      - Fetch Venue from repository
      - Create Event aggregate via Event.Create()
      - Aggregate validates business rules
      - Add Event to repository
      - Repository adds to DbContext
   
   e. TransactionBehavior:
      - Calls SaveEntitiesAsync()
      - Domain events published to event bus
      - Transaction committed
   
   f. LoggingBehavior logs:
      "Event created successfully. EventId: 123"

5. EventsApi returns:
   201 Created
   Location: /api/v1/events/123
   Body: { "eventId": 123 }

6. Client receives response
```

---

## Testing Strategy

```
UNIT TESTS (Event.Domain.Tests)
├─ Event aggregate tests
│  ├─ Can create event with valid data
│  ├─ Cannot create event with invalid dates
│  ├─ Can book ticket
│  └─ Cannot book when event cancelled
└─ Value Object tests
   ├─ Address equality
   └─ Price comparisons

INTEGRATION TESTS (Event.Api.IntegrationTests)
├─ Database tests
│  ├─ Can save and retrieve event
│  └─ Repository operations
└─ Handler tests
   ├─ CreateEventCommandHandler (with real DB)
   └─ GetEventByIdQueryHandler

API TESTS (Event.Api.FunctionalTests)
├─ POST /api/v1/events → 201 Created
├─ GET /api/v1/events/{id} → 200 OK
├─ PUT /api/v1/events/{id} → 204 NoContent
├─ Invalid input → 400 BadRequest
└─ Not found → 404 NotFound
```

---

This is the complete flow of how clean architecture works in your event ticketing system!
