# Clean Architecture & CQRS Learning Guide for Event Ticketing System

## 1. THE FOUR LAYERS OF CLEAN ARCHITECTURE

### Layer 1: Domain Layer (Event.Domain)
**Purpose**: Pure business logic - NO external dependencies

**What goes here:**
- Aggregates (main domain objects)
- Value Objects (immutable objects)
- Domain Events (things that happened in the business)
- Exceptions (business rule violations)
- Seedwork (base classes)

**Key Rule**: Domain layer knows NOTHING about:
- Databases
- HTTP requests
- External services
- UI frameworks

### Layer 2: Application Layer (Event.Api/Application)
**Purpose**: Use cases & orchestration - HOW to use domain objects

**What goes here:**
- Commands (actions that change state)
- Queries (actions that read state)
- Command/Query Handlers (business logic orchestration)
- Behaviors (cross-cutting concerns)
- DTOs (data transfer objects for responses)
- Application Events (for external systems)

### Layer 3: Infrastructure Layer (Event.Infrastructure)
**Purpose**: Technical implementation - HOW to implement persistence

**What goes here:**
- DbContext & Entity Framework Core
- Repository implementations
- Database migrations
- External service integrations
- Configuration for data access

**Key principle**: Implements interfaces DEFINED in Domain

### Layer 4: Presentation Layer (API Endpoints)
**Purpose**: Handle HTTP requests/responses - entry point

**Modern approach**: NO Controllers, use Minimal APIs instead

---

## 2. DOMAIN-DRIVEN DESIGN (DDD) CONCEPTS

### What is an Aggregate?
A cluster of domain objects that should be treated as a single unit.

Example - Event Aggregate:
```
Event (Aggregate Root)
├── Tickets (Child Entities)
│   ├── Ticket 1
│   ├── Ticket 2
│   └── Ticket 3
└── EventStatus (Value Object)
```

**Key rules:**
- Only the Aggregate Root can be accessed from outside
- Changes to child entities go through the root
- All changes maintain business rules

### What is a Value Object?
An object defined by its attributes, not identity.

Examples:
- Address (street, city, zip)
- Money (amount, currency)
- EventDate (start time, end time)
- Price (amount, currency)

**Characteristics:**
- Immutable (can't change after creation)
- No database ID
- Compared by value, not identity

### What is a Domain Event?
Something important happened in your business domain.

Examples:
- EventCreated
- TicketsBooked
- EventCancelled
- PaymentProcessed

---

## 3. CQRS PATTERN (Command Query Responsibility Segregation)

**Concept**: Separate READ operations from WRITE operations

### Commands (Write Operations - Change State)

Commands describe intent:
```csharp
public class CreateEventCommand : IRequest<int>
{
    public string EventName { get; set; }
    public DateTime StartDate { get; set; }
    public int TotalSeats { get; set; }
}
```

Handlers execute the command:
```csharp
public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    private readonly IEventRepository _repository;
    
    public async Task<int> Handle(CreateEventCommand request, CancellationToken cancellationToken)
    {
        var @event = new Event(request.EventName, request.StartDate, ...);
        _repository.Add(@event);
        await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return @event.EventId;
    }
}
```

### Queries (Read Operations - Don't Change State)

Queries describe what we want:
```csharp
public class GetEventByIdQuery : IRequest<EventDto>
{
    public int EventId { get; set; }
}
```

Handlers fetch and return data:
```csharp
public class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, EventDto>
{
    private readonly IEventQueries _queries;
    
    public async Task<EventDto> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
    {
        return await _queries.GetEventByIdAsync(request.EventId);
    }
}
```

**Benefits:**
- Read models optimized for queries
- Write models optimized for consistency
- Easy to scale reads separately from writes

---

## 4. MEDIATOR PATTERN

**What it does**: Decouples components by having them communicate through a mediator.

**Without Mediator** (tightly coupled):
```csharp
public class Handler
{
    public void Handle(Command command)
    {
        var validator = new Validator();
        validator.Validate(command);
        
        var logger = new Logger();
        logger.Log("Processing");
        
        // Business logic here
    }
}
```

**With Mediator** (decoupled):
```csharp
// Handler is SIMPLE - just business logic
public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, int>
{
    private readonly IEventRepository _repository;
    
    public async Task<int> Handle(CreateEventCommand request, CancellationToken ct)
    {
        var @event = new Event(...);
        _repository.Add(@event);
        return @event.EventId;
    }
}

// Concerns handled by BEHAVIORS
public class LoggingBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        _logger.Log($"Handling {typeof(TRequest).Name}");
        var response = await next();
        _logger.Log($"Completed {typeof(TRequest).Name}");
        return response;
    }
}
```

**Pipeline Flow:**
```
Request
   ↓
ValidatorBehavior
   ↓
LoggingBehavior
   ↓
TransactionBehavior
   ↓
[ACTUAL HANDLER]
   ↓
TransactionBehavior (commit)
   ↓
LoggingBehavior
   ↓
Response
```

---

## 5. REPOSITORY PATTERN

**What it does**: Hide data access complexity behind a simple interface

**Without Repository** (tightly coupled):
```csharp
public class Handler
{
    private readonly EventDbContext _context;
    
    public async Task<int> Handle(Command request)
    {
        var @event = new Event(...);
        _context.Events.Add(@event);  // ❌ Tightly coupled to EF Core
        await _context.SaveChangesAsync();
        return @event.EventId;
    }
}
```

**With Repository** (abstraction):
```csharp
public class Handler
{
    private readonly IEventRepository _repository;  // ✅ Abstraction
    
    public async Task<int> Handle(Command request)
    {
        var @event = new Event(...);
        _repository.Add(@event);
        await _repository.UnitOfWork.SaveEntitiesAsync();
        return @event.EventId;
    }
}

public class EventRepository : IEventRepository
{
    private readonly EventDbContext _context;
    
    public void Add(Event @event)
    {
        _context.Events.Add(@event);
    }
}
```

**Benefits:**
- Change database without changing handlers
- Easy to mock for testing
- Single responsibility

---

## 6. DEPENDENCY INJECTION (DI)

**What it does**: Provide dependencies instead of creating them

**Without DI** (tightly coupled):
```csharp
public class Handler
{
    public void Handle(Command request)
    {
        var repository = new EventRepository();  // Creates its own
        var logger = new Logger();               // Creates its own
        var validator = new Validator();         // Creates its own
    }
}
```

**With DI** (decoupled):
```csharp
public class Handler
{
    private readonly IEventRepository _repository;
    private readonly ILogger _logger;
    
    public Handler(IEventRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public void Handle(Command request)
    {
        _repository.Add(...);
        _logger.Log(...);
    }
}

// Setup in Program.cs
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddLogging();
```

**Benefits:**
- Easy to test (inject mocks)
- Loose coupling
- Single responsibility

---

## 7. MINIMAL APIs (No Controllers)

**Traditional Controllers:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        // ...
    }
}
```

**Minimal APIs:**
```csharp
public static class EventsApi
{
    public static void MapEventsApiV1(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/events");
        group.MapGet("{id}", GetEventAsync);
        group.MapPost("/", CreateEventAsync);
    }
    
    private static async Task<EventDto> GetEventAsync(
        int id,
        [AsParameters] EventServices services)
    {
        return await services.Queries.GetEventByIdAsync(id);
    }
}

// Bundles dependencies
public class EventServices(
    IMediator mediator,
    IEventQueries queries,
    ILogger<EventServices> logger)
{
    public IMediator Mediator { get; } = mediator;
    public IEventQueries Queries { get; } = queries;
}
```

**Benefits:**
- No controller class overhead
- All routes visible in one place
- Lightweight and fast

---

## 8. DATA FLOW EXAMPLES

### Example 1: Creating an Event

```
POST /api/events
  { "eventName": "Concert 2024", "startDate": "2024-12-15", ... }
        ↓
MapGroup routes to CreateEventAsync
        ↓
CreateEventAsync receives CreateEventCommand + EventServices
        ↓
await services.Mediator.Send(command)
        ↓
ValidatorBehavior checks input
        ↓
LoggingBehavior logs "Creating event..."
        ↓
TransactionBehavior begins DB transaction
        ↓
CreateEventCommandHandler executes:
  - Creates Event aggregate
  - Sets business properties
  - Validates business rules
  - Adds to repository
  - Saves to database
        ↓
TransactionBehavior commits transaction
        ↓
LoggingBehavior logs "Event created"
        ↓
Response: 200 OK with eventId
```

### Example 2: Getting Event Details

```
GET /api/events/123
        ↓
MapGroup routes to GetEventAsync(123)
        ↓
GetEventAsync calls services.Queries.GetEventByIdAsync(123)
        ↓
EventQueries fetches from database
        ↓
Maps to EventDto
        ↓
Response: 200 OK with EventDto
```

---

## 9. KEY PRINCIPLES TO REMEMBER

**Single Responsibility Principle:**
- Domain: Business rules only
- Application: Use cases and orchestration
- Infrastructure: Data access details
- API: HTTP concerns

**Dependency Rule:**
- Domain ← Application ← Infrastructure ← API
- Inner layers never know about outer layers

**Interfaces over Implementations:**
- Domain: Defines interfaces
- Infrastructure: Implements them

**Abstraction over Details:**
- Handler doesn't care about database type
- Handler doesn't care about HTTP framework

**Testability:**
- Unit test domain logic: no database
- Integration test application: mock repository
- API test: full system

---

## 10. YOUR EVENT TICKETING SYSTEM STRUCTURE (Future)

```
Event.Domain/
  ├── AggregatesModel/
  │   ├── EventAggregate/
  │   │   ├── Event.cs
  │   │   ├── Ticket.cs
  │   │   └── Seat.cs
  │   └── VenueAggregate/
  │       └── Venue.cs
  ├── SeedWork/
  │   ├── Entity.cs
  │   ├── ValueObject.cs
  │   └── IAggregateRoot.cs
  └── Events/
      └── EventCreatedDomainEvent.cs

Event.Api/
  ├── Application/
  │   ├── Commands/
  │   │   ├── CreateEventCommand.cs
  │   │   └── CreateEventCommandHandler.cs
  │   ├── Queries/
  │   │   ├── GetEventByIdQuery.cs
  │   │   └── EventDto.cs
  │   └── Behaviors/
  │       ├── LoggingBehavior.cs
  │       └── ValidatorBehavior.cs
  ├── Apis/
  │   ├── EventsApi.cs
  │   └── EventServices.cs
  └── Program.cs

Event.Infrastructure/
  ├── Data/
  │   ├── EventDbContext.cs
  │   └── EntityConfigurations/
  └── Repositories/
      ├── EventRepository.cs
      └── Queries/
          └── EventQueries.cs
```

---

## Learning Path:

1. Understand Aggregate Design
2. Learn DDD Value Objects
3. Study Command Pattern
4. Understand Repository Pattern
5. Learn CQRS
6. Study Mediator Pattern
7. Learn Minimal APIs
8. Master Dependency Injection

**Once comfortable with these, you'll be ready to refactor!**
