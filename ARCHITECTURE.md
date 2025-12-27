# Backend Architecture & Code Conventions

This document defines the coding standards and file structure for the School-Hub backend.

---

## Table of Contents

1. [Folder Structure](#folder-structure)
2. [Endpoints](#endpoints)
3. [Queries](#queries)
4. [Commands](#commands)
5. [Orchestrators](#orchestrators)
6. [DTOs (Request & Response)](#dtos-request--response)
7. [Repositories](#repositories)
8. [Examples](#examples)

---

## Folder Structure

```
Back-End/
├── EndPoints/
│   ├── {Role}/
│   │   └── {Feature}/
│   │       └── {FeatureName}EndPoint.cs
│
├── Application/
│   └── Features/
│       └── {Role}/
│           └── {Feature}/
│               ├── Commands/
│               │   └── {ActionName}Command.cs
│               ├── Queries/
│               │   └── {ActionName}Query.cs
│               └── DTOs/
│                   └── {Feature}Dtos.cs
│
├── Domain/
│   └── Entities/
│       └── {Category}/
│           └── {EntityName}.cs
│
└── Infrastructure/
    └── Persistence/
        ├── DbContexts/
        └── Repositories/
```

### Naming Conventions

- **Folders:** PascalCase (e.g., `User`, `Login`, `PortfolioBook`)
- **Files:** PascalCase matching class name (e.g., `UserLoginEndPoint.cs`)
- **Group by Role first:** `Student`, `Teacher`, `Admin`, `User`
- **Then by Feature:** `Login`, `Missions`, `PortfolioBook`, etc.

---

## Endpoints

### Rules

1. **One endpoint per folder** — Each endpoint lives in its own folder under `EndPoints/{Role}/{Feature}/`
2. **Inherits from `EndpointDefinition`** — Override `RegisterEndpoints()` method
3. **Only calls MediatR** — Never inject or call repositories directly
4. **Dispatches to Query, Command, or Orchestrator** — Based on operation type

### Structure

```csharp
namespace API.EndPoints.{Role}.{Feature};

public class {Feature}EndPoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.Map{HttpMethod}("/{Role}/{Feature}",
            async (IMediator mediator, {Request} request, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new {Command/Query}(request), cancellationToken);
                return Response(result);
            })
        .WithTags("{Role}")
        .AddEndpointFilter<JwtEndpointFilter>()
        .Produces<EndPointResponse<{Response}>>();
    }
}
```

---

## Queries

### Rules

1. **One query per file** — Each query in its own file under `Application/Features/{Role}/{Feature}/Queries/`
2. **Read-only operations** — Queries only fetch data, never modify
3. **One repository per query** — Each query handler injects only ONE repository (single responsibility)
4. **Returns `RequestResult<T>`** — Wrap response in standard result type

### Structure

```csharp
namespace API.Application.Features.{Role}.{Feature}.Queries;

public record {Name}Query({Parameters}) : IRequest<RequestResult<{ResponseDto}>>;

public class {Name}QueryHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<{Entity}> repository)  // Only ONE repository
    : RequestHandlerBase<{Name}Query, RequestResult<{ResponseDto}>>(parameters)
{
    public override async Task<RequestResult<{ResponseDto}>> Handle(
        {Name}Query request, 
        CancellationToken cancellationToken)
    {
        // Fetch data from repository
        var entity = await repository.Get(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
            return RequestResult<{ResponseDto}>.Failure(ErrorCode.NotFound, "Not found");

        // Map to DTO and return
        return RequestResult<{ResponseDto}>.Success(new {ResponseDto} { ... });
    }
}
```

---

## Commands

### Rules

1. **One command per file** — Each command in its own file under `Application/Features/{Role}/{Feature}/Commands/`
2. **Write operations** — Commands create, update, or delete data
3. **One repository per command** — Each command handler injects only ONE repository (single responsibility)
4. **Returns `RequestResult<T>`** — Wrap response in standard result type

### Structure

```csharp
namespace API.Application.Features.{Role}.{Feature}.Commands;

public record {Name}Command({RequestDto} Request) : IRequest<RequestResult<{ResponseDto}>>;

public class {Name}CommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<{Entity}> repository)  // Only ONE repository
    : RequestHandlerBase<{Name}Command, RequestResult<{ResponseDto}>>(parameters)
{
    public override async Task<RequestResult<{ResponseDto}>> Handle(
        {Name}Command request, 
        CancellationToken cancellationToken)
    {
        var studentId = _userState.UserID;

        // Validate
        // Create/Update entity
        // Save changes

        repository.Add(entity);
        await repository.SaveChangesAsync(cancellationToken);

        return RequestResult<{ResponseDto}>.Success(new {ResponseDto} { ... });
    }
}
```

---

## Orchestrators

### Rules

1. **Coordinates multiple operations** — When an endpoint needs to call multiple queries/commands
2. **NO repository injection** — Orchestrators NEVER call repositories directly
3. **Only dispatches MediatR requests** — Uses `IMediator` to send queries/commands
4. **Lives in Commands folder** — Named `{Name}Orchestrator.cs`

### Structure

```csharp
namespace API.Application.Features.{Role}.{Feature}.Commands;

public record {Name}Orchestrator({Parameters}) : IRequest<RequestResult<{ResponseDto}>>;

public class {Name}OrchestratorHandler(
    RequestHandlerBaseParameters parameters,
    IMediator mediator)  // Only IMediator, NO repositories
    : RequestHandlerBase<{Name}Orchestrator, RequestResult<{ResponseDto}>>(parameters)
{
    public override async Task<RequestResult<{ResponseDto}>> Handle(
        {Name}Orchestrator request, 
        CancellationToken cancellationToken)
    {
        // Call Query
        var queryResult = await mediator.Send(new SomeQuery(request.Id), cancellationToken);
        if (!queryResult.IsSuccess)
            return RequestResult<{ResponseDto}>.Failure(queryResult.ErrorCode, queryResult.Message);

        // Call Command
        var commandResult = await mediator.Send(new SomeCommand(request.Data), cancellationToken);

        return commandResult;
    }
}
```

---

## DTOs (Request & Response)

### Rules

1. **Separate Request and Response classes** — Even if they look similar
2. **Live in DTOs folder** — Under `Application/Features/{Role}/{Feature}/DTOs/`
3. **Use `[JsonConverter]` for long IDs** — Convert to string for JavaScript compatibility
4. **PascalCase properties** — Match C# conventions

### Structure

```csharp
namespace API.Application.Features.{Role}.{Feature}.DTOs;

// Request DTO (input from client)
public class {Name}Request
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... other properties
}

// Response DTO (output to client)
public class {Name}Response
{
    [JsonConverter(typeof(LongAsStringConverter))]
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    // ... other properties
}
```

---

## Repositories

### Rules

1. **One repository per handler** — Queries and Commands inject only ONE repository
2. **Generic `IRepository<TEntity>`** — Use the generic repository pattern
3. **Never in Orchestrators** — Orchestrators only use `IMediator`

---

## Examples

### Example 1: User Login (Orchestrator)

```
EndPoints/User/Login/UserLoginEndPoint.cs
    ↓ calls via MediatR
Application/Features/User/Login/Commands/UserLoginOrchestrator.cs
    ↓ calls via MediatR (NO repositories)
Application/Features/User/Login/Queries/GetUserByUsernameQuery.cs
Application/Features/User/Login/Commands/GenerateTokenCommand.cs
```

### Example 2: Save Portfolio Profile (Command)

```
EndPoints/Student/PortfolioBook/SavePortfolioProfileEndpoint.cs
    ↓ calls via MediatR
Application/Features/Student/PortfolioBook/Commands/SavePortfolioProfileCommand.cs
    ↓ injects ONE repository
IRepository<PortfolioBookProfile>
```

### Example 3: Get Portfolio Book (Query)

```
EndPoints/Student/PortfolioBook/GetPortfolioBookEndpoint.cs
    ↓ calls via MediatR
Application/Features/Student/PortfolioBook/Queries/GetPortfolioBookQuery.cs
    ↓ injects repositories (can be multiple for complex queries)
IRepository<PortfolioBookProfile>
IRepository<PortfolioBookGoals>
...
```

---

## Summary Table

| Type | Location | Repository Access | Purpose |
|------|----------|-------------------|---------|
| **Endpoint** | `EndPoints/{Role}/{Feature}/` | ❌ Never | HTTP routing, calls MediatR |
| **Query** | `Application/.../Queries/` | ✅ One repository | Read data |
| **Command** | `Application/.../Commands/` | ✅ One repository | Write data |
| **Orchestrator** | `Application/.../Commands/` | ❌ Never | Coordinate multiple requests |
| **DTO** | `Application/.../DTOs/` | N/A | Data transfer objects |

---

## Quick Reference

```
✅ Endpoint → MediatR → Query/Command/Orchestrator
✅ Query → One Repository → Read Data
✅ Command → One Repository → Write Data
✅ Orchestrator → MediatR → Multiple Queries/Commands

❌ Endpoint → Repository (NEVER)
❌ Orchestrator → Repository (NEVER)
❌ Multiple repositories in one Query/Command (NEVER)
```
