# Teacher Missions and Student Attendance System - Setup Guide

## Overview
This document describes the new Teacher Missions and Student Attendance features that have been added to the School Hub system.

## Database Setup

### Step 1: Run SQL Migration Script
Execute the SQL script to create all necessary tables:

```bash
psql -U your_username -d your_database -f create_teacher_missions_and_attendance_tables.sql
```

Or run the individual scripts:
- `create_teacher_missions_tables.sql` - Creates Teacher Missions tables
- `create_attendance_tables.sql` - Creates Student Attendance table

### Tables Created

#### Teacher Missions Tables:
1. **TeacherMissions** - Mission definitions for teachers
2. **TeacherActivities** - Activities within teacher missions
3. **TeacherMissionProgress** - Progress tracking for teacher missions
4. **TeacherActivityProgress** - Activity completion tracking

#### Attendance Tables:
1. **StudentAttendance** - Daily class attendance records

## Backend API Endpoints

### Teacher Missions Endpoints

#### GET `/Teacher/Missions`
Get all available teacher missions.

**Response:**
```json
{
  "IsSuccess": true,
  "Data": [
    {
      "Id": "123",
      "Title": "Mission Title",
      "Description": "Mission Description",
      "Icon": "📚",
      "Status": "not-started",
      "Progress": 0,
      "Badge": "Badge Name",
      "Duration": "30 mins",
      "Requirements": []
    }
  ]
}
```

#### GET `/Teacher/Missions/{missionId}`
Get detailed information about a specific mission.

#### GET `/Teacher/Missions/Progress`
Get teacher's progress summary for all missions.

#### POST `/Teacher/Missions/{missionId}/Start`
Start a new mission.

#### POST `/Teacher/Missions/{missionId}/Progress`
Update mission progress (mark activity as completed).

### Teacher Attendance Endpoints

#### GET `/Teacher/Classes/{classId}/Attendance/{date}`
Get attendance records for a class on a specific date.

**Response:**
```json
{
  "IsSuccess": true,
  "Data": {
    "Date": "2025-01-15",
    "ClassId": "123",
    "ClassName": "Grade 5A",
    "Students": [
      {
        "Id": "456",
        "StudentId": "789",
        "StudentName": "John Doe",
        "Status": "Present",
        "IsAutomatic": false
      }
    ],
    "PresentCount": 20,
    "AbsentCount": 2,
    "TotalStudents": 22
  }
}
```

#### POST `/Teacher/Classes/{classId}/Attendance`
Mark attendance for multiple students.

**Request Body:**
```json
{
  "AttendanceDate": "2025-01-15",
  "Students": [
    {
      "StudentId": "789",
      "Status": "Present",
      "Notes": "On time"
    }
  ]
}
```

#### POST `/Teacher/Classes/{classId}/Attendance/Bulk`
Bulk mark attendance with same status for multiple students.

#### PUT `/Teacher/Attendance/{attendanceId}`
Update an existing attendance record.

#### POST `/Teacher/Classes/{classId}/Attendance/ProcessAutomatic`
Process automatic attendance based on student activity.

### Student Attendance Endpoints

#### GET `/Student/Attendance`
Get student's attendance history.

**Query Parameters:**
- `startDate` (optional): Start date for filtering
- `endDate` (optional): End date for filtering

#### GET `/Student/Attendance/Statistics`
Get attendance statistics (percentage, streaks, etc.).

**Response:**
```json
{
  "IsSuccess": true,
  "Data": {
    "TotalDays": 30,
    "PresentDays": 28,
    "AbsentDays": 2,
    "AttendancePercentage": 93.33,
    "CurrentStreak": 10,
    "LongestStreak": 15
  }
}
```

#### POST `/Student/Attendance/CalculateBonus`
Calculate attendance bonus points/badges.

#### POST `/Student/Attendance/AwardBonus`
Award attendance bonus to student.

## Frontend Integration

### Services Created

#### Teacher Services:
- `MissionService` - Located at `Front-End/src/app/features/teacher/services/mission.service.ts`
- `AttendanceService` - Located at `Front-End/src/app/features/teacher/services/attendance.service.ts`

#### Student Services:
- `StudentAttendanceService` - Located at `Front-End/src/app/features/student/services/student-attendance.service.ts`

### Models Created

#### Teacher Models:
- `mission.model.ts` - Teacher mission interfaces
- `attendance.model.ts` - Attendance interfaces

#### Student Models:
- `attendance.model.ts` - Student attendance interfaces

### Usage Examples

#### Teacher Missions Service:
```typescript
import { MissionService } from './features/teacher/services/mission.service';

constructor(private missionService: MissionService) {}

// Load all missions
this.missionService.loadMissions().subscribe(response => {
  if (response.IsSuccess) {
    console.log('Missions:', this.missionService.missions());
  }
});

// Start a mission
this.missionService.startMission('123').subscribe(response => {
  console.log('Mission started');
});

// Update progress
this.missionService.updateProgress('123', {
  MissionId: '123',
  ActivityId: '456',
  Completed: true
}).subscribe();
```

#### Teacher Attendance Service:
```typescript
import { AttendanceService } from './features/teacher/services/attendance.service';

constructor(private attendanceService: AttendanceService) {}

// Get class attendance
const date = new Date();
this.attendanceService.getClassAttendance('classId', date).subscribe(response => {
  if (response.IsSuccess) {
    console.log('Attendance:', this.attendanceService.classAttendance());
  }
});

// Mark attendance
this.attendanceService.markAttendance('classId', {
  AttendanceDate: '2025-01-15',
  Students: [
    { StudentId: '123', Status: 'Present' }
  ]
}).subscribe();
```

#### Student Attendance Service:
```typescript
import { StudentAttendanceService } from './features/student/services/student-attendance.service';

constructor(private attendanceService: StudentAttendanceService) {}

// Get statistics
this.attendanceService.getStatistics().subscribe(response => {
  if (response.IsSuccess) {
    console.log('Statistics:', this.attendanceService.statistics());
  }
});

// Calculate bonus
this.attendanceService.calculateBonus().subscribe(response => {
  console.log('Bonus:', response.Data);
});
```

## Features

### Teacher Missions
- Teachers can view available missions
- Start missions and track progress
- Complete activities within missions
- Earn badges upon mission completion

### Student Attendance
- **Manual Marking**: Teachers can manually mark attendance
- **Bulk Marking**: Mark multiple students at once
- **Automatic Marking**: System auto-marks based on student activity
- **Statistics**: Track attendance percentage, streaks, and history
- **Bonus System**: Award points and badges for good attendance

### Attendance Bonus Rules
- Perfect week (5+ days) → +50 points
- Perfect month (15+ days) → +200 points + badge
- Attendance streaks → 5 points per day in streak

## Next Steps

1. **Run Database Migration**: Execute the SQL scripts
2. **Test Backend APIs**: Verify all endpoints work correctly
3. **Create UI Components**: Build frontend pages/components for:
   - Teacher Missions dashboard
   - Mission detail page
   - Attendance marking interface
   - Attendance reports
   - Student attendance view
4. **Add Routes**: Add routes to teacher and student routing modules
5. **Test Integration**: Test end-to-end functionality

## Notes

- All IDs are handled as strings in the frontend (due to LongAsStringConverter)
- Attendance dates should be in ISO format (YYYY-MM-DD)
- Automatic attendance processing checks for student activity during class hours
- Attendance bonuses are calculated based on recent attendance records (last 30 days)

