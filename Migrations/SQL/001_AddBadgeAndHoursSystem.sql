-- ============================================
-- Migration: Add Badge and Hours System
-- Description: Adds LearningHours table and updates Missions/Challenges tables
-- Date: 2025-12-25
-- ============================================

-- ============================================
-- PART 1: CREATE NEW LEARNINGHOURS TABLE
-- ============================================

-- Create LearningHours table in General schema
CREATE TABLE IF NOT EXISTS "General"."LearningHours" (
    "ID" bigint NOT NULL,
    "CompanyID" bigint NOT NULL,
    "StudentId" bigint NOT NULL,
    "ActivityType" integer NOT NULL,
    "ActivityId" bigint NOT NULL,
    "HoursEarned" decimal(5,2) NOT NULL,
    "ActivityDate" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL DEFAULT NOW(),
    "UpdatedAt" timestamp with time zone NULL,
    "IsDeleted" boolean NOT NULL DEFAULT false,
    
    CONSTRAINT "PK_LearningHours" PRIMARY KEY ("ID")
);

-- Create indexes for better query performance
CREATE INDEX IF NOT EXISTS "IX_LearningHours_StudentId" 
    ON "General"."LearningHours"("StudentId") 
    WHERE "IsDeleted" = false;

CREATE INDEX IF NOT EXISTS "IX_LearningHours_ActivityDate" 
    ON "General"."LearningHours"("ActivityDate") 
    WHERE "IsDeleted" = false;

CREATE INDEX IF NOT EXISTS "IX_LearningHours_CompanyID" 
    ON "General"."LearningHours"("CompanyID") 
    WHERE "IsDeleted" = false;

CREATE INDEX IF NOT EXISTS "IX_LearningHours_ActivityType_ActivityId" 
    ON "General"."LearningHours"("ActivityType", "ActivityId") 
    WHERE "IsDeleted" = false;

-- Add comments to table and columns
COMMENT ON TABLE "General"."LearningHours" IS 'Tracks learning hours earned by students for completing activities';
COMMENT ON COLUMN "General"."LearningHours"."ID" IS 'Primary key';
COMMENT ON COLUMN "General"."LearningHours"."CompanyID" IS 'Company identifier';
COMMENT ON COLUMN "General"."LearningHours"."StudentId" IS 'Student who earned the hours';
COMMENT ON COLUMN "General"."LearningHours"."ActivityType" IS 'Type of activity: 1=Login, 2=Badge, 3=Upload, 4=Completion, 5=Update, 6=Delete, 7=Create';
COMMENT ON COLUMN "General"."LearningHours"."ActivityId" IS 'ID of the activity (Mission, Challenge, etc.)';
COMMENT ON COLUMN "General"."LearningHours"."HoursEarned" IS 'Number of learning hours earned (max 999.99)';
COMMENT ON COLUMN "General"."LearningHours"."ActivityDate" IS 'When the hours were earned';

-- ============================================
-- PART 2: UPDATE MISSIONS TABLE
-- ============================================

-- Add HoursAwarded column to Missions table if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'Missions' 
        AND table_name = 'Missions' 
        AND column_name = 'HoursAwarded'
    ) THEN
        ALTER TABLE "Missions"."Missions" 
        ADD COLUMN "HoursAwarded" decimal(5,2) NOT NULL DEFAULT 1.5;
        
        COMMENT ON COLUMN "Missions"."Missions"."HoursAwarded" IS 'Learning hours awarded upon completion';
    END IF;
END $$;

-- Update existing missions with appropriate hours based on mission number
UPDATE "Missions"."Missions"
SET "HoursAwarded" = CASE 
    WHEN "Number" BETWEEN 1 AND 4 THEN 1.5
    WHEN "Number" BETWEEN 5 AND 8 THEN 2.0
    ELSE 1.5
END
WHERE "HoursAwarded" = 1.5; -- Only update default values

-- ============================================
-- PART 3: UPDATE CHALLENGES TABLE
-- ============================================

-- Add BadgeId column to Challenges table if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'Gamification' 
        AND table_name = 'Challenges' 
        AND column_name = 'BadgeId'
    ) THEN
        ALTER TABLE "Gamification"."Challenges" 
        ADD COLUMN "BadgeId" bigint NULL;
        
        COMMENT ON COLUMN "Gamification"."Challenges"."BadgeId" IS 'Optional badge awarded upon completion';
    END IF;
END $$;

-- Add HoursAwarded column to Challenges table if it doesn't exist
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 
        FROM information_schema.columns 
        WHERE table_schema = 'Gamification' 
        AND table_name = 'Challenges' 
        AND column_name = 'HoursAwarded'
    ) THEN
        ALTER TABLE "Gamification"."Challenges" 
        ADD COLUMN "HoursAwarded" decimal(5,2) NOT NULL DEFAULT 0.5;
        
        COMMENT ON COLUMN "Gamification"."Challenges"."HoursAwarded" IS 'Learning hours awarded upon completion';
    END IF;
END $$;

-- Update existing challenges with hours based on difficulty
UPDATE "Gamification"."Challenges"
SET "HoursAwarded" = CASE 
    WHEN "Difficulty" = 0 THEN 0.5  -- Easy
    WHEN "Difficulty" = 1 THEN 1.0  -- Medium
    WHEN "Difficulty" = 2 THEN 1.5  -- Hard
    ELSE 0.5
END
WHERE "HoursAwarded" = 0.5; -- Only update default values

-- ============================================
-- PART 4: CREATE STATISTICS VIEW (OPTIONAL)
-- ============================================

-- Create a view for student learning hours statistics
CREATE OR REPLACE VIEW "General"."V_StudentLearningHoursStats" AS
SELECT 
    lh."StudentId",
    lh."CompanyID",
    SUM(lh."HoursEarned") as "TotalHours",
    COUNT(*) as "TotalActivities",
    SUM(CASE WHEN lh."ActivityDate" >= DATE_TRUNC('week', NOW()) THEN lh."HoursEarned" ELSE 0 END) as "ThisWeekHours",
    SUM(CASE WHEN lh."ActivityDate" >= DATE_TRUNC('month', NOW()) THEN lh."HoursEarned" ELSE 0 END) as "ThisMonthHours",
    SUM(CASE WHEN lh."ActivityDate" >= DATE_TRUNC('year', NOW()) THEN lh."HoursEarned" ELSE 0 END) as "ThisYearHours",
    MAX(lh."ActivityDate") as "LastActivityDate"
FROM "General"."LearningHours" lh
WHERE lh."IsDeleted" = false
GROUP BY lh."StudentId", lh."CompanyID";

COMMENT ON VIEW "General"."V_StudentLearningHoursStats" IS 'Aggregated learning hours statistics per student';

-- ============================================
-- PART 5: SEED DEFAULT DATA (OPTIONAL)
-- ============================================

-- Note: Uncomment and modify these sections if you want to seed initial data

/*
-- Example: Update all missions with specific hours
UPDATE "Missions"."Missions" SET "HoursAwarded" = 1.5 WHERE "Number" = 1;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 1.5 WHERE "Number" = 2;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 1.5 WHERE "Number" = 3;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 1.5 WHERE "Number" = 4;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 2.0 WHERE "Number" = 5;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 2.0 WHERE "Number" = 6;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 2.0 WHERE "Number" = 7;
UPDATE "Missions"."Missions" SET "HoursAwarded" = 2.0 WHERE "Number" = 8;
*/

-- ============================================
-- VERIFICATION QUERIES
-- ============================================

-- Verify LearningHours table was created
SELECT 
    table_schema, 
    table_name, 
    column_name, 
    data_type 
FROM information_schema.columns 
WHERE table_schema = 'General' 
AND table_name = 'LearningHours'
ORDER BY ordinal_position;

-- Verify Missions table was updated
SELECT 
    column_name, 
    data_type, 
    column_default 
FROM information_schema.columns 
WHERE table_schema = 'Missions' 
AND table_name = 'Missions' 
AND column_name = 'HoursAwarded';

-- Verify Challenges table was updated
SELECT 
    column_name, 
    data_type, 
    column_default 
FROM information_schema.columns 
WHERE table_schema = 'Gamification' 
AND table_name = 'Challenges' 
AND column_name IN ('BadgeId', 'HoursAwarded');

-- Check indexes were created
SELECT 
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname = 'General'
AND tablename = 'LearningHours';

-- ============================================
-- ROLLBACK SCRIPT (IF NEEDED)
-- ============================================
-- Uncomment to rollback changes

/*
-- Drop view
DROP VIEW IF EXISTS "General"."V_StudentLearningHoursStats";

-- Drop LearningHours table
DROP TABLE IF EXISTS "General"."LearningHours";

-- Remove columns from Missions
ALTER TABLE "Missions"."Missions" DROP COLUMN IF EXISTS "HoursAwarded";

-- Remove columns from Challenges
ALTER TABLE "Gamification"."Challenges" DROP COLUMN IF EXISTS "BadgeId";
ALTER TABLE "Gamification"."Challenges" DROP COLUMN IF EXISTS "HoursAwarded";
*/

-- ============================================
-- END OF MIGRATION
-- ============================================

SELECT 'Migration completed successfully!' as status;

