-- =============================================
-- Teacher Missions and Student Attendance Tables
-- PostgreSQL 14+
-- Run this script to create all new tables for Teacher Missions and Attendance features
-- =============================================

-- Ensure schemas exist
CREATE SCHEMA IF NOT EXISTS "Teacher";
CREATE SCHEMA IF NOT EXISTS "Academic";

-- =============================================
-- TEACHER MISSIONS TABLES
-- =============================================

-- Teacher Missions Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherMissions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "Number" INTEGER NOT NULL,
    "Title" VARCHAR(500) NOT NULL,
    "Description" TEXT,
    "Icon" VARCHAR(100),
    "EstimatedMinutes" INTEGER NOT NULL DEFAULT 30,
    "BadgeId" BIGINT NOT NULL,
    "Order" INTEGER NOT NULL DEFAULT 0,
    "IsEnabled" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherMissions_Badge" FOREIGN KEY ("BadgeId") 
        REFERENCES "Gamification"."Badges"("ID") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS "idx_teachermissions_badge" ON "Teacher"."TeacherMissions"("BadgeId");
CREATE INDEX IF NOT EXISTS "idx_teachermissions_enabled" ON "Teacher"."TeacherMissions"("IsEnabled");
CREATE INDEX IF NOT EXISTS "idx_teachermissions_order" ON "Teacher"."TeacherMissions"("Order");

-- Teacher Activities Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherActivities" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "MissionId" BIGINT NOT NULL,
    "Number" INTEGER NOT NULL,
    "Title" VARCHAR(500) NOT NULL,
    "Type" INTEGER NOT NULL, -- ActivityType enum
    "ContentUrl" VARCHAR(500),
    "EstimatedMinutes" INTEGER NOT NULL DEFAULT 10,
    "Instructions" TEXT,
    "Order" INTEGER NOT NULL DEFAULT 0,
    "IsRequired" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherActivities_Mission" FOREIGN KEY ("MissionId") 
        REFERENCES "Teacher"."TeacherMissions"("ID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_teacheractivities_mission" ON "Teacher"."TeacherActivities"("MissionId");
CREATE INDEX IF NOT EXISTS "idx_teacheractivities_order" ON "Teacher"."TeacherActivities"("Order");

-- Teacher Mission Progress Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherMissionProgress" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "MissionId" BIGINT NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1, -- ProgressStatus enum: 1=NotStarted, 2=InProgress, 3=Completed
    "CompletedActivities" INTEGER NOT NULL DEFAULT 0,
    "TotalActivities" INTEGER NOT NULL DEFAULT 0,
    "ProgressPercentage" DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    "StartedAt" TIMESTAMP WITH TIME ZONE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherMissionProgress_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherMissionProgress_Mission" FOREIGN KEY ("MissionId") 
        REFERENCES "Teacher"."TeacherMissions"("ID") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_teachermissionprogress_unique" 
    ON "Teacher"."TeacherMissionProgress"("TeacherId", "MissionId") 
    WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_teachermissionprogress_teacher" ON "Teacher"."TeacherMissionProgress"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teachermissionprogress_mission" ON "Teacher"."TeacherMissionProgress"("MissionId");
CREATE INDEX IF NOT EXISTS "idx_teachermissionprogress_status" ON "Teacher"."TeacherMissionProgress"("Status");

-- Teacher Activity Progress Table
CREATE TABLE IF NOT EXISTS "Teacher"."TeacherActivityProgress" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "TeacherId" BIGINT NOT NULL,
    "ActivityId" BIGINT NOT NULL,
    "MissionId" BIGINT,
    "IsCompleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "CompletedAt" TIMESTAMP WITH TIME ZONE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_TeacherActivityProgress_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherActivityProgress_Activity" FOREIGN KEY ("ActivityId") 
        REFERENCES "Teacher"."TeacherActivities"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherActivityProgress_Mission" FOREIGN KEY ("MissionId") 
        REFERENCES "Teacher"."TeacherMissions"("ID") ON DELETE CASCADE
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_teacheractivityprogress_unique" 
    ON "Teacher"."TeacherActivityProgress"("TeacherId", "ActivityId") 
    WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_teacheractivityprogress_teacher" ON "Teacher"."TeacherActivityProgress"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacheractivityprogress_activity" ON "Teacher"."TeacherActivityProgress"("ActivityId");
CREATE INDEX IF NOT EXISTS "idx_teacheractivityprogress_mission" ON "Teacher"."TeacherActivityProgress"("MissionId");

-- =============================================
-- STUDENT ATTENDANCE TABLES
-- =============================================

-- Student Attendance Table
CREATE TABLE IF NOT EXISTS "Academic"."StudentAttendance" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "StudentId" BIGINT NOT NULL,
    "ClassId" BIGINT NOT NULL,
    "AttendanceDate" DATE NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 1, -- AttendanceStatus enum: 1=Present, 2=Absent, 3=Late, 4=Excused
    "MarkedBy" BIGINT, -- TeacherId who marked the attendance
    "MarkedAt" TIMESTAMP WITH TIME ZONE,
    "IsAutomatic" BOOLEAN NOT NULL DEFAULT FALSE,
    "Notes" TEXT,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_StudentAttendance_Student" FOREIGN KEY ("StudentId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_StudentAttendance_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_StudentAttendance_MarkedBy" FOREIGN KEY ("MarkedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS "idx_studentattendance_unique" 
    ON "Academic"."StudentAttendance"("StudentId", "ClassId", "AttendanceDate") 
    WHERE "IsDeleted" = FALSE;
CREATE INDEX IF NOT EXISTS "idx_studentattendance_student" ON "Academic"."StudentAttendance"("StudentId");
CREATE INDEX IF NOT EXISTS "idx_studentattendance_class" ON "Academic"."StudentAttendance"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_studentattendance_date" ON "Academic"."StudentAttendance"("AttendanceDate");
CREATE INDEX IF NOT EXISTS "idx_studentattendance_status" ON "Academic"."StudentAttendance"("Status");
CREATE INDEX IF NOT EXISTS "idx_studentattendance_markedby" ON "Academic"."StudentAttendance"("MarkedBy");

-- =============================================
-- END OF SCRIPT
-- =============================================

