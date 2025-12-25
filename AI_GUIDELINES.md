# AI Coding Guidelines for School Hub

This document outlines the architectural patterns, coding standards, and best practices for the School Hub project. AI agents should follow these guidelines when generating code.

## 1. Architecture Overview

The project follows **Clean Architecture** with **CQRS** (Command Query Responsibility Segregation) and **Vertical Slicing**.

### Layers

- **API**: Presentation layer. Contains Endpoints (Minimal API), Middlewares, and Filters.
- **Application**: Business logic. Contains Features (Commands/Queries), DTOs, and Validators.
- **Domain**: Core entities, Enums, and Interfaces.
- **Infrastructure**: Data access, External services, and Configurations.
- **Shared**: Common utilities, Models, and Helpers.

## 2. Vertical Slicing

Features are organized by domain/feature rather than by technical layer where possible, especially in the `EndPoints` and `Application` layers.

### Directory Structure

```
EndPoints/
  User/
    Login/
      UserLoginEndPoint.cs
      UserLoginRequest.cs
      UserLoginResponse.cs
      UserLoginRequestValidator.cs
    Register/
      ...
```

```
Application/
  Features/
    User/
      Login/
        Commands/
          UserLoginOrchestrator.cs
      Register/
        ...
```

## 3. Coding Standards

### Endpoints

- Inherit from `EndpointDefinition`.
- Use `RegisterEndpoints` to map routes.
- Use `MediatR` to send Commands/Queries.
- Use `AddEndpointFilter<ValidationFilter<TRequest>>()` for validation.
- Return `EndPointResponse<T>`.

**Example:**

```csharp
public class MyEndpoint : EndpointDefinition
{
    public override void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("/my/route", async (IMediator mediator, MyRequest request, CancellationToken ct) =>
        {
            var result = await mediator.Send(new MyCommand(request), ct);
            return Response(result);
        })
        .WithTags("MyTag")
        .AddEndpointFilter<ValidationFilter<MyRequest>>()
        .Produces<EndPointResponse<MyResponse>>();
    }
}
```

### Entities

- Inherit from `BaseEntity`.
- Use Data Annotations for schema configuration (Table, Column, etc.) or Fluent API in DbContext.
- Place in `Domain/Entities/<FeatureName>/`.
- **Foreign Keys MUST be `long` type, NOT `Guid`.**
- BaseEntity provides: `ID`, `CompanyID`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `IsDeleted`, `TTL`, `RowVersion`.

**Example:**

```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Domain.Entities.MyFeature;

[Table("MyEntity", Schema = "MySchema")]
public class MyEntity : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public long RelatedEntityId { get; set; } // FK uses long, not Guid
}
```

### Enums

- All static values (status, type, priority, etc.) **MUST use enums**, not strings.
- Place enums in `Domain/Enums/`.
- Enum values should start from 1 (not 0) for database compatibility.

**Example:**

```csharp
namespace API.Domain.Enums;

public enum ProgressStatus
{
    NotStarted = 1,
    InProgress = 2,
    Completed = 3
}
```

### CQRS (MediatR)

- **Commands**: Modify state. Return `RequestResult<T>`.
- **Queries**: Read state. Return `RequestResult<T>` or `PagingDto<T>`.
- **Handlers**: Inherit from `RequestHandlerBase`.

**Example Command:**

```csharp
public record MyCommand(string Name) : IRequest<RequestResult<bool>>;

public class MyCommandHandler : RequestHandlerBase<MyCommand, RequestResult<bool>>
{
    public MyCommandHandler(RequestHandlerBaseParameters parameters) : base(parameters) {}

    public override async Task<RequestResult<bool>> Handle(MyCommand request, CancellationToken ct)
    {
        // Logic
        return RequestResult<bool>.Success(true);
    }
}
```

## 4. Database

- Database: **PostgreSQL**.
- ORM: **Dapper** (for queries/performance) and **Entity Framework Core** (optional/hybrid).
- Use `Npgsql` for connections.
- Primary Key: `long ID` (not Guid).
- Foreign Keys: `long` (not Guid).

## 5. General Rules

- **Async/Await**: Always use async/await.
- **CancellationTokens**: Pass `CancellationToken` to async methods.
- **Validation**: Use `FluentValidation` for request validation.
- **Error Handling**: Use `Result` pattern (`RequestResult`).
- **Default Values**: Initialize string properties with `string.Empty`, not null.
- **Nullable Properties**: Use `?` suffix for nullable types (e.g., `string?`, `long?`).

---

## 6. Existing Enums

| Enum Name              | Values                                                | Usage                               |
| ---------------------- | ----------------------------------------------------- | ----------------------------------- |
| `ApplicationRole`      | Admin=1, Teacher=3, Student=4                         | User roles                          |
| `ProgressStatus`       | NotStarted=1, InProgress=2, Completed=3               | Mission/Activity/Challenge progress |
| `ActivityType`         | OneNote=1, Video=2, Game=3, Reading=4, Quiz=5         | Activity types                      |
| `ChallengeType`        | Quiz=1, Game=2, Creative=3, Investigation=4           | Challenge types                     |
| `DifficultyLevel`      | Easy=1, Medium=2, Hard=3                              | Challenge difficulty                |
| `BadgeCategory`        | DigitalCitizenship=1, AITools=2, Microsoft365=3, etc. | Badge categories                    |
| `BadgeTargetRole`      | Student=1, Teacher=2, Both=3                          | Badge target audience               |
| `FileType`             | Pdf=1, Docx=2, Pptx=3, Jpg=4, Png=5, Mp4=6, Xlsx=7    | Portfolio file types                |
| `FeedbackType`         | Comment=1, RevisionRequest=2                          | Teacher feedback types              |
| `ReviewStatus`         | Pending=1, Reviewed=2, NeedsRevision=3                | Portfolio review status             |
| `SubmissionStatus`     | Pending=1, Approved=2, Rejected=3                     | Badge submission status             |
| `VideoProvider`        | YouTube=1, Vimeo=2, SelfHosted=3                      | Video providers                     |
| `PublishStatus`        | Draft=1, Published=2, Scheduled=3                     | Content publish status              |
| `NotificationType`     | MissionComplete=1, BadgeEarned=2, LevelUp=3, etc.     | Notification types                  |
| `ActivityLogType`      | Login=1, Badge=2, Upload=3, Completion=4, etc.        | Activity log types                  |
| `AnnouncementPriority` | Normal=1, Important=2, Urgent=3                       | Announcement priority               |
| `SettingDataType`      | String=1, Number=2, Boolean=3, Json=4                 | System setting data types           |
| `SettingCategory`      | General=1, Email=2, Notifications=3, Backup=4         | System setting categories           |
| `LogLevel`             | Info=1, Warning=2, Error=3, Debug=4                   | System log levels                   |
| `StudentLevelName`     | DigitalScout=1, Explorer=2, Champion=3, Leader=4      | Gamification levels                 |

---

## 7. Existing Entities

### Identity Schema

| Entity  | Table          | Description                                |
| ------- | -------------- | ------------------------------------------ |
| `User`  | Identity.User  | User accounts (students, teachers, admins) |
| `Token` | Identity.Token | JWT tokens for authentication              |

### General Schema

| Entity     | Table            | Description                      |
| ---------- | ---------------- | -------------------------------- |
| `Classes`  | General.Classes  | Student class/grade organization |
| `Subjects` | General.Subjects | Subject/course definitions       |

### Missions Schema

| Entity                    | Table                            | Description                            |
| ------------------------- | -------------------------------- | -------------------------------------- |
| `Missions`                | Missions.Missions                | Digital Citizenship missions (8 total) |
| `Activities`              | Missions.Activities              | Individual activities within missions  |
| `StudentMissionProgress`  | Missions.StudentMissionProgress  | Student progress on missions           |
| `StudentActivityProgress` | Missions.StudentActivityProgress | Student progress on activities         |

### Gamification Schema

| Entity              | Table                          | Description                |
| ------------------- | ------------------------------ | -------------------------- |
| `Badges`            | Gamification.Badges            | Badge definitions          |
| `StudentBadges`     | Gamification.StudentBadges     | Student badge awards       |
| `StudentLevels`     | Gamification.StudentLevels     | Student level/progression  |
| `Challenges`        | Gamification.Challenges        | Challenge Zone activities  |
| `StudentChallenges` | Gamification.StudentChallenges | Student challenge progress |
| `QuizAttempts`      | Gamification.QuizAttempts      | Mission quiz attempts      |

### Portfolio Schema

| Entity                 | Table                          | Description                      |
| ---------------------- | ------------------------------ | -------------------------------- |
| `PortfolioFiles`       | Portfolio.PortfolioFiles       | Student portfolio file uploads   |
| `PortfolioReflections` | Portfolio.PortfolioReflections | Student reflections (rich text)  |
| `TeacherFeedback`      | Portfolio.TeacherFeedback      | Teacher feedback on student work |
| `PortfolioLikes`       | Portfolio.PortfolioLikes       | Teacher likes on portfolios      |
| `PortfolioStatus`      | Portfolio.PortfolioStatus      | Portfolio review status          |

### Teacher Schema

| Entity                    | Table                           | Description                     |
| ------------------------- | ------------------------------- | ------------------------------- |
| `TeacherBadgeSubmissions` | Teacher.TeacherBadgeSubmissions | Teacher CPD badge submissions   |
| `CpdModules`              | Teacher.CpdModules              | CPD training modules            |
| `TeacherCpdProgress`      | Teacher.TeacherCpdProgress      | Teacher progress on CPD modules |
| `TeacherSubjects`         | Teacher.TeacherSubjects         | Teacher-subject assignments     |
| `WeeklyChallenges`        | Teacher.WeeklyChallenges        | Teacher weekly challenges       |

### System Schema

| Entity           | Table                 | Description                 |
| ---------------- | --------------------- | --------------------------- |
| `Notifications`  | System.Notifications  | User notifications          |
| `ActivityLogs`   | System.ActivityLogs   | System activity audit trail |
| `Announcements`  | System.Announcements  | System-wide announcements   |
| `SystemSettings` | System.SystemSettings | Global system configuration |
| `SystemLogs`     | System.SystemLogs     | System error and debug logs |

---

## 8. BaseEntity Properties

All entities inherit from `BaseEntity` which provides:

```csharp
public class BaseEntity
{
    public long ID { get; set; }                    // Primary Key
    public long CompanyID { get; set; }             // Multi-tenancy
    public DateTime CreatedAt { get; set; }         // Creation timestamp
    public long? CreatedBy { get; set; }            // Creator user ID
    public DateTime? UpdatedAt { get; set; }        // Last update timestamp
    public long? UpdatedBy { get; set; }            // Last updater user ID
    public bool IsDeleted { get; set; }             // Soft delete flag
    public DateTime? TTL { get; set; }              // Time-to-live
    public byte[] RowVersion { get; set; }          // Concurrency token
}
```
