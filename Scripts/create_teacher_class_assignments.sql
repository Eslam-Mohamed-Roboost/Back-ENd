-- =============================================
-- CREATE TeacherClassAssignments TABLE
-- This script creates the TeacherClassAssignments table if it doesn't exist
-- =============================================

-- Ensure Teacher schema exists
CREATE SCHEMA IF NOT EXISTS "Teacher";

-- Create TeacherClassAssignments Table
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

-- Create Indexes for TeacherClassAssignments
CREATE INDEX IF NOT EXISTS "idx_teacherclass_teacher" ON "Teacher"."TeacherClassAssignments"("TeacherId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_class" ON "Teacher"."TeacherClassAssignments"("ClassId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_subject" ON "Teacher"."TeacherClassAssignments"("SubjectId");
CREATE INDEX IF NOT EXISTS "idx_teacherclass_isdeleted" ON "Teacher"."TeacherClassAssignments"("IsDeleted");

-- Create unique constraint to prevent duplicate assignments
CREATE UNIQUE INDEX IF NOT EXISTS "idx_teacherclass_unique" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- Composite indexes for better query performance
CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_teacher_class_subject" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_class_subject" 
    ON "Teacher"."TeacherClassAssignments"("ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_teacher_subject" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_assignedat" 
    ON "Teacher"."TeacherClassAssignments"("AssignedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Add comment
COMMENT ON TABLE "Teacher"."TeacherClassAssignments" IS 'Links teachers to the classes and subjects they teach';

-- Verify table creation
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.tables 
        WHERE table_schema = 'Teacher' 
        AND table_name = 'TeacherClassAssignments'
    ) THEN
        RAISE NOTICE 'Table Teacher.TeacherClassAssignments created successfully';
    ELSE
        RAISE EXCEPTION 'Failed to create table Teacher.TeacherClassAssignments';
    END IF;
END $$;

