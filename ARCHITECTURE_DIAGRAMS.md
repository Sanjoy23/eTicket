# Clean Architecture - Visual Diagrams

## Diagram 1: The Four Layers

```
┌─────────────────────────────────────────────────────────┐
│           PRESENTATION LAYER (API)                      │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Minimal APIs (No Controllers)                   │   │
│  │  - EventsApi.MapGet("/", GetEventAsync)         │   │
│  │  - EventsApi.MapPost("/", CreateEventAsync)     │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         ↓ (Uses)
┌─────────────────────────────────────────────────────────┐
│        APPLICATION LAYER (Event.Api/Application)        │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Commands: CreateEventCommand                   │   │
│  │  Queries: GetEventByIdQuery                      │   │
│  │  Handlers: CreateEventCommandHandler             │   │
│  │  Behaviors: LoggingBehavior, ValidatorBehavior  │   │
│  │  DTOs: EventDto, EventSummaryDto                │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         ↓ (Uses)
┌─────────────────────────────────────────────────────────┐
│         DOMAIN LAYER (Event.Domain)                     │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Aggregates: Event, Ticket, Venue               │   │
│  │  Value Objects: Address, EventDate, Price       │   │
│  │  Domain Events: EventCreated, TicketBooked      │   │
│  │  Seedwork: Entity, ValueObject, IAggregateRoot  │   │
│  │  Interfaces: IRepository, IUnitOfWork            │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                         ↓ (Implements)
┌─────────────────────────────────────────────────────────┐
│    INFRASTRUCTURE LAYER (Event.Infrastructure)          │
│  ┌──────────────────────────────────────────────────┐   │
│  │  Repositories: EventRepository                   │   │
│  │  DbContext: EventDbContext                       │   │
│  │  Entity Mappings: EventEntityConfiguration       │   │
│  │  Migrations: Database schema changes             │   │
│  └──────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
```

## Diagram 2: Request/Response Flow

```
┌─────────┐
│  Client │
│(Browser)│
└────┬────┘
     │ POST /api/events { CreateEventCommand }
     ↓
┌─────────────────────────────────────┐
│   EventsApi.CreateEventAsync()      │
│   - Receives command                │
│   - Gets EventServices via [AsParams]│
└────────┬────────────────────────────┘
         │ Mediator.Send(command)
         ↓
┌─────────────────────────────────────┐
│   MediatR Pipeline (Mediator)       │
│   1. ValidatorBehavior              │
│   2. LoggingBehavior (start)        │
│   3. TransactionBehavior (begin)    │
│   4. [Handler executes]             │
│   5. TransactionBehavior (commit)   │
│   6. LoggingBehavior (end)          │
└────────┬────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────┐
│  CreateEventCommandHandler.Handle()│
│  1. Create Event aggregate          │
│  2. Validate business rules         │
│  3. Add to repository               │
│  4. SaveEntitiesAsync()             │
└────────┬────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────┐
│  EventRepository                    │
│  - implements IEventRepository      │
│  - wraps EventDbContext             │
│  - translates to EF Core calls      │
└────────┬────────────────────────────┘
         │
         ↓
┌─────────────────────────────────────┐
│  EventDbContext (Entity Framework)  │
│  - saves Event to database          │
└────────┬────────────────────────────┘
         │
         ↓
┌──────────────┐
│   Database   │
│  (PostgreSQL)│
└────────┬─────┘
         │
         ↓ Response flows back up
┌─────────┐
│  Client │
│ 200 OK  │
│EventId: │
│  12345  │
└─────────┘
```

## Diagram 3: Aggregate Structure

```
Event Aggregate
┌──────────────────────────────────────────────┐
│                 EVENT (Root)                 │
│                                              │
│  Properties:                                 │
│  - EventId (unique identifier)               │
│  - EventName                                 │
│  - StartDate                                 │
│  - TotalSeats                                │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │    TICKETS (Child Entities)          │   │
│  │  ┌────────────────────────────────┐  │   │
│  │  │ Ticket 1                       │  │   │
│  │  │ - TicketNumber                 │  │   │
│  │  │ - Status: Available/Booked     │  │   │
│  │  └────────────────────────────────┘  │   │
│  │  ┌────────────────────────────────┐  │   │
│  │  │ Ticket 2                       │  │   │
│  │  │ - TicketNumber                 │  │   │
│  │  │ - Status: Available/Booked     │  │   │
│  │  └────────────────────────────────┘  │   │
│  └──────────────────────────────────────┘   │
│                                              │
│  ┌──────────────────────────────────────┐   │
│  │    VENUE (Value Object)              │   │
│  │  - VenueName                         │   │
│  │  - Address                           │   │
│  │  - Capacity                          │   │
│  └──────────────────────────────────────┘   │
│                                              │
│  Business Logic (Methods):                   │
│  + BookTicket(ticketId, customerId)         │
│  + CancelEvent()                            │
│  + UpdateEventStatus()                      │
│                                              │
└──────────────────────────────────────────────┘

⚠️ RULES:
- Only access Event from OUTSIDE aggregate
- Tickets accessed ONLY through Event methods
- Venue cannot be modified directly
- All changes maintain business rules
```

## Diagram 4: CQRS Pattern

```
COMMANDS (Write Operations)          QUERIES (Read Operations)
┌────────────────────────────────┐  ┌─────────────────────────┐
│  CreateEventCommand            │  │ GetEventByIdQuery       │
│  - EventName                   │  │ - EventId               │
│  - StartDate                   │  └──────────┬──────────────┘
│  - TotalSeats                  │             │
└──────────┬─────────────────────┘             ↓
           │                          ┌─────────────────────────┐
           ↓                          │ EventQueries            │
┌────────────────────────────────┐   │ (Optimized for reading) │
│  CreateEventCommandHandler     │   │ - Join tables as needed │
│  (Executes business logic)     │   │ - No transactions       │
│  - Create Event aggregate      │   │ - Returns DTO directly  │
│  - Validate business rules     │   └──────────┬──────────────┘
│  - Save to repository          │             │
│  - Commit transaction          │             ↓
└──────────┬─────────────────────┘  ┌─────────────────────────┐
           │                         │ EventDto                │
           ↓                         │ - EventName             │
┌────────────────────────────────┐  │ - StartDate             │
│ Event Aggregate (Domain)       │  │ - AvailableSeats        │
│ - Business rules enforced      │  │ - Performers            │
│ - State changed                │  └─────────────────────────┘
│ - Events triggered             │
└────────────────────────────────┘

KEY DIFFERENCE:
- Commands: Change state, trigger events, use transactions
- Queries: Just read data, no transactions, optimized reads
```

## Diagram 5: Dependency Injection

```
┌─────────────────────────────────────────────────────────┐
│              Program.cs (Dependency Setup)              │
│                                                         │
│  builder.Services.AddScoped<IEventRepository,          │
│                               EventRepository>();     │
│  builder.Services.AddScoped<IEventQueries,             │
│                               EventQueries>();        │
│  builder.Services.AddMediatR(...);                     │
│  builder.Services.AddLogging();                        │
│                                                         │
└──────────┬──────────────────────────────────────────────┘
           │
           ↓
┌──────────────────────────────────────────────────────────┐
│        DI Container (Dependency Resolver)               │
│                                                          │
│  When CreateEventCommandHandler is needed:              │
│  1. Check constructor: needs IEventRepository           │
│  2. Look up: IEventRepository → EventRepository         │
│  3. Check constructor: needs ILogger                    │
│  4. Look up: ILogger → LoggerFactory                    │
│  5. Create EventRepository with ILogger                 │
│  6. Create CreateEventCommandHandler with repos+logger  │
│  7. Return ready handler to caller                      │
│                                                          │
└──────────┬──────────────────────────────────────────────┘
           │
           ↓
┌──────────────────────────────────────────────────────────┐
│              CreateEventCommandHandler                   │
│  public Handler(IEventRepository repo,                  │
│                 ILogger logger) { ... }                │
│                                                          │
│  Ready to use with all dependencies!                    │
└──────────────────────────────────────────────────────────┘

BENEFIT: Handler doesn't know HOW logger is created,
or what database EventRepository uses. It just uses
the abstractions.
```

## Diagram 6: Minimal APIs vs Traditional Controllers

```
TRADITIONAL CONTROLLERS:
┌────────────────────────────────────────────┐
│  EventsController : ControllerBase         │
│  {                                         │
│    [HttpGet("{id}")]                       │
│    public async Task<IActionResult>        │
│    GetEvent(int id) { ... }                │
│                                            │
│    [HttpPost]                              │
│    public async Task<IActionResult>        │
│    CreateEvent([FromBody] ...) { ... }     │
│                                            │
│    [HttpPut("{id}")]                       │
│    public async Task<IActionResult>        │
│    UpdateEvent(int id, ...) { ... }        │
│  }                                         │
└────────────────────────────────────────────┘
❌ Issues: Boilerplate code, attributes scattered

MINIMAL APIS:
┌────────────────────────────────────────────┐
│  EventsApi (static class)                  │
│  {                                         │
│    public static void MapEventsApiV1(      │
│      this IEndpointRouteBuilder app)       │
│    {                                       │
│      var group = app.MapGroup("api/events")│
│      group.MapGet("{id}", GetEventAsync);  │
│      group.MapPost("/", CreateEventAsync); │
│      group.MapPut("{id}", UpdateAsync);    │
│    }                                       │
│                                            │
│    private static async Task<EventDto>    │
│    GetEventAsync(int id,                   │
│      [AsParameters] EventServices svc)     │
│    { ... }                                 │
│  }                                         │
└────────────────────────────────────────────┘
✅ Benefits: All routes visible, clean code, no instantiation
```

## Diagram 7: Repository Pattern Benefit

```
WITHOUT REPOSITORY:
┌──────────────────────────────┐
│  CreateEventCommandHandler   │
│  - depends on EF Core        │
│  - knows DbContext           │
│  - knows SQL details         │
└────────┬─────────────────────┘
         │
         ↓
┌──────────────────────────────┐
│  EventDbContext              │
│  _context.Events.Add(event)  │
│  _context.SaveChangesAsync() │
└────────┬─────────────────────┘
         │
    If you change to:
    - Different ORM (Dapper)
    - Different Database (MySQL)
    - Different pattern (GraphQL)
    
    ❌ Handler code breaks!

WITH REPOSITORY:
┌──────────────────────────────┐
│  CreateEventCommandHandler   │
│  - depends on abstraction    │
│  - uses IEventRepository     │
│  - doesn't know DB details   │
└────────┬─────────────────────┘
         │
         ↓
┌──────────────────────────────┐
│  IEventRepository            │  (Abstraction)
│  - Add(Event)                │
│  - GetAsync(id)              │
└────────┬─────────────────────┘
         │
         ├─→ ┌──────────────────────────┐
         │   │ EventRepository (EF Core)│
         │   └──────────────────────────┘
         │
         ├─→ ┌──────────────────────────┐
         │   │ EventRepositoryMock      │  (For Testing)
         │   └──────────────────────────┘
         │
         └─→ ┌──────────────────────────┐
             │ EventRepositoryDapper    │  (Alternative)
             └──────────────────────────┘
    
    ✅ Handler stays the same!
    Easy to test, swap implementations!
```

---

## Summary Table: Which Layer Gets What?

| Concern | Domain | Application | Infrastructure | API |
|---------|--------|-------------|-----------------|-----|
| Business Rules | ✅ YES | - | - | - |
| Database Access | - | - | ✅ YES | - |
| HTTP Concerns | - | - | - | ✅ YES |
| Entity Framework | - | - | ✅ YES | - |
| Commands/Queries | - | ✅ YES | - | - |
| Controllers | - | - | - | ❌ NO (Use Minimal APIs) |
| Repositories (Impl.) | - | - | ✅ YES | - |
| Repositories (Interface) | ✅ YES | - | - | - |
| Aggregates | ✅ YES | - | - | - |
| DTOs | - | ✅ YES | - | - |

**Golden Rule**: Inner layers must NOT know about outer layers!
