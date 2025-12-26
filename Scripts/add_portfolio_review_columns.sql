-- =============================================
-- Migration Script: Add Review Columns to PortfolioFiles
-- =============================================
-- This script adds the missing review-related columns to the PortfolioFiles table
-- Run this script if you're getting "column p.ReviewedAt does not exist" errors

-- Add Status column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Portfolio' 
        AND table_name = 'PortfolioFiles' 
        AND column_name = 'Status'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles" 
        ADD COLUMN "Status" VARCHAR(20) NOT NULL DEFAULT 'Pending';
    END IF;
END $$;

-- Add ReviewedBy column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Portfolio' 
        AND table_name = 'PortfolioFiles' 
        AND column_name = 'ReviewedBy'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles"
        ADD COLUMN "ReviewedBy" BIGINT;
    END IF;
END $$;

-- Add ReviewedAt column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Portfolio' 
        AND table_name = 'PortfolioFiles' 
        AND column_name = 'ReviewedAt'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles"
        ADD COLUMN "ReviewedAt" TIMESTAMP WITH TIME ZONE;
    END IF;
END $$;

-- Add RevisionNotes column if it doesn't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'Portfolio' 
        AND table_name = 'PortfolioFiles' 
        AND column_name = 'RevisionNotes'
    ) THEN
        ALTER TABLE "Portfolio"."PortfolioFiles"
        ADD COLUMN "RevisionNotes" TEXT;
    END IF;
END $$;

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
            FOREIGN KEY ("ReviewedBy") REFERENCES "Identity"."User"("ID") ON DELETE SET NULL;
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

-- Verify columns were added
SELECT 
    column_name, 
    data_type, 
    is_nullable,
    column_default
FROM information_schema.columns 
WHERE table_schema = 'Portfolio' 
    AND table_name = 'PortfolioFiles'
    AND column_name IN ('Status', 'ReviewedBy', 'ReviewedAt', 'RevisionNotes')
ORDER BY column_name;

