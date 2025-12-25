-- =============================================
-- PERFORMANCE INDEX OPTIMIZATION SCRIPT
-- Adds missing indexes for optimal query performance
-- Based on query pattern analysis
-- =============================================

BEGIN;

-- =============================================
-- IDENTITY SCHEMA - User Table
-- =============================================

-- Composite index for Role + IsActive filtering (common in user queries)
CREATE INDEX IF NOT EXISTS "idx_user_role_isactive" 
    ON "Identity"."User"("Role", "IsActive") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for Role + IsActive + ClassID (for student filtering)
CREATE INDEX IF NOT EXISTS "idx_user_role_isactive_classid" 
    ON "Identity"."User"("Role", "IsActive", "ClassID") 
    WHERE "IsDeleted" = FALSE AND "Role" = 2; -- ApplicationRole.Student = 2

-- Index for IsActive filtering (used in many queries)
CREATE INDEX IF NOT EXISTS "idx_user_isactive" 
    ON "Identity"."User"("IsActive") 
    WHERE "IsDeleted" = FALSE;

-- Index for Name text search (used in search queries)
CREATE INDEX IF NOT EXISTS "idx_user_name_trgm" 
    ON "Identity"."User" USING gin("Name" gin_trgm_ops) 
    WHERE "IsDeleted" = FALSE;

-- Index for Email text search (used in search queries)
-- Note: Email already has unique index, but we need trigram for contains searches
CREATE INDEX IF NOT EXISTS "idx_user_email_trgm" 
    ON "Identity"."User" USING gin("Email" gin_trgm_ops) 
    WHERE "IsDeleted" = FALSE;

-- Index for UserName text search (used in search queries)
CREATE INDEX IF NOT EXISTS "idx_user_username_trgm" 
    ON "Identity"."User" USING gin("UserName" gin_trgm_ops) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for ClassID + Role (for class student queries)
CREATE INDEX IF NOT EXISTS "idx_user_classid_role" 
    ON "Identity"."User"("ClassID", "Role") 
    WHERE "IsDeleted" = FALSE AND "ClassID" IS NOT NULL;

-- Index for CreatedAt sorting (used in many queries)
CREATE INDEX IF NOT EXISTS "idx_user_createdat_desc" 
    ON "Identity"."User"("CreatedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Index for LastLogin (used in user list queries)
CREATE INDEX IF NOT EXISTS "idx_user_lastlogin" 
    ON "Identity"."User"("LastLogin" DESC NULLS LAST) 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- GENERAL SCHEMA - Classes Table
-- =============================================

-- Composite index for Grade + Name (used in ordering)
CREATE INDEX IF NOT EXISTS "idx_classes_grade_name" 
    ON "General"."Classes"("Grade", "Name") 
    WHERE "IsDeleted" = FALSE;

-- Index for IsDeleted filtering (used in dropdown queries)
CREATE INDEX IF NOT EXISTS "idx_classes_isdeleted" 
    ON "General"."Classes"("IsDeleted") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for TeacherId + IsDeleted
CREATE INDEX IF NOT EXISTS "idx_classes_teacherid_isdeleted" 
    ON "General"."Classes"("TeacherId", "IsDeleted") 
    WHERE "IsDeleted" = FALSE AND "TeacherId" IS NOT NULL;

-- =============================================
-- GENERAL SCHEMA - Subjects Table
-- =============================================

-- Index for IsActive filtering
CREATE INDEX IF NOT EXISTS "idx_subjects_isactive" 
    ON "General"."Subjects"("IsActive") 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- MISSIONS SCHEMA
-- =============================================

-- Index for MissionId in StudentMissionProgress (missing)
CREATE INDEX IF NOT EXISTS "idx_studentmissionprogress_missionid" 
    ON "Missions"."StudentMissionProgress"("MissionId") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for StudentId + MissionId + Status (common query pattern)
CREATE INDEX IF NOT EXISTS "idx_studentmissionprogress_student_mission_status" 
    ON "Missions"."StudentMissionProgress"("StudentId", "MissionId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for Status + CompletedAt (for progress queries)
CREATE INDEX IF NOT EXISTS "idx_studentmissionprogress_status_completed" 
    ON "Missions"."StudentMissionProgress"("Status", "CompletedAt" DESC NULLS LAST) 
    WHERE "IsDeleted" = FALSE;

-- Index for MissionId in StudentActivityProgress
CREATE INDEX IF NOT EXISTS "idx_studentactivityprogress_missionid" 
    ON "Missions"."StudentActivityProgress"("ActivityId") 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- GAMIFICATION SCHEMA
-- =============================================

-- Index for BadgeId in StudentBadges (missing)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_badgeid" 
    ON "Gamification"."StudentBadges"("BadgeId") 
    WHERE "IsDeleted" = FALSE;

-- Index for Status in StudentBadges (for pending badges query)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_status" 
    ON "Gamification"."StudentBadges"("Status") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for StudentId + BadgeId + Status (common query pattern)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_student_badge_status" 
    ON "Gamification"."StudentBadges"("StudentId", "BadgeId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Index for EarnedDate sorting (used in badge queries)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_earneddate_desc" 
    ON "Gamification"."StudentBadges"("EarnedDate" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for BadgeId + Status (for pending badges by badge)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_badge_status" 
    ON "Gamification"."StudentBadges"("BadgeId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Index for MissionId in StudentBadges (if used)
CREATE INDEX IF NOT EXISTS "idx_studentbadges_missionid" 
    ON "Gamification"."StudentBadges"("MissionId") 
    WHERE "IsDeleted" = FALSE AND "MissionId" IS NOT NULL;

-- =============================================
-- PORTFOLIO SCHEMA
-- =============================================

-- Composite index for StudentId + SubjectId (common query pattern)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_student_subject" 
    ON "Portfolio"."PortfolioFiles"("StudentId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for StudentId + Status (for portfolio status queries)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_student_status" 
    ON "Portfolio"."PortfolioFiles"("StudentId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for SubjectId + Status (for subject portfolio queries)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_subject_status" 
    ON "Portfolio"."PortfolioFiles"("SubjectId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for Status + UploadedAt (for sorting by upload date)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_status_uploaded" 
    ON "Portfolio"."PortfolioFiles"("Status", "UploadedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for ReviewedBy + Status (for teacher review queries)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_reviewedby_status" 
    ON "Portfolio"."PortfolioFiles"("ReviewedBy", "Status") 
    WHERE "IsDeleted" = FALSE AND "ReviewedBy" IS NOT NULL;

-- Index for UploadedAt sorting (used in many queries)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_uploadedat_desc" 
    ON "Portfolio"."PortfolioFiles"("UploadedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for StudentId + SubjectId + Status (for detailed portfolio queries)
CREATE INDEX IF NOT EXISTS "idx_portfoliofiles_student_subject_status" 
    ON "Portfolio"."PortfolioFiles"("StudentId", "SubjectId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- TEACHER SCHEMA
-- =============================================

-- Composite index for TeacherId + ClassId + SubjectId (for assignment queries)
CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_teacher_class_subject" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for ClassId + SubjectId (for class-subject queries)
CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_class_subject" 
    ON "Teacher"."TeacherClassAssignments"("ClassId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for TeacherId + SubjectId (for teacher-subject queries)
CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_teacher_subject" 
    ON "Teacher"."TeacherClassAssignments"("TeacherId", "SubjectId") 
    WHERE "IsDeleted" = FALSE;

-- Index for AssignedAt sorting
CREATE INDEX IF NOT EXISTS "idx_teacherclassassignments_assignedat" 
    ON "Teacher"."TeacherClassAssignments"("AssignedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for TeacherId + Status in TeacherBadgeSubmissions
CREATE INDEX IF NOT EXISTS "idx_teacherbadgesubmissions_teacher_status" 
    ON "Teacher"."TeacherBadgeSubmissions"("TeacherId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for Status + SubmittedAt (for pending submissions)
CREATE INDEX IF NOT EXISTS "idx_teacherbadgesubmissions_status_submitted" 
    ON "Teacher"."TeacherBadgeSubmissions"("Status", "SubmittedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for TeacherId + ModuleId + Status in TeacherCpdProgress
CREATE INDEX IF NOT EXISTS "idx_teachercpdprogress_teacher_module_status" 
    ON "Teacher"."TeacherCpdProgress"("TeacherId", "ModuleId", "Status") 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- SYSTEM SCHEMA
-- =============================================

-- Composite index for UserId + IsRead (for unread notifications)
CREATE INDEX IF NOT EXISTS "idx_notifications_user_unread" 
    ON "System"."Notifications"("UserId", "IsRead") 
    WHERE "IsDeleted" = FALSE AND "IsRead" = FALSE;

-- Composite index for UserId + Type (for notification filtering)
CREATE INDEX IF NOT EXISTS "idx_notifications_user_type" 
    ON "System"."Notifications"("UserId", "Type") 
    WHERE "IsDeleted" = FALSE;

-- Composite index for UserId + CreatedAt (for notification sorting)
CREATE INDEX IF NOT EXISTS "idx_notifications_user_createdat" 
    ON "System"."Notifications"("UserId", "CreatedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- Composite index for UserId + Type + CreatedAt in ActivityLogs
CREATE INDEX IF NOT EXISTS "idx_activitylogs_user_type_createdat" 
    ON "System"."ActivityLogs"("UserId", "Type", "CreatedAt" DESC) 
    WHERE "IsDeleted" = FALSE;

-- =============================================
-- ENABLE EXTENSIONS (if not already enabled)
-- =============================================

-- Enable pg_trgm extension for text search indexes
CREATE EXTENSION IF NOT EXISTS pg_trgm;

COMMIT;

-- =============================================
-- VERIFICATION QUERIES
-- =============================================

-- Check all indexes created
SELECT 
    schemaname,
    tablename,
    indexname,
    indexdef
FROM pg_indexes
WHERE schemaname IN ('Identity', 'General', 'Missions', 'Gamification', 'Portfolio', 'Teacher', 'System')
    AND indexname LIKE 'idx_%'
ORDER BY schemaname, tablename, indexname;

-- Check index sizes
SELECT 
    schemaname,
    tablename,
    indexname,
    pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
WHERE schemaname IN ('Identity', 'General', 'Missions', 'Gamification', 'Portfolio', 'Teacher', 'System')
    AND indexrelname LIKE 'idx_%'
ORDER BY pg_relation_size(indexrelid) DESC;

