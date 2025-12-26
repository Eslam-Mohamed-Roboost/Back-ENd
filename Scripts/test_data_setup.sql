-- =============================================
-- Test Data Setup Script for School Hub v2
-- PostgreSQL 14+
-- =============================================
-- 
-- IMPORTANT NOTES:
-- 1. This script creates test data for manual testing scenarios
-- 2. Passwords need to be hashed using SecurityHelper.GetHashedPassword()
-- 3. UserName needs to be encrypted using SecurityHelper.Encrypt()
-- 4. For easier setup, use the Admin interface to create users OR
--    use the SeedDatabaseCommand endpoint: POST /Admin/SeedData
--
-- =============================================

BEGIN;

-- =============================================
-- 1. CREATE TEST CLASSES
-- =============================================

INSERT INTO "General"."Classes" ("Name", "Grade", "StudentCount", "CreatedAt", "IsDeleted")
VALUES
    ('Grade 6A', 6, 25, NOW(), FALSE),
    ('Grade 6B', 6, 25, NOW(), FALSE),
    ('Grade 7A', 7, 28, NOW(), FALSE),
    ('Grade 7B', 7, 27, NOW(), FALSE),
    ('Grade 8A', 8, 30, NOW(), FALSE),
    ('Grade 8B', 8, 29, NOW(), FALSE)
ON CONFLICT DO NOTHING;

-- =============================================
-- 2. CREATE TEST SUBJECTS
-- =============================================

INSERT INTO "General"."Subjects" ("Name", "Icon", "Color", "IsActive", "CreatedAt", "IsDeleted")
VALUES
    ('Mathematics', '🔢', '#FF9800', TRUE, NOW(), FALSE),
    ('Science', '🔬', '#9C27B0', TRUE, NOW(), FALSE),
    ('English Language Arts', '📚', '#4CAF50', TRUE, NOW(), FALSE),
    ('Arabic', '🌍', '#F44336', TRUE, NOW(), FALSE),
    ('Islamic Studies', '☪️', '#00BCD4', TRUE, NOW(), FALSE),
    ('ICT', '💻', '#2196F3', TRUE, NOW(), FALSE)
ON CONFLICT DO NOTHING;

-- =============================================
-- 3. CREATE TEST USERS
-- =============================================
-- NOTE: Passwords and UserNames need to be hashed/encrypted
-- Use the Admin interface or API to create users with proper password hashing
-- OR use the SeedDatabaseCommand endpoint
--
-- Test User Credentials (to be created via Admin interface or API):
-- 
-- Admin User:
--   Username: admin
--   Password: Admin123!
--   Email: admin@school.ae
--   Role: Admin (1)
--
-- Teacher Users:
--   Teacher 1:
--     Username: teacher1
--     Password: Teacher123!
--     Email: teacher1@school.ae
--     Role: Teacher (3)
--
--   Teacher 2:
--     Username: teacher2
--     Password: Teacher123!
--     Email: teacher2@school.ae
--     Role: Teacher (3)
--
-- Student Users:
--   Student 1:
--     Username: student1
--     Password: Student123!
--     Email: student1@school.ae
--     Role: Student (4)
--     Class: Grade 7A
--
--   Student 2:
--     Username: student2
--     Password: Student123!
--     Email: student2@school.ae
--     Role: Student (4)
--     Class: Grade 7A
--
--   Student 3:
--     Username: student3
--     Password: Student123!
--     Email: student3@school.ae
--     Role: Student (4)
--     Class: Grade 7B
--
--   Student 4:
--     Username: student4
--     Password: Student123!
--     Email: student4@school.ae
--     Role: Student (4)
--     Class: Grade 8A
--
--   Student 5:
--     Username: student5
--     Password: Student123!
--     Email: student5@school.ae
--     Role: Student (4)
--     Class: Grade 8A

-- =============================================
-- 4. CREATE TEACHER-CLASS-SUBJECT ASSIGNMENTS
-- =============================================
-- NOTE: This requires Teacher IDs and Class IDs from the users and classes created above
-- Replace the IDs below with actual IDs from your database
--
-- Example assignments (update IDs based on your data):
-- Teacher 1 -> Grade 7A -> Mathematics
-- Teacher 1 -> Grade 7A -> Science
-- Teacher 1 -> Grade 7B -> Mathematics
-- Teacher 2 -> Grade 8A -> English Language Arts
-- Teacher 2 -> Grade 8A -> ICT

-- Example SQL (uncomment and update IDs):
/*
INSERT INTO "Teacher"."TeacherClassAssignments" 
    ("TeacherId", "ClassId", "SubjectId", "AssignedAt", "CreatedAt", "IsDeleted")
SELECT 
    t."ID" as "TeacherId",
    c."ID" as "ClassId",
    s."ID" as "SubjectId",
    NOW() as "AssignedAt",
    NOW() as "CreatedAt",
    FALSE as "IsDeleted"
FROM "Identity"."User" t
CROSS JOIN "General"."Classes" c
CROSS JOIN "General"."Subjects" s
WHERE t."Role" = 3  -- Teacher role
  AND t."UserName" = 'encrypted_teacher1_username'  -- Replace with encrypted username
  AND c."Name" = 'Grade 7A'
  AND s."Name" = 'Mathematics'
ON CONFLICT DO NOTHING;
*/

-- =============================================
-- 5. CREATE TEST EXERCISES (Optional)
-- =============================================
-- NOTE: Requires Teacher, Class, and Subject IDs
-- Can be created via Teacher interface or API

-- =============================================
-- 6. CREATE TEST EXAMINATIONS (Optional)
-- =============================================
-- NOTE: Requires Teacher, Class, and Subject IDs
-- Can be created via Teacher interface or API

-- =============================================
-- 7. CREATE TEST MISSIONS (Optional)
-- =============================================
-- NOTE: Can be created via Admin interface

-- =============================================
-- 8. CREATE TEST BADGES (Optional)
-- =============================================
-- NOTE: Can be created via Admin interface

COMMIT;

-- =============================================
-- SETUP INSTRUCTIONS
-- =============================================
--
-- RECOMMENDED APPROACH:
-- 1. Run this script to create Classes and Subjects
-- 2. Use the Admin interface to create test users:
--    - Login as admin (or create admin user first via database)
--    - Navigate to /admin/users
--    - Create test users with the credentials listed above
-- 3. Assign teachers to classes/subjects via Admin interface:
--    - Navigate to /admin/users or /admin/classes
--    - Find a teacher
--    - Click "Assign Classes/Subjects"
--    - Select classes and subjects
-- 4. Create exercises, examinations, missions via respective interfaces
--
-- ALTERNATIVE APPROACH:
-- Use the SeedDatabaseCommand endpoint:
-- POST /Admin/SeedData
-- This will create comprehensive test data including users, classes, subjects, etc.
--
-- =============================================

