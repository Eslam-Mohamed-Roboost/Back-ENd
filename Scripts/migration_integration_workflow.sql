-- =============================================
-- INTEGRATION WORKFLOW MIGRATION
-- Adds tables and columns for complete system integration
-- =============================================

BEGIN;

-- =============================================
-- 1. CREATE TeacherClassAssignments TABLE
-- =============================================
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
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherClassAssignments_Class" FOREIGN KEY ("ClassId") 
        REFERENCES "General"."Classes"("ID") ON DELETE CASCADE,
    CONSTRAINT "FK_TeacherClassAssignments_Subject" FOREIGN KEY ("SubjectId") 
        REFERENCES "General"."Subjects"("ID") ON DELETE CASCADE
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

-- Add comment
COMMENT ON TABLE "Teacher"."TeacherClassAssignments" IS 'Links teachers to the classes and subjects they teach';

-- =============================================
-- 2. MODIFY PortfolioFiles TABLE
-- =============================================

-- Add Status column to PortfolioFiles
ALTER TABLE "Portfolio"."PortfolioFiles" 
ADD COLUMN IF NOT EXISTS "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending';

-- Add ReviewedBy foreign key
ALTER TABLE "Portfolio"."PortfolioFiles"
ADD COLUMN IF NOT EXISTS "ReviewedBy" BIGINT;

-- Add ReviewedAt timestamp
ALTER TABLE "Portfolio"."PortfolioFiles"
ADD COLUMN IF NOT EXISTS "ReviewedAt" TIMESTAMP WITH TIME ZONE;

-- Add RevisionNotes for teacher feedback
ALTER TABLE "Portfolio"."PortfolioFiles"
ADD COLUMN IF NOT EXISTS "RevisionNotes" TEXT;

-- Add foreign key constraint if not exists
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_PortfolioFiles_ReviewedBy'
        AND table_schema = 'Portfolio'
        AND table_name = 'PortfolioFiles'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles"
        ADD CONSTRAINT "FK_PortfolioFiles_ReviewedBy" 
            FOREIGN KEY ("ReviewedBy") REFERENCES "Identity"."User"("ID");
    END IF;
END $$;

-- Create index on Status for filtering
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_status" ON "Portfolio"."PortfolioFiles"("Status");

-- Create index on ReviewedBy
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_reviewedby" ON "Portfolio"."PortfolioFiles"("ReviewedBy");

-- Add check constraint for valid status values
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'chk_portfoliofiles_status'
        AND table_schema = 'Portfolio'
        AND table_name = 'PortfolioFiles'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles"
        ADD CONSTRAINT "chk_portfoliofiles_status" 
        CHECK ("Status" IN ('Pending', 'Reviewed', 'NeedsRevision'));
    END IF;
END $$;

-- Add comments
COMMENT ON COLUMN "Portfolio"."PortfolioFiles"."Status" IS 'Portfolio review status: Pending, Reviewed, NeedsRevision';
COMMENT ON COLUMN "Portfolio"."PortfolioFiles"."ReviewedBy" IS 'Teacher ID who reviewed this portfolio file';
COMMENT ON COLUMN "Portfolio"."PortfolioFiles"."ReviewedAt" IS 'Timestamp when portfolio was reviewed';
COMMENT ON COLUMN "Portfolio"."PortfolioFiles"."RevisionNotes" IS 'Teacher feedback and revision requests';

-- =============================================
-- 3. VERIFY User.ClassID EXISTS
-- =============================================

-- Add ClassID column if it doesn't exist
ALTER TABLE "Identity"."User" 
ADD COLUMN IF NOT EXISTS "ClassID" BIGINT;

-- Add foreign key constraint if not exists
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'FK_User_Class'
        AND table_schema = 'Identity'
        AND table_name = 'User'
    ) THEN
        ALTER TABLE "Identity"."User"
        ADD CONSTRAINT "FK_User_Class" 
            FOREIGN KEY ("ClassID") REFERENCES "General"."Classes"("ID");
    END IF;
END $$;

-- Create index on ClassID
CREATE INDEX IF NOT EXISTS "idx_user_classid" ON "Identity"."User"("ClassID");

-- Add comment
COMMENT ON COLUMN "Identity"."User"."ClassID" IS 'Class assignment for students (NULL for teachers and admins)';

-- =============================================
-- 4. CREATE Notifications TABLE (Optional - Phase 7)
-- =============================================

CREATE TABLE IF NOT EXISTS "System"."Notifications" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "UserId" BIGINT NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Message" TEXT NOT NULL,
    "Type" VARCHAR(20) NOT NULL DEFAULT 'Info',
    "ActionLink" TEXT,
    "IsRead" BOOLEAN NOT NULL DEFAULT FALSE,
    "ReadAt" TIMESTAMP WITH TIME ZONE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_Notifications_User" FOREIGN KEY ("UserId") 
        REFERENCES "Identity"."User"("ID") ON DELETE CASCADE
);

-- Create Indexes for Notifications
CREATE INDEX IF NOT EXISTS "idx_notifications_userid" ON "System"."Notifications"("UserId");
CREATE INDEX IF NOT EXISTS "idx_notifications_isread" ON "System"."Notifications"("IsRead");
CREATE INDEX IF NOT EXISTS "idx_notifications_createdat" ON "System"."Notifications"("CreatedAt" DESC);
CREATE INDEX IF NOT EXISTS "idx_notifications_user_unread" 
    ON "System"."Notifications"("UserId", "IsRead") 
    WHERE "IsRead" = FALSE AND "IsDeleted" = FALSE;

-- Add check constraint for valid types
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.table_constraints 
        WHERE constraint_name = 'chk_notifications_type'
        AND table_schema = 'System'
        AND table_name = 'Notifications'
    ) THEN
        ALTER TABLE "System"."Notifications"
        ADD CONSTRAINT "chk_notifications_type" 
        CHECK ("Type" IN ('Info', 'Warning', 'Success', 'Error'));
    END IF;
END $$;

-- Add comment
COMMENT ON TABLE "System"."Notifications" IS 'User notifications for portfolio reviews, badge approvals, mission updates, etc.';

COMMIT;

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Verify migration success
SELECT 
    'TeacherClassAssignments' as table_name,
    COUNT(*) as column_count
FROM information_schema.columns 
WHERE table_schema = 'Teacher' AND table_name = 'TeacherClassAssignments'
UNION ALL
SELECT 
    'PortfolioFiles (Status column)',
    COUNT(*)
FROM information_schema.columns 
WHERE table_schema = 'Portfolio' AND table_name = 'PortfolioFiles' AND column_name = 'Status'
UNION ALL
SELECT 
    'User (ClassID column)',
    COUNT(*)
FROM information_schema.columns 
WHERE table_schema = 'Identity' AND table_name = 'User' AND column_name = 'ClassID'
UNION ALL
SELECT 
    'Notifications',
    COUNT(*)
FROM information_schema.columns 
WHERE table_schema = 'System' AND table_name = 'Notifications';

