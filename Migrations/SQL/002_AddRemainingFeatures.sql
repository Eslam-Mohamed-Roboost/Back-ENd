-- ============================================================================
-- Migration: 002_AddRemainingFeatures
-- Description: Adds Exercise system, Mission prerequisites/deadlines
-- Date: 2025-12-25
-- ============================================================================

-- ============================================================================
-- 1. CREATE EXERCISES SCHEMA AND TABLES
-- ============================================================================

CREATE SCHEMA IF NOT EXISTS "Exercises";

CREATE TABLE IF NOT EXISTS "Exercises"."Exercises" (
    "ID" BIGSERIAL PRIMARY KEY,
    "Title" VARCHAR(250) NOT NULL,
    "Description" TEXT,
    "Type" INT NOT NULL DEFAULT 1,
    "TeacherId" BIGINT NOT NULL,
    "SubjectId" BIGINT,
    "ClassId" BIGINT,
    "DueDate" TIMESTAMP,
    "MaxPoints" INT NOT NULL DEFAULT 100,
    "Instructions" TEXT,
    "RubricJson" JSONB,
    "AllowLateSubmission" BOOLEAN DEFAULT FALSE,
    "LatePenaltyPercentage" NUMERIC(18,2),
    "AssignedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "IsActive" BOOLEAN DEFAULT TRUE,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP,
    CONSTRAINT "FK_Exercises_Teacher" FOREIGN KEY ("TeacherId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_Exercises_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE SET NULL,
    CONSTRAINT "FK_Exercises_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS "Exercises"."ExerciseSubmissions" (
    "ID" BIGSERIAL PRIMARY KEY,
    "ExerciseId" BIGINT NOT NULL,
    "StudentId" BIGINT NOT NULL,
    "SubmissionText" TEXT,
    "FileUrl" VARCHAR(500),
    "SubmittedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "IsLate" BOOLEAN DEFAULT FALSE,
    "Status" INT NOT NULL DEFAULT 1,
    "Grade" NUMERIC(18,2),
    "Feedback" TEXT,
    "GradedBy" BIGINT,
    "GradedAt" TIMESTAMP,
    "CreatedAt" TIMESTAMP NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP,
    CONSTRAINT "FK_ExerciseSubmissions_Exercise" FOREIGN KEY ("ExerciseId") 
        REFERENCES "Exercises"."Exercises"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExerciseSubmissions_Student" FOREIGN KEY ("StudentId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_ExerciseSubmissions_GradedBy" FOREIGN KEY ("GradedBy") 
        REFERENCES "Identity"."User"("ID") ON DELETE SET NULL,
    CONSTRAINT "UQ_ExerciseSubmissions_Student_Exercise" UNIQUE ("ExerciseId", "StudentId")
);

-- ============================================================================
-- 2. CREATE INDEXES FOR EXERCISES
-- ============================================================================

CREATE INDEX IF NOT EXISTS "idx_exercises_teacher_id" 
    ON "Exercises"."Exercises"("TeacherId");

CREATE INDEX IF NOT EXISTS "idx_exercises_class_id" 
    ON "Exercises"."Exercises"("ClassId");

CREATE INDEX IF NOT EXISTS "idx_exercises_subject_id" 
    ON "Exercises"."Exercises"("SubjectId");

CREATE INDEX IF NOT EXISTS "idx_exercises_due_date" 
    ON "Exercises"."Exercises"("DueDate");

CREATE INDEX IF NOT EXISTS "idx_submissions_exercise_id" 
    ON "Exercises"."ExerciseSubmissions"("ExerciseId");

CREATE INDEX IF NOT EXISTS "idx_submissions_student_id" 
    ON "Exercises"."ExerciseSubmissions"("StudentId");

CREATE INDEX IF NOT EXISTS "idx_submissions_status" 
    ON "Exercises"."ExerciseSubmissions"("Status");

-- ============================================================================
-- 3. ADD MISSION PREREQUISITES AND DEADLINES
-- ============================================================================

-- Add Deadline column to Missions table
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Missions' 
        AND table_name = 'Missions' 
        AND column_name = 'Deadline'
    ) THEN
        ALTER TABLE "Missions"."Missions" 
        ADD COLUMN "Deadline" TIMESTAMP;
    END IF;
END $$;

-- Add PrerequisiteMissionIds column to Missions table
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Missions' 
        AND table_name = 'Missions' 
        AND column_name = 'PrerequisiteMissionIds'
    ) THEN
        ALTER TABLE "Missions"."Missions" 
        ADD COLUMN "PrerequisiteMissionIds" TEXT;
    END IF;
END $$;

-- Create index on Deadline
CREATE INDEX IF NOT EXISTS "idx_missions_deadline" 
    ON "Missions"."Missions"("Deadline");

-- ============================================================================
-- 4. NOTIFICATION TYPES FOR NEW FEATURES
-- ============================================================================

-- Note: Notification types are handled in code via NotificationService
-- No database changes needed for notification types

-- ============================================================================
-- 5. VIEWS FOR REPORTING
-- ============================================================================

-- Exercise Statistics View
CREATE OR REPLACE VIEW "Exercises"."V_ExerciseStatistics" AS
SELECT 
    e."ID" AS "ExerciseId",
    e."Title",
    e."TeacherId",
    e."ClassId",
    COUNT(es."ID") AS "TotalSubmissions",
    COUNT(CASE WHEN es."Status" = 3 THEN 1 END) AS "GradedSubmissions",
    COUNT(CASE WHEN es."IsLate" = TRUE THEN 1 END) AS "LateSubmissions",
    AVG(es."Grade") AS "AverageGrade",
    MAX(es."Grade") AS "HighestGrade",
    MIN(es."Grade") AS "LowestGrade"
FROM "Exercises"."Exercises" e
LEFT JOIN "Exercises"."ExerciseSubmissions" es ON e."ID" = es."ExerciseId"
GROUP BY e."ID", e."Title", e."TeacherId", e."ClassId";

-- Student Exercise Progress View
CREATE OR REPLACE VIEW "Exercises"."V_StudentExerciseProgress" AS
SELECT 
    u."ID" AS "StudentId",
    u."Name" AS "StudentName",
    u."ClassID",
    COUNT(DISTINCT e."ID") AS "TotalExercises",
    COUNT(DISTINCT es."ID") AS "SubmittedExercises",
    COUNT(DISTINCT CASE WHEN es."Status" = 3 THEN es."ID" END) AS "GradedExercises",
    AVG(es."Grade") AS "AverageGrade",
    COUNT(DISTINCT CASE WHEN es."IsLate" = TRUE THEN es."ID" END) AS "LateSubmissions"
FROM "Identity"."User" u
LEFT JOIN "Exercises"."Exercises" e ON u."ClassID" = e."ClassId"
LEFT JOIN "Exercises"."ExerciseSubmissions" es ON e."ID" = es."ExerciseId" AND u."ID" = es."StudentId"
WHERE u."Role" = 2 -- Student role
GROUP BY u."ID", u."Name", u."ClassID";

-- ============================================================================
-- 6. GRANT PERMISSIONS (if needed)
-- ============================================================================

-- Grant permissions to application user (adjust username as needed)
-- GRANT ALL PRIVILEGES ON SCHEMA "Exercises" TO your_app_user;
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA "Exercises" TO your_app_user;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA "Exercises" TO your_app_user;

-- ============================================================================
-- MIGRATION COMPLETE
-- ============================================================================

-- Verify tables created
DO $$
DECLARE
    v_exercises_count INT;
    v_submissions_count INT;
BEGIN
    SELECT COUNT(*) INTO v_exercises_count 
    FROM information_schema.tables 
    WHERE table_schema = 'Exercises' AND table_name = 'Exercises';
    
    SELECT COUNT(*) INTO v_submissions_count 
    FROM information_schema.tables 
    WHERE table_schema = 'Exercises' AND table_name = 'ExerciseSubmissions';
    
    IF v_exercises_count > 0 AND v_submissions_count > 0 THEN
        RAISE NOTICE '✅ Migration 002 completed successfully!';
        RAISE NOTICE '   - Exercises schema created';
        RAISE NOTICE '   - 2 tables created';
        RAISE NOTICE '   - 8 indexes created';
        RAISE NOTICE '   - 2 views created';
        RAISE NOTICE '   - Mission prerequisites/deadlines added';
    ELSE
        RAISE EXCEPTION '❌ Migration 002 failed - tables not created';
    END IF;
END $$;

