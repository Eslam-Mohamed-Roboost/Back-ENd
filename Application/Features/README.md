# Application Features Index

This document provides a comprehensive index of the application's features within the `Application/Features` directory.

## Admin Features

### Activity Logs

- `Application/Features/Admin/ActivityLogs`

### Announcements

- `Application/Features/Admin/Announcements`

### Badges & Submissions

- `Application/Features/Admin/Badges`
- `Application/Features/Admin/BadgeSubmissions`

### CPD (Continuous Professional Development)

- `Application/Features/Admin/CPD`

### Challenges

- `Application/Features/Admin/Challenges`
- `Application/Features/Admin/WeeklyChallenges`

### Dashboard & Stats

- `Application/Features/Admin/Dashboard`
- `Application/Features/Admin/DashboardStats`
- `Application/Features/Admin/GetAdminKPI`

### Evidence

- `Application/Features/Admin/Evidence`

### Users Management

- `Application/Features/Admin/GetUsers`
- `Application/Features/Admin/UpdateUser`

### Missions

- `Application/Features/Admin/Missions`

### Portfolio

- `Application/Features/Admin/Portfolio`

### Reports

- `Application/Features/Admin/Reports`

### Settings & Seed Data

- `Application/Features/Admin/Settings`
- `Application/Features/Admin/SeedData`

### Teacher Subjects

- `Application/Features/Admin/TeacherSubjects`

---

## Student Features

### Activity

- `Application/Features/Student/Activity`

### Badges

- `Application/Features/Student/Badges`

### Challenges & Progress

- `Application/Features/Student/Challenges`
- `Application/Features/Student/Progress`

### Dashboard

- `Application/Features/Student/Dashboard`

### Goals

- `Application/Features/Student/Goals`

### Missions

- `Application/Features/Student/Missions`

### Notebook

- `Application/Features/Student/Notebook`

### Notifications

- `Application/Features/Student/Notifications`

### Portfolio

- **Standard Portfolio**: `Application/Features/Student/Portfolio`
- **Digital Portfolio Book**: `Application/Features/Student/PortfolioBook` (New Feature)

---

## Teacher Features

### CPD

- `Application/Features/Teacher/Cpd`

### Dashboard

- `Application/Features/Teacher/Dashboard`

### Portfolio

- **Standard Portfolio**: `Application/Features/Teacher/Portfolio`
- **Digital Portfolio Book**: `Application/Features/Teacher/PortfolioBook` (New Feature)

---

## User Features (Shared)

### Authentication

- `Application/Features/User/Login`
- `Application/Features/User/Register`

### Data

- `Application/Features/User/Export`
- `Application/Features/User/GetUsers`

---

# Refactoring Instructions

- **Optimize Queries**: Use eager loading (`Include`/`ThenInclude`) in repositories to avoid N+1 queries.
- **Colocate Handlers**: Query/Command definitions and their Handlers must be in the **same file**.
- **Orchestrator Pattern**: Handlers requiring **multiple repositories** must be refactored into an **Orchestrator**.
  - The Orchestrator should **not** inject repositories directly.
  - Instead, it should coordinate logic by calling smaller, single-responsibility Queries/Commands via `IMediator`.
