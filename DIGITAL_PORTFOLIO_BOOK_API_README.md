# Digital Portfolio Book API Endpoints

This README documents the backend endpoints required for the **Digital Portfolio Book** feature.

- **Route**: `/student/portfolio-book/:subjectId`
- **Angular Component**: `DigitalPortfolioBookComponent`

---

## 1. GET `/Student/PortfolioBook/{subjectId}`

- **Purpose**: Load the complete portfolio book data for a student's subject
- **Used by**: `DigitalPortfolioBookComponent` on init
- **Request**
  - Method: `GET`
  - Route parameter: `subjectId: string`
  - Auth: JWT identifies the current student

### Response Body

```json
{
  "SubjectId": "1449417842356125696",
  "SubjectName": "ELA Subject",
  "StudentName": "Ahmed Al Mansoori",
  "AcademicYear": "2024-25",
  "Profile": {
    "FullName": "Ahmed Al Mansoori",
    "GradeSection": "Grade 6-A",
    "FavoriteThings": "Reading, Football",
    "Uniqueness": "Creative storytelling",
    "FutureDream": "Author"
  },
  "Goals": {
    "AcademicGoal": "Improve reading speed",
    "BehavioralGoal": "Participate more",
    "PersonalGrowthGoal": "Better time management",
    "AchievementSteps": "Practice daily",
    "TargetDate": "2025-06-01"
  },
  "LearningStyle": {
    "LearnsBestBy": "visual",
    "BestTimeToStudy": "morning",
    "FocusConditions": "Quiet room",
    "HelpfulTools": "OneNote, flashcards",
    "Distractions": "Phone notifications"
  },
  "MapScores": [
    {
      "Id": "1",
      "Term": "Fall",
      "Year": 2024,
      "Score": 185,
      "DateTaken": "2024-09-15T00:00:00Z",
      "Percentile": 65,
      "Growth": null,
      "GoalScore": null
    }
  ],
  "ExactPathProgress": {
    "Reading": {
      "CurrentLevel": "6.2",
      "LessonsCompleted": 24,
      "TotalLessons": 40,
      "MinutesThisWeek": 120,
      "TargetCompletion": "End of Term 2"
    },
    "Vocabulary": {
      "CurrentLevel": "6.5",
      "WordsMastered": 156,
      "AccuracyRate": 82
    },
    "Grammar": {
      "CurrentLevel": "6.0",
      "LessonsCompleted": 18,
      "TotalLessons": 35,
      "FocusAreas": ["Punctuation", "Sentence Structure"]
    }
  },
  "Assignments": [],
  "Reflections": [],
  "JourneyEntries": [],
  "Milestones": [],
  "Projects": [],
  "Progress": {
    "CompletionPercentage": 78,
    "PagesCompleted": 7,
    "TotalPages": 9,
    "ReflectionsThisTerm": 12,
    "ProjectsUploaded": 3
  }
}
```

---

## 2. PUT `/Student/PortfolioBook/Profile`

- **Purpose**: Save the student's "All About Me" profile page
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "FullName": "Ahmed Al Mansoori",
  "GradeSection": "Grade 6-A",
  "FavoriteThings": "Reading, Football, Gaming",
  "Uniqueness": "Creative storytelling and problem solving",
  "FutureDream": "Become an author or game designer"
}
```

---

## 3. PUT `/Student/PortfolioBook/Goals`

- **Purpose**: Save learning goals
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "AcademicGoal": "Improve my reading comprehension",
  "BehavioralGoal": "Participate more in class discussions",
  "PersonalGrowthGoal": "Better time management",
  "AchievementSteps": "Practice reading for 30 mins daily",
  "TargetDate": "2025-06-01"
}
```

---

## 4. PUT `/Student/PortfolioBook/LearningStyle`

- **Purpose**: Save learning style preferences
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "LearnsBestBy": "visual",
  "BestTimeToStudy": "morning",
  "FocusConditions": "Quiet environment with soft background music",
  "HelpfulTools": "OneNote, YouTube tutorials, flashcards",
  "Distractions": "Phone notifications, noisy environments"
}
```

---

## 5. POST `/Student/PortfolioBook/Assignment`

- **Purpose**: Add or update an assignment entry
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "Id": null,
  "Name": "Book Report Chapter 3",
  "DueDate": "2025-01-15",
  "Status": "In Progress",
  "Notes": "Need to finish summary section"
}
```

- **Note**: If `Id` is null, creates new. If `Id` is provided, updates existing.

---

## 6. POST `/Student/PortfolioBook/Reflection`

- **Purpose**: Save a weekly reflection entry
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "WeekOf": "2025-01-13",
  "WhatLearned": "Learned about metaphors and similes",
  "BiggestAchievement": "Completed my first poem",
  "ChallengesFaced": "Struggled with grammar rules",
  "HelpNeeded": "More practice with punctuation",
  "Mood": "Good"
}
```

- **Mood values**: `Excellent`, `Good`, `Okay`, `Challenging`, `Difficult`

---

## 7. POST `/Student/PortfolioBook/Journey`

- **Purpose**: Save a learning journey entry
- **Access**: Student only
- **Request Body**

```json
{
  "SubjectId": "1449417842356125696",
  "Date": "2025-01-15",
  "SkillsWorking": "Reading comprehension, vocabulary",
  "EvidenceOfLearning": "Improved test scores, completed 3 book reports",
  "HowGrown": "Now able to read faster and understand better",
  "NextSteps": "Focus on writing skills next"
}
```

---

## 8. POST `/Student/PortfolioBook/Project`

- **Purpose**: Save a project with optional file uploads
- **Access**: Student only
- **Request**: `multipart/form-data`
  - `SubjectId: string`
  - `Title: string`
  - `Type: string` (`Writing Project`, `Research Report`, `Creative Project`, `Group Project`, `Digital Creation`, `Presentation`)
  - `Description: string`
  - `SkillsUsed: string`
  - `WhatLearned: string`
  - `Files: File[]` (optional, multiple files)

---

## Teacher-Only Endpoints

> [!IMPORTANT]
> The following endpoints are for **teachers only**. Students have read-only access to this data.

### 9. PUT `/Teacher/PortfolioBook/MapScore`

- **Purpose**: Update MAP assessment scores for a student
- **Access**: Teacher only
- **Request Body**

```json
{
  "StudentId": "student-uuid",
  "SubjectId": "1449417842356125696",
  "Term": "Winter",
  "Year": 2025,
  "Score": 192,
  "DateTaken": "2025-01-10",
  "Percentile": 72
}
```

### 10. PUT `/Teacher/PortfolioBook/ExactPath`

- **Purpose**: Update Exact Path progress for a student
- **Access**: Teacher only
- **Request Body**

```json
{
  "StudentId": "student-uuid",
  "SubjectId": "1449417842356125696",
  "Reading": {
    "CurrentLevel": "6.2",
    "LessonsCompleted": 24,
    "TotalLessons": 40,
    "MinutesThisWeek": 120,
    "TargetCompletion": "End of Term 2"
  },
  "Vocabulary": {
    "CurrentLevel": "6.5",
    "WordsMastered": 156,
    "AccuracyRate": 82
  },
  "Grammar": {
    "CurrentLevel": "6.0",
    "LessonsCompleted": 18,
    "TotalLessons": 35,
    "FocusAreas": ["Punctuation", "Sentence Structure"]
  }
}
```

---

## Response Wrapping

All responses follow the standard API envelope:

```json
{
  "IsSuccess": true,
  "Data": {
    /* DTO */
  },
  "Message": null,
  "ErrorCode": null
}
```

---

## Summary Table

| Endpoint                               | Method | Access  | Purpose                |
| -------------------------------------- | ------ | ------- | ---------------------- |
| `/Student/PortfolioBook/{subjectId}`   | GET    | Student | Load full book data    |
| `/Student/PortfolioBook/Profile`       | PUT    | Student | Save profile page      |
| `/Student/PortfolioBook/Goals`         | PUT    | Student | Save goals page        |
| `/Student/PortfolioBook/LearningStyle` | PUT    | Student | Save learning style    |
| `/Student/PortfolioBook/Assignment`    | POST   | Student | Add/update assignment  |
| `/Student/PortfolioBook/Reflection`    | POST   | Student | Save weekly reflection |
| `/Student/PortfolioBook/Journey`       | POST   | Student | Save journey entry     |
| `/Student/PortfolioBook/Project`       | POST   | Student | Save project + files   |
| `/Teacher/PortfolioBook/MapScore`      | PUT    | Teacher | Update MAP scores      |
| `/Teacher/PortfolioBook/ExactPath`     | PUT    | Teacher | Update Exact Path data |
