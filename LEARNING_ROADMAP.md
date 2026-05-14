# Clean Architecture Learning Roadmap for Event Ticketing System

## Week 1-2: Foundations

### Day 1-2: Understand The Four Layers
**Focus**: Know what goes where

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 1 & 2

**Tasks**:
1. Draw the four layers on paper
2. List what belongs in each layer
3. Remember: Inner layers don't know about outer layers

**Key Concept**: 
- Domain = business rules only
- Application = how to use business rules
- Infrastructure = technical details
- Presentation = HTTP stuff

**Questions to ask yourself**:
- Where would database code go? (Infrastructure ✓)
- Where would HTTP code go? (Presentation ✓)
- Where would business rules go? (Domain ✓)
- Where would command handlers go? (Application ✓)

---

### Day 3-4: Understand Domain-Driven Design
**Focus**: Aggregates and Value Objects

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 2

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 3

**Tasks**:
1. Draw your Event aggregate
   ```
   Event (Root)
   ├── Tickets (Children)
   ├── Venue (Value Object)
   └── EventStatus (Enumeration)
   ```

2. Identify value objects in your domain:
   - Address? Price? EventDate? Location?

3. List business rules:
   - Can't book ticket if event is cancelled
   - Can't create event with past dates
   - Total booked seats ≤ total seats

**Key Concept**:
- Aggregate = a cluster that must stay consistent together
- Only Aggregate Root can be accessed from outside
- All changes go through methods on the root

**Example in your system**:
```csharp
// ❌ WRONG: Direct access
event.Tickets.Add(newTicket);

// ✅ CORRECT: Through aggregate
event.BookTicket(ticketNumber, customerId);
```

---

### Day 5-7: Understand CQRS Pattern
**Focus**: Commands vs Queries

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 3

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 4

**Tasks**:
1. List all commands (write operations) for events:
   - CreateEvent
   - BookTicket
   - CancelEvent
   - UpdateEvent

2. List all queries (read operations) for events:
   - GetEventById
   - ListAllEvents
   - GetUpcomingEvents
   - GetEventTickets

3. Understand the difference:
   - Commands change state → need validation, transactions
   - Queries just read → no transactions needed, optimized

**Key Concept**:
- One side optimized for writing (consistency)
- Other side optimized for reading (performance)
- They use different models

**Example**:
```
Commands use: Event aggregate (with all rules)
Queries use: EventDto (optimized projection)
```

---

## Week 3-4: Core Patterns

### Day 8-9: Repository Pattern
**Focus**: Hide database details

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 5

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 7

**Tasks**:
1. Define repository interfaces for your domain:
   ```csharp
   IEventRepository
   IVenueRepository
   ITicketRepository
   ```

2. Plan repository methods:
   ```csharp
   Add(Event @event)
   GetAsync(int eventId)
   Update(Event @event)
   DeleteAsync(int eventId)
   ```

3. Understand: Repository is an abstraction!
   - Domain defines the interface
   - Infrastructure implements it
   - Handler never sees the implementation

**Key Concept**:
- Repository = abstraction over data access
- Handler knows "IEventRepository"
- Handler doesn't care if using EF Core, Dapper, or SQL
- Can swap implementations without changing handlers

---

### Day 10-11: Dependency Injection
**Focus**: Don't create, receive!

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 6

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 5

**Tasks**:
1. Identify all dependencies:
   - Logger
   - Repository
   - Validator
   - Mapper

2. Practice writing constructors:
   ```csharp
   public CreateEventCommandHandler(
       IEventRepository repo,
       ILogger logger)
   {
       _repo = repo;
       _logger = logger;
   }
   ```

3. Don't do this:
   ```csharp
   // ❌ WRONG
   var repo = new EventRepository();
   var logger = new Logger();
   ```

**Key Concept**:
- Dependencies injected via constructor
- Container creates and wires them
- Easy to test (inject mocks)
- Easy to change implementations

---

### Day 12-14: Mediator Pattern
**Focus**: Decoupling and pipeline

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 4

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 2

**Tasks**:
1. Understand the pipeline:
   ```
   ValidatorBehavior
   LoggingBehavior  
   TransactionBehavior
   [Your Handler]
   TransactionBehavior (commit)
   LoggingBehavior
   ```

2. Identify your behaviors:
   - Validation (check input)
   - Logging (log operations)
   - Transaction (begin/commit)
   - Error handling (catch exceptions)

3. Draw the flow for CreateEvent command

**Key Concept**:
- Handler is SIMPLE (just business logic)
- Concerns handled by behaviors (validation, logging, etc.)
- Behaviors execute in pipeline order
- Easy to add/remove behaviors

---

## Week 5-6: Implementation Patterns

### Day 15-16: Minimal APIs
**Focus**: No Controllers!

**Read**: [LEARNING_GUIDE.md](LEARNING_GUIDE.md) - Section 7

**Visualize**: [ARCHITECTURE_DIAGRAMS.md](ARCHITECTURE_DIAGRAMS.md) - Diagram 6

**Practical**: [PRACTICAL_EXAMPLES.md](PRACTICAL_EXAMPLES.md) - Example 4

**Tasks**:
1. Design your endpoints:
   ```
   POST   /api/v1/events          → CreateEvent
   GET    /api/v1/events          → ListEvents
   GET    /api/v1/events/{id}     → GetEvent
   PUT    /api/v1/events/{id}     → UpdateEvent
   DELETE /api/v1/events/{id}     → CancelEvent
   
   POST   /api/v1/events/{id}/book-ticket
   GET    /api/v1/events/{id}/available-seats
   ```

2. Create EventServices class:
   ```csharp
   public class EventServices(
       IMediator mediator,
       IEventQueries queries,
       ILogger logger)
   {
       // Dependencies bundled
   }
   ```

3. Create static handler methods

**Key Concept**:
- Routes defined in one place (MapEventsApiV1)
- Handler methods are static (lightweight)
- Dependencies via [AsParameters]
- No controller class needed

---

### Day 17-18: Complete Command Flow
**Focus**: End-to-end understanding

**Read**: [PRACTICAL_EXAMPLES.md](PRACTICAL_EXAMPLES.md) - Example 1

**Tasks**:
1. Trace CreateEvent flow:
   ```
   HTTP Request
   → EventsApi endpoint
   → CreateEventCommand
   → Validator
   → CreateEventCommandHandler
   → Event aggregate
   → Repository
   → Database
   → Response
   ```

2. Write it down step by step
3. Identify each layer involved

**Key Concept**:
- Request flows from outside → inside
- Response flows from inside → outside
- Each layer has specific responsibility

---

### Day 19-20: Complete Query Flow
**Focus**: Reading data efficiently

**Read**: [PRACTICAL_EXAMPLES.md](PRACTICAL_EXAMPLES.md) - Example 2

**Tasks**:
1. Trace GetEvent flow:
   ```
   HTTP Request
   → EventsApi endpoint
   → GetEventByIdQuery
   → EventQueries (optimized read)
   → EventDto (projection)
   → Response
   ```

2. Notice differences from commands:
   - No validation behavior
   - No transaction
   - Optimized query
   - Direct DTO return

**Key Concept**:
- Queries are simpler than commands
- No need for full aggregate
- Use projections for performance

---

## Week 7-8: Your Event Ticketing System

### Day 21-22: Domain Model
**Focus**: Core business logic

**Read**: [PRACTICAL_EXAMPLES.md](PRACTICAL_EXAMPLES.md) - Example 3

**Tasks**:
1. Create Event.Domain/SeedWork/:
   ```
   Entity.cs
   ValueObject.cs
   IAggregateRoot.cs
   IRepository.cs
   IUnitOfWork.cs
   ```

2. Create Event aggregate:
   ```
   Event.Domain/AggregatesModel/EventAggregate/
   ├── Event.cs (aggregate root)
   ├── Ticket.cs (child entity)
   └── EventStatus.cs (enumeration)
   ```

3. Add business logic:
   ```csharp
   public void BookTicket(int ticketNumber, string customerId)
   {
       // Validate rules
       // Update state
       // Publish events
   }
   ```

4. Test domain logic (unit tests, no DB)

---

### Day 23-24: Application Layer
**Focus**: Commands and Queries

**Tasks**:
1. Create Event.Api/Application/:
   ```
   Application/
   ├── Commands/
   │   ├── CreateEventCommand.cs
   │   ├── CreateEventCommandHandler.cs
   │   └── CreateEventCommandValidator.cs
   ├── Queries/
   │   ├── GetEventByIdQuery.cs
   │   ├── GetEventByIdQueryHandler.cs
   │   └── EventDto.cs
   └── Behaviors/
       ├── LoggingBehavior.cs
       └── ValidatorBehavior.cs
   ```

2. Implement handlers
3. Write validators
4. Create DTOs

---

### Day 25-26: Infrastructure Layer
**Focus**: Data Access

**Tasks**:
1. Create Event.Infrastructure/:
   ```
   Infrastructure/
   ├── Data/
   │   ├── EventDbContext.cs
   │   ├── EventContextSeed.cs
   │   └── EntityConfigurations/
   ├── Repositories/
   │   └── EventRepository.cs
   └── Queries/
       └── EventQueries.cs
   ```

2. Implement repositories
3. Configure EF Core mappings
4. Create migrations

---

### Day 27-28: API Endpoints
**Focus**: Minimal APIs

**Tasks**:
1. Create Event.Api/Apis/:
   ```
   Apis/
   ├── EventsApi.cs
   └── EventServices.cs
   ```

2. Map all endpoints
3. Wire up DI in Program.cs
4. Test endpoints with Postman/REST Client

---

## Week 9+: Testing & Refinement

### Unit Tests
```
Event.Domain.Tests/
├── AggregatesModel/
│   └── EventAggregateTests.cs
└── SeedWork/
    └── ValueObjectTests.cs
```

Test: Business logic without database

### Integration Tests
```
Event.Api.IntegrationTests/
├── Repositories/
│   └── EventRepositoryTests.cs
└── Application/
    └── CreateEventCommandHandlerTests.cs
```

Test: Commands and queries with real DB

### API Tests
```
Event.Api.FunctionalTests/
└── EventsApiTests.cs
```

Test: Full HTTP requests

---

## Self-Assessment Checklist

### After Week 1-2:
- [ ] I can explain the four layers
- [ ] I know what goes in each layer
- [ ] I understand aggregates
- [ ] I understand value objects

### After Week 3-4:
- [ ] I can design a repository
- [ ] I understand DI
- [ ] I can write constructors with dependencies
- [ ] I understand the mediator pattern

### After Week 5-6:
- [ ] I know how to create minimal APIs
- [ ] I understand commands and queries
- [ ] I can trace a complete request flow
- [ ] I can trace a complete query flow

### After Week 7-8:
- [ ] I've created the domain layer
- [ ] I've created the application layer
- [ ] I've created the infrastructure layer
- [ ] I've created the API endpoints
- [ ] All endpoints work

### After Week 9+:
- [ ] I have unit tests for domain
- [ ] I have integration tests for handlers
- [ ] I have API tests for endpoints
- [ ] I can refactor without breaking tests

---

## Quick Reference Commands

```bash
# Create project structure
dotnet new classlib -n Event.Domain
dotnet new classlib -n Event.Infrastructure
dotnet new webapi -n Event.Api

# Add NuGet packages
dotnet add Event.Api package MediatR
dotnet add Event.Api package FluentValidation
dotnet add Event.Api package AutoMapper
dotnet add Event.Api package Microsoft.EntityFrameworkCore
dotnet add Event.Api package Npgsql.EntityFrameworkCore.PostgreSQL

# Run migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run tests
dotnet test
```

---

## Resources

**Books to Read**:
- "Clean Architecture" by Robert C. Martin
- "Domain-Driven Design" by Eric Evans
- "Building Microservices" by Sam Newman

**Online Resources**:
- [MediatR GitHub](https://github.com/jbogard/MediatR)
- [FluentValidation GitHub](https://github.com/FluentValidation/FluentValidation)
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)

**Your Reference**:
- See the eShop Ordering module code
- Read through Practical Examples
- Study the diagrams

---

## Common Questions

**Q: Why have both Command and Query?**
A: Commands change state (need validation, transactions). Queries just read (optimized, fast).

**Q: Why have Repository if using EF Core?**
A: Abstraction! Change database without changing code.

**Q: Why use Mediator?**
A: Decoupling. Handler knows nothing about validation, logging, transactions.

**Q: Why Minimal APIs instead of Controllers?**
A: Lighter, cleaner, all routes visible in one place.

**Q: Can I test without database?**
A: Yes! Domain tests need no DB. Application tests use mock repositories.

---

**Start with the basics. Don't rush. Understand each concept before moving to the next.**

Good luck! 🚀
