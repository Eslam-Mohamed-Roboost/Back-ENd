-- =============================================
-- School Hub Database Schema
-- PostgreSQL 14+
-- Generated: 2025-12-05
-- =============================================

-- Create Schemas
CREATE SCHEMA IF NOT EXISTS "Identity";
CREATE SCHEMA IF NOT EXISTS "General";
CREATE SCHEMA IF NOT EXISTS "Missions";
CREATE SCHEMA IF NOT EXISTS "Gamification";
CREATE SCHEMA IF NOT EXISTS "Portfolio";
CREATE SCHEMA IF NOT EXISTS "Teacher";
CREATE SCHEMA IF NOT EXISTS "System";
CREATE SCHEMA IF NOT EXISTS "Academic";

-- =============================================
-- IDENTITY SCHEMA
-- =============================================

-- Users Table
CREATE TABLE IF NOT EXISTS "Identity"."User" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Name" VARCHAR(200) NOT NULL,
    "UserName" VARCHAR(100) NOT NULL,
    "Email" VARCHAR(255) NOT NULL,
    "Password" VARCHAR(500) NOT NULL,
    "SaltPassword" VARCHAR(500) NOT NULL,
    "PhoneNumber" VARCHAR(20),
    "Role" INTEGER NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_user_username" ON "Identity"."User" ("UserName") WHERE "IsDeleted" = FALSE;
CREATE UNIQUE INDEX IF NOT EXISTS "idx_user_email" ON "Identity"."User" ("Email") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_user_role" ON "Identity"."User" ("Role");
CREATE INDEX IF NOT EXISTS "idx_user_companyid" ON "Identity"."User" ("CompanyID");

-- Token Table
CREATE TABLE IF NOT EXISTS "Identity"."Token" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "UserID" BIGINT NOT NULL,
    "JwtID" BIGINT NOT NULL,
    "LoggedOutAt" TIMESTAMP WITH TIME ZONE,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_token_userid" ON "Identity"."Token" ("UserID");
CREATE UNIQUE INDEX IF NOT EXISTS "idx_token_jwtid" ON "Identity"."Token" ("JwtID");
CREATE INDEX IF NOT EXISTS "idx_token_isactive" ON "Identity"."Token" ("IsActive");

-- =============================================
-- GENERAL SCHEMA
-- =============================================

-- Classes Table
CREATE TABLE IF NOT EXISTS "General"."Classes" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Name" VARCHAR(50) NOT NULL,
    "Grade" INTEGER NOT NULL,
    "TeacherId" BIGINT,
    "StudentCount" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_classes_grade" ON "General"."Classes" ("Grade");
CREATE INDEX IF NOT EXISTS "idx_classes_teacherid" ON "General"."Classes" ("TeacherId");

-- Subjects Table
CREATE TABLE IF NOT EXISTS "General"."Subjects" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Name" VARCHAR(200) NOT NULL,
    "Icon" VARCHAR(100),
    "Color" VARCHAR(50),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

-- =============================================
-- MISSIONS SCHEMA
-- =============================================

-- Missions Table
CREATE TABLE IF NOT EXISTS "Missions"."Missions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Number" INTEGER NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Icon" VARCHAR(50),
    "EstimatedMinutes" INTEGER NOT NULL DEFAULT 30,
    "BadgeId" BIGINT NOT NULL,
    "Order" INTEGER NOT NULL,
    "IsEnabled" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_missions_number" ON "Missions"."Missions" ("Number") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_missions_order" ON "Missions"."Missions" ("Order");

-- Activities Table
CREATE TABLE IF NOT EXISTS "Missions"."Activities" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "MissionId" BIGINT NOT NULL,
    "Number" INTEGER NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Type" INTEGER NOT NULL,
    "ContentUrl" TEXT,
    "EstimatedMinutes" INTEGER NOT NULL DEFAULT 10,
    "Instructions" TEXT,
    "Order" INTEGER NOT NULL,
    "IsRequired" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_activities_missionid" ON "Missions"."Activities" ("MissionId");
CREATE INDEX IF NOT EXISTS "idx_activities_order" ON "Missions"."Activities" ("MissionId", "Order");

-- StudentMissionProgress Table
CREATE TABLE IF NOT EXISTS "Missions"."StudentMissionProgress" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "MissionId" BIGINT NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "CompletedActivities" INTEGER NOT NULL DEFAULT 0,
    "TotalActivities" INTEGER NOT NULL,
    "ProgressPercentage" DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    "StartedAt" TIMESTAMP WITH TIME ZONE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentmissionprogress_unique" ON "Missions"."StudentMissionProgress" ("StudentId", "MissionId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentmissionprogress_studentid" ON "Missions"."StudentMissionProgress" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_studentmissionprogress_status" ON "Missions"."StudentMissionProgress" ("Status");

-- StudentActivityProgress Table
CREATE TABLE IF NOT EXISTS "Missions"."StudentActivityProgress" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "ActivityId" BIGINT NOT NULL,
    "IsCompleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentactivityprogress_unique" ON "Missions"."StudentActivityProgress" ("StudentId", "ActivityId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentactivityprogress_studentid" ON "Missions"."StudentActivityProgress" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_studentactivityprogress_activityid" ON "Missions"."StudentActivityProgress" ("ActivityId");

-- =============================================
-- GAMIFICATION SCHEMA
-- =============================================

-- Badges Table
CREATE TABLE IF NOT EXISTS "Gamification"."Badges" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Name" VARCHAR(200) NOT NULL,
    "Description" TEXT,
    "Icon" VARCHAR(100),
    "Color" VARCHAR(50),
    "Category" INTEGER NOT NULL,
    "TargetRole" INTEGER NOT NULL,
    "CpdHours" DECIMAL(4,2),
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_badges_category" ON "Gamification"."Badges" ("Category");
CREATE INDEX IF NOT EXISTS "idx_badges_targetrole" ON "Gamification"."Badges" ("TargetRole");

-- StudentBadges Table
CREATE TABLE IF NOT EXISTS "Gamification"."StudentBadges" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "BadgeId" BIGINT NOT NULL,
    "EarnedDate" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "MissionId" BIGINT,
    "AutoAwarded" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentbadges_unique" ON "Gamification"."StudentBadges" ("StudentId", "BadgeId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentbadges_studentid" ON "Gamification"."StudentBadges" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_studentbadges_earneddate" ON "Gamification"."StudentBadges" ("EarnedDate");

-- StudentLevels Table
CREATE TABLE IF NOT EXISTS "Gamification"."StudentLevels" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "CurrentLevel" INTEGER NOT NULL DEFAULT 1,
    "LevelName" INTEGER,
    "BadgesEarned" INTEGER NOT NULL DEFAULT 0,
    "NextLevelBadgeCount" INTEGER,
    "LevelIcon" VARCHAR(100),
    "LastLevelUpDate" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentlevels_studentid" ON "Gamification"."StudentLevels" ("StudentId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentlevels_currentlevel" ON "Gamification"."StudentLevels" ("CurrentLevel");

-- Challenges Table
CREATE TABLE IF NOT EXISTS "Gamification"."Challenges" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Title" VARCHAR(300) NOT NULL,
    "Description" TEXT,
    "Type" INTEGER NOT NULL,
    "Difficulty" INTEGER NOT NULL,
    "EstimatedMinutes" INTEGER NOT NULL DEFAULT 15,
    "Icon" VARCHAR(100),
    "BackgroundColor" VARCHAR(50),
    "ContentUrl" TEXT,
    "Points" INTEGER NOT NULL DEFAULT 25,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_challenges_difficulty" ON "Gamification"."Challenges" ("Difficulty");
CREATE INDEX IF NOT EXISTS "idx_challenges_type" ON "Gamification"."Challenges" ("Type");

-- StudentChallenges Table
CREATE TABLE IF NOT EXISTS "Gamification"."StudentChallenges" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "ChallengeId" BIGINT NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "Score" INTEGER,
    "PointsEarned" INTEGER NOT NULL DEFAULT 0,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentchallenges_unique" ON "Gamification"."StudentChallenges" ("StudentId", "ChallengeId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentchallenges_studentid" ON "Gamification"."StudentChallenges" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_studentchallenges_status" ON "Gamification"."StudentChallenges" ("Status");

-- QuizAttempts Table
CREATE TABLE IF NOT EXISTS "Gamification"."QuizAttempts" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "MissionId" BIGINT NOT NULL,
    "Score" DECIMAL(5,2) NOT NULL,
    "PassScore" DECIMAL(5,2) NOT NULL DEFAULT 70.00,
    "IsPassed" BOOLEAN NOT NULL,
    "Answers" JSONB,
    "AttemptNumber" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_quizattempts_studentid" ON "Gamification"."QuizAttempts" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_quizattempts_missionid" ON "Gamification"."QuizAttempts" ("MissionId");
CREATE INDEX IF NOT EXISTS "idx_quizattempts_createdat" ON "Gamification"."QuizAttempts" ("CreatedAt");

-- =============================================
-- PORTFOLIO SCHEMA
-- =============================================

-- PortfolioFiles Table
CREATE TABLE IF NOT EXISTS "Portfolio"."PortfolioFiles" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "FileName" VARCHAR(500) NOT NULL,
    "FileType" INTEGER NOT NULL,
    "FileSize" BIGINT NOT NULL,
    "StoragePath" TEXT NOT NULL,
    "ThumbnailUrl" TEXT,
    "PreviewUrl" TEXT,
    "DownloadUrl" TEXT NOT NULL,
    "UploadedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending',
    "ReviewedBy" BIGINT,
    "ReviewedAt" TIMESTAMP WITH TIME ZONE,
    "RevisionNotes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_PortfolioFiles_ReviewedBy" FOREIGN KEY ("ReviewedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL,
    CONSTRAINT "chk_portfoliofiles_status" CHECK ("Status" IN ('Pending', 'Reviewed', 'NeedsRevision'))
);

CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_studentid" ON "Portfolio"."PortfolioFiles" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_subjectid" ON "Portfolio"."PortfolioFiles" ("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_uploadedat" ON "Portfolio"."PortfolioFiles" ("UploadedAt");
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_status" ON "Portfolio"."PortfolioFiles"("Status");
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_reviewedby" ON "Portfolio"."PortfolioFiles"("ReviewedBy");

-- PortfolioReflections Table
CREATE TABLE IF NOT EXISTS "Portfolio"."PortfolioReflections" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "Content" TEXT NOT NULL,
    "Prompt" TEXT,
    "IsAutoSaved" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_portfolioreflections_studentid" ON "Portfolio"."PortfolioReflections" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_portfolioreflections_subjectid" ON "Portfolio"."PortfolioReflections" ("SubjectId");

-- TeacherFeedback Table
CREATE TABLE IF NOT EXISTS "Portfolio"."TeacherFeedback" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "TeacherId" BIGINT NOT NULL,
    "SubjectId" BIGINT,
    "FileId" BIGINT,
    "Comment" TEXT NOT NULL,
    "Type" INTEGER NOT NULL,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_teacherfeedback_studentid" ON "Portfolio"."TeacherFeedback" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_teacherfeedback_teacherid" ON "Portfolio"."TeacherFeedback" ("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacherfeedback_createdat" ON "Portfolio"."TeacherFeedback" ("CreatedAt");

-- PortfolioLikes Table
CREATE TABLE IF NOT EXISTS "Portfolio"."PortfolioLikes" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "StudentId" BIGINT NOT NULL,
    "SubjectId" BIGINT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_portfoliolikes_unique" ON "Portfolio"."PortfolioLikes" ("TeacherId", "StudentId", "SubjectId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_portfoliolikes_studentid" ON "Portfolio"."PortfolioLikes" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_portfoliolikes_teacherid" ON "Portfolio"."PortfolioLikes" ("TeacherId");

-- PortfolioStatus Table
CREATE TABLE IF NOT EXISTS "Portfolio"."PortfolioStatus" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "LastReviewedBy" BIGINT,
    "LastReviewedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_portfoliostatus_unique" ON "Portfolio"."PortfolioStatus" ("StudentId", "SubjectId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_portfoliostatus_studentid" ON "Portfolio"."PortfolioStatus" ("StudentId");
CREATE INDEX IF NOT EXISTS "idx_portfoliostatus_status" ON "Portfolio"."PortfolioStatus" ("Status");

-- =============================================
-- TEACHER SCHEMA
-- =============================================

-- TeacherBadgeSubmissions Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherBadgeSubmissions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "BadgeId" BIGINT NOT NULL,
    "EvidenceLink" TEXT NOT NULL,
    "SubmitterNotes" TEXT,
    "SubmittedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "Status" INTEGER NOT NULL DEFAULT 1,
    "ReviewedBy" BIGINT,
    "ReviewedAt" TIMESTAMP WITH TIME ZONE,
    "ReviewNotes" TEXT,
    "CpdHoursAwarded" DECIMAL(4,2),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_teacherbadgesubmissions_teacherid" ON "Teacher"."TeacherBadgeSubmissions" ("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacherbadgesubmissions_status" ON "Teacher"."TeacherBadgeSubmissions" ("Status");
CREATE INDEX IF NOT EXISTS "idx_teacherbadgesubmissions_submittedat" ON "Teacher"."TeacherBadgeSubmissions" ("SubmittedAt");

-- CpdModules Table
CREATE TABLE IF NOT EXISTS "Teacher"."CpdModules" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Title" VARCHAR(300) NOT NULL,
    "Description" TEXT,
    "DurationMinutes" INTEGER NOT NULL,
    "Icon" VARCHAR(100),
    "Color" VARCHAR(50),
    "BackgroundColor" VARCHAR(50),
    "VideoUrl" TEXT,
    "VideoProvider" INTEGER,
    "GuideContent" TEXT,
    "FormUrl" TEXT,
    "BadgeId" BIGINT,
    "Order" INTEGER NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_cpdmodules_order" ON "Teacher"."CpdModules" ("Order");

-- TeacherCpdProgress Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherCpdProgress" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ModuleId" BIGINT NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "StartedAt" TIMESTAMP WITH TIME ZONE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "LastAccessedAt" TIMESTAMP WITH TIME ZONE,
    "EvidenceFiles" JSONB,
    "HoursEarned" DECIMAL(4,2),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_teachercpdprogress_unique" ON "Teacher"."TeacherCpdProgress" ("TeacherId", "ModuleId") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_teachercpdprogress_teacherid" ON "Teacher"."TeacherCpdProgress" ("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teachercpdprogress_status" ON "Teacher"."TeacherCpdProgress" ("Status");

-- TeacherSubjects Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherSubjects" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "Grade" INTEGER NOT NULL,
    "AssignedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_teachersubjects_unique" ON "Teacher"."TeacherSubjects" ("TeacherId", "SubjectId", "Grade") WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_teachersubjects_teacherid" ON "Teacher"."TeacherSubjects" ("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teachersubjects_subjectid" ON "Teacher"."TeacherSubjects" ("SubjectId");

-- TeacherClassAssignments Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherClassAssignments" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ClassId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "AssignedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherClassAssignments_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_TeacherClassAssignments_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_TeacherClassAssignments_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "idx_teacherclass_teacher" ON "Teacher"."TeacherClassAssignments"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_class" ON "Teacher"."TeacherClassAssignments"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_subject" ON "Teacher"."TeacherClassAssignments"("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_isdeleted" ON "Teacher"."TeacherClassAssignments"("IsDeleted");
CREATE UNIQUE INDEX IF NOT EXISTS "idx_teacherclass_unique" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- WeeklyChallenges Table
CREATE TABLE IF NOT EXISTS "Teacher"."WeeklyChallenges" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "WeekNumber" INTEGER NOT NULL,
    "Title" VARCHAR(300) NOT NULL,
    "Description" TEXT NOT NULL,
    "ResourceLinks" JSONB,
    "TutorialVideo" TEXT,
    "SubmissionFormLink" TEXT,
    "Status" INTEGER NOT NULL DEFAULT 1,
    "PublishedAt" TIMESTAMP WITH TIME ZONE,
    "AutoNotify" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_weeklychallenges_weeknumber" ON "Teacher"."WeeklyChallenges" ("WeekNumber");
CREATE INDEX IF NOT EXISTS "idx_weeklychallenges_status" ON "Teacher"."WeeklyChallenges" ("Status");

-- =============================================
-- SYSTEM SCHEMA
-- =============================================

-- Notifications Table
CREATE TABLE IF NOT EXISTS "System"."Notifications" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "UserId" BIGINT NOT NULL,
    "Type" INTEGER NOT NULL,
    "Title" VARCHAR(300) NOT NULL,
    "Message" TEXT NOT NULL,
    "Icon" VARCHAR(100),
    "Link" TEXT,
    "IsRead" BOOLEAN NOT NULL DEFAULT FALSE,
    "ReadAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_notifications_userid" ON "System"."Notifications" ("UserId");
CREATE INDEX IF NOT EXISTS "idx_notifications_isread" ON "System"."Notifications" ("IsRead");
CREATE INDEX IF NOT EXISTS "idx_notifications_createdat" ON "System"."Notifications" ("CreatedAt");

-- ActivityLogs Table
CREATE TABLE IF NOT EXISTS "System"."ActivityLogs" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "UserId" BIGINT NOT NULL,
    "Action" VARCHAR(300) NOT NULL,
    "Type" INTEGER NOT NULL,
    "Details" TEXT,
    "IpAddress" VARCHAR(50),
    "UserAgent" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_activitylogs_userid" ON "System"."ActivityLogs" ("UserId");
CREATE INDEX IF NOT EXISTS "idx_activitylogs_type" ON "System"."ActivityLogs" ("Type");
CREATE INDEX IF NOT EXISTS "idx_activitylogs_createdat" ON "System"."ActivityLogs" ("CreatedAt" DESC);

-- Announcements Table
CREATE TABLE IF NOT EXISTS "System"."Announcements" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Title" VARCHAR(300) NOT NULL,
    "Content" TEXT NOT NULL,
    "Priority" INTEGER NOT NULL DEFAULT 1,
    "TargetAudience" JSONB NOT NULL,
    "IsPinned" BOOLEAN NOT NULL DEFAULT FALSE,
    "ShowAsPopup" BOOLEAN NOT NULL DEFAULT FALSE,
    "SendEmail" BOOLEAN NOT NULL DEFAULT FALSE,
    "PublishedAt" TIMESTAMP WITH TIME ZONE,
    "ViewCount" INTEGER NOT NULL DEFAULT 0,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_announcements_publishedat" ON "System"."Announcements" ("PublishedAt");
CREATE INDEX IF NOT EXISTS "idx_announcements_priority" ON "System"."Announcements" ("Priority");

-- SystemSettings Table
CREATE TABLE IF NOT EXISTS "System"."SystemSettings" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "SettingKey" VARCHAR(200) NOT NULL,
    "SettingValue" TEXT NOT NULL,
    "DataType" INTEGER NOT NULL DEFAULT 1,
    "Category" INTEGER NOT NULL DEFAULT 1,
    "Description" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_systemsettings_settingkey" ON "System"."SystemSettings" ("SettingKey") WHERE "IsDeleted" = FALSE;

-- SystemLogs Table
CREATE TABLE IF NOT EXISTS "System"."SystemLogs" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Level" INTEGER NOT NULL DEFAULT 1,
    "UserId" BIGINT,
    "Action" VARCHAR(300) NOT NULL,
    "Details" TEXT,
    "IpAddress" VARCHAR(50),
    "StackTrace" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000'
);

CREATE INDEX IF NOT EXISTS "idx_systemlogs_level" ON "System"."SystemLogs" ("Level");
CREATE INDEX IF NOT EXISTS "idx_systemlogs_createdat" ON "System"."SystemLogs" ("CreatedAt" DESC);

-- =============================================
-- ACADEMIC SCHEMA
-- =============================================

-- Exercises Table
CREATE TABLE IF NOT EXISTS "Academic"."Exercises" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ClassId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "Title" VARCHAR(500) NOT NULL,
    "Description" TEXT,
    "Type" VARCHAR(50) NOT NULL, -- Homework, Classwork, Project
    "DueDate" TIMESTAMP WITH TIME ZONE,
    "MaxScore" DECIMAL(10,2) NOT NULL DEFAULT 100.00,
    "Instructions" TEXT,
    "Attachments" JSONB,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Published, Closed
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_Exercises_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Exercises_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Exercises_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_exercises_teacher" ON "Academic"."Exercises"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_exercises_class" ON "Academic"."Exercises"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_exercises_subject" ON "Academic"."Exercises"("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_exercises_status" ON "Academic"."Exercises"("Status");
CREATE INDEX IF NOT EXISTS "idx_exercises_type" ON "Academic"."Exercises"("Type");
CREATE INDEX IF NOT EXISTS "idx_exercises_duedate" ON "Academic"."Exercises"("DueDate");

-- Examinations Table
CREATE TABLE IF NOT EXISTS "Academic"."Examinations" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ClassId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "Title" VARCHAR(500) NOT NULL,
    "Description" TEXT,
    "Type" VARCHAR(50) NOT NULL, -- Quiz, Test, Exam
    "ScheduledDate" TIMESTAMP WITH TIME ZONE,
    "Duration" INTEGER, -- Duration in minutes
    "MaxScore" DECIMAL(10,2) NOT NULL DEFAULT 100.00,
    "Instructions" TEXT,
    "Questions" JSONB,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Scheduled, InProgress, Completed
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_Examinations_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Examinations_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Examinations_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_examinations_teacher" ON "Academic"."Examinations"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_examinations_class" ON "Academic"."Examinations"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_examinations_subject" ON "Academic"."Examinations"("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_examinations_status" ON "Academic"."Examinations"("Status");
CREATE INDEX IF NOT EXISTS "idx_examinations_type" ON "Academic"."Examinations"("Type");
CREATE INDEX IF NOT EXISTS "idx_examinations_scheduleddate" ON "Academic"."Examinations"("ScheduledDate");

-- Exercise Submissions Table
CREATE TABLE IF NOT EXISTS "Academic"."ExerciseSubmissions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "ExerciseId" BIGINT NOT NULL,
    "StudentId" BIGINT NOT NULL,
    "SubmittedAt" TIMESTAMP WITH TIME ZONE,
    "Content" TEXT,
    "Attachments" JSONB,
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Submitted', -- Submitted, Late, Graded
    "Score" DECIMAL(10,2),
    "Feedback" TEXT,
    "GradedBy" BIGINT,
    "GradedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_ExerciseSubmissions_Exercise" FOREIGN KEY ("ExerciseId") 
        REFERENCES "Academic"."Exercises"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExerciseSubmissions_Student" FOREIGN KEY ("StudentId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExerciseSubmissions_GradedBy" FOREIGN KEY ("GradedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "idx_exercisesubmissions_exercise" ON "Academic"."ExerciseSubmissions"("ExerciseId");
CREATE INDEX IF NOT EXISTS "idx_exercisesubmissions_student" ON "Academic"."ExerciseSubmissions"("StudentId");
CREATE INDEX IF NOT EXISTS "idx_exercisesubmissions_status" ON "Academic"."ExerciseSubmissions"("Status");
CREATE UNIQUE INDEX IF NOT EXISTS "idx_exercisesubmissions_unique" 
    ON "Academic"."ExerciseSubmissions"("ExerciseId", "StudentId") 
    WHERE "IsDeleted" = FALSE;

-- Examination Attempts Table
CREATE TABLE IF NOT EXISTS "Academic"."ExaminationAttempts" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "ExaminationId" BIGINT NOT NULL,
    "StudentId" BIGINT NOT NULL,
    "StartedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "SubmittedAt" TIMESTAMP WITH TIME ZONE,
    "Answers" JSONB,
    "Score" DECIMAL(10,2),
    "Status" VARCHAR(50) NOT NULL DEFAULT 'InProgress', -- InProgress, Submitted, Graded
    "GradedBy" BIGINT,
    "GradedAt" TIMESTAMP WITH TIME ZONE,
    "TimeSpent" INTEGER, -- Time spent in minutes
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_ExaminationAttempts_Examination" FOREIGN KEY ("ExaminationId") 
        REFERENCES "Academic"."Examinations"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExaminationAttempts_Student" FOREIGN KEY ("StudentId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExaminationAttempts_GradedBy" FOREIGN KEY ("GradedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS "idx_examinationattempts_examination" ON "Academic"."ExaminationAttempts"("ExaminationId");
CREATE INDEX IF NOT EXISTS "idx_examinationattempts_student" ON "Academic"."ExaminationAttempts"("StudentId");
CREATE INDEX IF NOT EXISTS "idx_examinationattempts_status" ON "Academic"."ExaminationAttempts"("Status");
CREATE UNIQUE INDEX IF NOT EXISTS "idx_examinationattempts_unique" 
    ON "Academic"."ExaminationAttempts"("ExaminationId", "StudentId") 
    WHERE "IsDeleted" = FALSE AND "Status" != 'InProgress';

-- Grades Table
CREATE TABLE IF NOT EXISTS "Academic"."Grades" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "ClassId" BIGINT NOT NULL,
    "SubjectId" BIGINT NOT NULL,
    "ExerciseId" BIGINT,
    "ExaminationId" BIGINT,
    "Score" DECIMAL(10,2) NOT NULL,
    "MaxScore" DECIMAL(10,2) NOT NULL,
    "Percentage" DECIMAL(5,2) NOT NULL,
    "LetterGrade" VARCHAR(10),
    "Term" VARCHAR(50),
    "Year" INTEGER NOT NULL,
    "GradedBy" BIGINT NOT NULL,
    "GradedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "Status" VARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, PendingApproval, Approved, Rejected
    "ApprovedBy" BIGINT,
    "ApprovedAt" TIMESTAMP WITH TIME ZONE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_Grades_Student" FOREIGN KEY ("StudentId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Grades_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Grades_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Grades_Exercise" FOREIGN KEY ("ExerciseId") 
        REFERENCES "Academic"."Exercises"("ID") ON DELETE SET NULL,
    CONSTRAINT "FK_Grades_Examination" FOREIGN KEY ("ExaminationId") 
        REFERENCES "Academic"."Examinations"("ID") ON DELETE SET NULL,
    CONSTRAINT "FK_Grades_GradedBy" FOREIGN KEY ("GradedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE RESTRICT,
    CONSTRAINT "FK_Grades_ApprovedBy" FOREIGN KEY ("ApprovedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL,
    CONSTRAINT "CHK_Grades_ExerciseOrExamination" CHECK (
        ("ExerciseId" IS NOT NULL AND "ExaminationId" IS NULL) OR 
        ("ExerciseId" IS NULL AND "ExaminationId" IS NOT NULL) OR
        ("ExerciseId" IS NULL AND "ExaminationId" IS NULL)
    )
);

CREATE INDEX IF NOT EXISTS "idx_grades_student" ON "Academic"."Grades"("StudentId");
CREATE INDEX IF NOT EXISTS "idx_grades_class" ON "Academic"."Grades"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_grades_subject" ON "Academic"."Grades"("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_grades_exercise" ON "Academic"."Grades"("ExerciseId");
CREATE INDEX IF NOT EXISTS "idx_grades_examination" ON "Academic"."Grades"("ExaminationId");
CREATE INDEX IF NOT EXISTS "idx_grades_status" ON "Academic"."Grades"("Status");
CREATE INDEX IF NOT EXISTS "idx_grades_term_year" ON "Academic"."Grades"("Term", "Year");

-- Teacher Permissions Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherPermissions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ClassId" BIGINT,
    "SubjectId" BIGINT,
    "CanCreateExercises" BOOLEAN NOT NULL DEFAULT TRUE,
    "CanCreateExaminations" BOOLEAN NOT NULL DEFAULT TRUE,
    "CanGradeOwnClasses" BOOLEAN NOT NULL DEFAULT TRUE,
    "CanGradeAllClasses" BOOLEAN NOT NULL DEFAULT FALSE,
    "CanApproveGrades" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherPermissions_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherPermissions_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherPermissions_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_teacherpermissions_teacher" ON "Teacher"."TeacherPermissions"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacherpermissions_class" ON "Teacher"."TeacherPermissions"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_teacherpermissions_subject" ON "Teacher"."TeacherPermissions"("SubjectId");
CREATE UNIQUE INDEX IF NOT EXISTS "idx_teacherpermissions_unique" 
    ON "Teacher"."TeacherPermissions"("TeacherId", "ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- END OF SCHEMA
-- =============================================
