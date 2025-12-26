-- =============================================
-- Student Attendance Tables
-- PostgreSQL 14+
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

