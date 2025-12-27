# Student Mission Tracking Guide

## Overview

The system tracks student mission progress at two levels:
1. **Mission Level** (`StudentMissionProgress`) - Overall mission completion status
2. **Activity Level** (`StudentActivityProgress`) - Individual activity completion within missions

## Database Entities

### StudentMissionProgress
Tracks overall mission progress for each student:
- `StudentId` - The student
- `MissionId` - The mission being tracked
- `Status` - ProgressStatus enum (NotStarted, InProgress, Completed)
- `CompletedActivities` - Number of activities completed
- `TotalActivities` - Total activities in the mission
- `ProgressPercentage` - Calculated percentage (0-100)
- `StartedAt` - When student first started the mission
- `CompletedAt` - When mission was completed

### StudentActivityProgress
Tracks individual activity completion:
- `StudentId` - The student
- `ActivityId` - The activity
- `MissionId` - The parent mission
- `IsCompleted` - Boolean completion flag
- `CompletedAt` - Timestamp of completion
- `Notes` - Optional notes

## Tracking Flow

### 1. Student Starts a Mission
When a student views mission details or completes their first activity:
- `StudentMissionProgress` record is created automatically
- Status set to `InProgress`
- `StartedAt` timestamp recorded
- `TotalActivities` count populated

### 2. Student Completes an Activity
When student marks an activity as complete:
- `StudentActivityProgress` record is created/updated
- `CompletedAt` timestamp recorded
- `StudentMissionProgress.CompletedActivities` incremented
- `ProgressPercentage` recalculated
- If all activities completed:
  - Status changes to `Completed`
  - `CompletedAt` timestamp recorded
  - Badge automatically awarded (if mission has BadgeId)

### 3. Progress Updates
Progress is updated via:
- **Endpoint**: `POST /Student/Missions/{missionId}/Progress`
- **Request Body**: `UpdateMissionProgressRequest`
  ```json
  {
    "ActivityId": "123",
    "Completed": true,
    "ActivityData": { /* optional activity-specific data */ }
  }
  ```

## API Endpoints

### Student Endpoints

#### Get All Missions (with progress)
- **GET** `/Student/Missions`
- **Returns**: `List<MissionDto>`
- **Includes**: Status, Progress percentage, Requirements

#### Get Mission Details
- **GET** `/Student/Missions/{missionId}`
- **Returns**: `MissionDetailDto`
- **Includes**: Activities with completion status, Resources, Progress

#### Update Mission Progress
- **POST** `/Student/Missions/{missionId}/Progress`
- **Body**: `UpdateMissionProgressRequest`
- **Returns**: `MissionProgressResponse`
- **Triggers**: Activity completion, mission completion check, badge award

#### Get Student Progress Summary
- **GET** `/Student/Progress`
- **Returns**: `StudentProgressDto`
- **Includes**: Mission statistics, badge progress, subject progress

### Admin Endpoints

#### Get Mission Progress Overview
- **GET** `/Admin/Missions/Progress`
- **Returns**: `MissionProgressOverviewDto`
- **Includes**: Overall statistics, per-mission progress, at-risk students

### Teacher Endpoints

#### Get Class Mission Progress
- **GET** `/Teacher/Missions/ClassProgress`
- **Returns**: `List<ClassMissionProgressDto>`
- **Includes**: Progress for all students in teacher's classes

## Tracking Data Available

### For Students
- Mission status (not-started, in-progress, completed, locked)
- Progress percentage (0-100%)
- Completed activities count
- Requirements/prerequisites
- Badge earned status
- Activity completion status

### For Teachers/Admins
- Student mission completion rates
- Class-level mission statistics
- At-risk students (low completion rates)
- Mission popularity (start/completion rates)
- Time to completion metrics
- Activity-level completion tracking

## Implementation Details

### Automatic Tracking
- Mission progress is created automatically when first activity is completed
- Badge is awarded automatically when mission is completed
- Progress percentage is calculated automatically
- Mission status updates automatically based on activity completion

### Manual Tracking
- Teachers/Admins can view progress but cannot manually update it
- Progress is student-driven (students mark activities as complete)
- System ensures data integrity (prevents duplicate completions)

## Example Usage

### Frontend: Update Progress
```typescript
// When student completes an activity
this.missionsService.updateMissionProgress({
  MissionId: missionId,
  ActivityId: activityId,
  Completed: true
});
```

### Backend: Query Progress
```csharp
// Get student's mission progress
var progress = await progressRepository
    .Get(x => x.StudentId == studentId && x.MissionId == missionId)
    .FirstOrDefaultAsync();

// Get all completed activities
var completedActivities = await activityProgressRepository
    .Get(x => x.StudentId == studentId && x.MissionId == missionId && x.IsCompleted)
    .ToListAsync();
```

## Tracking Metrics

The system tracks:
- ✅ Mission start/completion rates
- ✅ Activity completion rates
- ✅ Time to complete missions
- ✅ Student engagement levels
- ✅ Prerequisite completion
- ✅ Badge award tracking
- ✅ Progress over time

## Best Practices

1. **Always check prerequisites** before allowing mission start
2. **Validate activity completion** before updating progress
3. **Use transactions** when updating both activity and mission progress
4. **Cache progress data** for better performance
5. **Log progress updates** for audit trails



