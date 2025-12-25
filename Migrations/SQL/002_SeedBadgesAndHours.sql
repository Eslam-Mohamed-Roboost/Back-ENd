-- ============================================
-- Seed Script: Badge and Hours Configuration
-- Description: Seeds initial badges and configures hours for activities
-- Date: 2025-12-25
-- ============================================

-- ============================================
-- PART 1: SEED STUDENT BADGES
-- ============================================

-- Insert Digital Citizenship Mission Badges (if they don't exist)
DO $$
DECLARE
    v_company_id bigint := 1; -- Adjust to your company ID
BEGIN
    -- Mission 1: Digital Citizen Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Digital Citizen') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Digital Citizen',
            'Completed Mission 1: Understanding Digital Citizenship',
            '🏛️',
            '#4A90E2',
            0, -- Category: Achievement
            0, -- TargetRole: Student
            true,
            NOW()
        );
    END IF;

    -- Mission 2: Footprint Tracker Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Footprint Tracker') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Footprint Tracker',
            'Completed Mission 2: Managing Your Digital Footprint',
            '👣',
            '#27AE60',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 3: Safety Shield Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Safety Shield') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Safety Shield',
            'Completed Mission 3: Staying Safe Online',
            '🛡️',
            '#E74C3C',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 4: Kindness Champion Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Kindness Champion') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Kindness Champion',
            'Completed Mission 4: Being Kind Online',
            '💝',
            '#F39C12',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 5: Truth Seeker Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Truth Seeker') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Truth Seeker',
            'Completed Mission 5: Spotting Fake News',
            '🔍',
            '#9B59B6',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 6: Communication Pro Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Communication Pro') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Communication Pro',
            'Completed Mission 6: Communicating Responsibly',
            '💬',
            '#1ABC9C',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 7: Balance Master Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Balance Master') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Balance Master',
            'Completed Mission 7: Balancing Screen Time',
            '⚖️',
            '#34495E',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Mission 8: Digital Leader Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Digital Leader') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Digital Leader',
            'Completed Mission 8: Leading by Example',
            '👑',
            '#FFD700',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Challenge Conqueror Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Challenge Conqueror') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Challenge Conqueror',
            'Completed 10 challenges',
            '⚡',
            '#FF6B6B',
            0,
            0,
            true,
            NOW()
        );
    END IF;

    -- Quick Learner Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Quick Learner') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Quick Learner',
            'Completed a mission in under 1 hour',
            '⚡',
            '#FFA500',
            0,
            0,
            true,
            NOW()
        );
    END IF;
END $$;

-- ============================================
-- PART 2: SEED TEACHER CPD BADGES
-- ============================================

DO $$
DECLARE
    v_company_id bigint := 1; -- Adjust to your company ID
BEGIN
    -- Tech Innovator Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Tech Innovator') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CpdHours", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Tech Innovator',
            'Earned 5 CPD hours',
            '💡',
            '#3498DB',
            1, -- Category: CPD
            1, -- TargetRole: Teacher
            true,
            5.0,
            NOW()
        );
    END IF;

    -- Digital Mentor Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Digital Mentor') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CpdHours", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Digital Mentor',
            'Earned 10 CPD hours',
            '🎓',
            '#2ECC71',
            1,
            1,
            true,
            10.0,
            NOW()
        );
    END IF;

    -- Master Educator Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Master Educator') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CpdHours", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Master Educator',
            'Earned 20 CPD hours',
            '🏆',
            '#E67E22',
            1,
            1,
            true,
            20.0,
            NOW()
        );
    END IF;

    -- Challenge Creator Badge
    IF NOT EXISTS (SELECT 1 FROM "Gamification"."Badges" WHERE "Name" = 'Challenge Creator') THEN
        INSERT INTO "Gamification"."Badges" (
            "ID", "CompanyID", "Name", "Description", "Icon", "Color", 
            "Category", "TargetRole", "IsActive", "CpdHours", "CreatedAt"
        ) VALUES (
            (SELECT COALESCE(MAX("ID"), 0) + 1 FROM "Gamification"."Badges"),
            v_company_id,
            'Challenge Creator',
            'Created 5 challenges',
            '🎨',
            '#9B59B6',
            1,
            1,
            true,
            2.0,
            NOW()
        );
    END IF;
END $$;

-- ============================================
-- PART 3: LINK BADGES TO MISSIONS
-- ============================================

-- Update Missions table to link badges
UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Digital Citizenship%' AND b."Name" = 'Digital Citizen';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Digital Footprint%' AND b."Name" = 'Footprint Tracker';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Safe Online%' OR m."Title" LIKE '%Safety%' AND b."Name" = 'Safety Shield';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Kind%' OR m."Title" LIKE '%Kindness%' AND b."Name" = 'Kindness Champion';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Fake News%' OR m."Title" LIKE '%Truth%' AND b."Name" = 'Truth Seeker';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Communicat%' AND b."Name" = 'Communication Pro';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Balance%' OR m."Title" LIKE '%Screen Time%' AND b."Name" = 'Balance Master';

UPDATE "Missions"."Missions" m
SET "BadgeId" = b."ID"
FROM "Gamification"."Badges" b
WHERE m."Title" LIKE '%Lead%' AND b."Name" = 'Digital Leader';

-- ============================================
-- PART 4: VERIFY SEEDED DATA
-- ============================================

-- Count badges by target role
SELECT 
    "TargetRole",
    CASE 
        WHEN "TargetRole" = 0 THEN 'Student'
        WHEN "TargetRole" = 1 THEN 'Teacher'
        WHEN "TargetRole" = 2 THEN 'Both'
        ELSE 'Unknown'
    END as "RoleName",
    COUNT(*) as "BadgeCount"
FROM "Gamification"."Badges"
WHERE "IsActive" = true
GROUP BY "TargetRole"
ORDER BY "TargetRole";

-- Show all mission badges with hours
SELECT 
    m."Number",
    m."Title",
    m."HoursAwarded",
    b."Name" as "BadgeName",
    b."Icon" as "BadgeIcon"
FROM "Missions"."Missions" m
LEFT JOIN "Gamification"."Badges" b ON m."BadgeId" = b."ID"
ORDER BY m."Number";

-- Show challenge configuration
SELECT 
    "Difficulty",
    CASE 
        WHEN "Difficulty" = 0 THEN 'Easy'
        WHEN "Difficulty" = 1 THEN 'Medium'
        WHEN "Difficulty" = 2 THEN 'Hard'
        ELSE 'Unknown'
    END as "DifficultyName",
    AVG("HoursAwarded") as "AverageHours",
    COUNT(*) as "ChallengeCount"
FROM "Gamification"."Challenges"
WHERE "IsActive" = true
GROUP BY "Difficulty"
ORDER BY "Difficulty";

-- ============================================
-- END OF SEED SCRIPT
-- ============================================

SELECT 'Seed data inserted successfully!' as status;

