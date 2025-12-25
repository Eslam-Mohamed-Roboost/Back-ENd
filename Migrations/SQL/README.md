# Database Migration Scripts

## Overview
This folder contains SQL migration scripts for the Badge and Hours Earning System.

## Scripts

### 1. `001_AddBadgeAndHoursSystem.sql`
**Purpose**: Creates the LearningHours table and adds hours/badge columns to existing tables.

**What it does**:
- Creates `General.LearningHours` table with appropriate indexes
- Adds `HoursAwarded` column to `Missions.Missions` table
- Adds `BadgeId` and `HoursAwarded` columns to `Gamification.Challenges` table
- Creates statistics view for student learning hours
- Includes verification queries and rollback script

**When to run**: First time setup or when upgrading existing database

### 2. `002_SeedBadgesAndHours.sql`
**Purpose**: Seeds initial badges and configures hours for activities.

**What it does**:
- Inserts 8 Digital Citizenship mission badges for students
- Inserts special badges (Challenge Conqueror, Quick Learner)
- Inserts 4 CPD badges for teachers
- Links badges to missions
- Configures default hours for challenges based on difficulty

**When to run**: After running `001_AddBadgeAndHoursSystem.sql`

## How to Run

### Option 1: Using psql Command Line

```bash
# Navigate to the SQL directory
cd Back-End/Migrations/SQL

# Run migration script
psql -h localhost -U your_username -d your_database -f 001_AddBadgeAndHoursSystem.sql

# Run seed script
psql -h localhost -U your_username -d your_database -f 002_SeedBadgesAndHours.sql
```

### Option 2: Using pgAdmin or Database GUI
1. Open pgAdmin or your preferred PostgreSQL GUI
2. Connect to your database
3. Open Query Tool
4. Load and execute `001_AddBadgeAndHoursSystem.sql`
5. Load and execute `002_SeedBadgesAndHours.sql`

### Option 3: Using Entity Framework Migration (Recommended)

```bash
# Navigate to Back-End directory
cd Back-End

# Create migration
dotnet ef migrations add AddBadgeAndHoursSystem

# Update database
dotnet ef database update

# Then run seed script separately using psql or pgAdmin
```

## Pre-requisites

Before running the scripts, ensure:
- PostgreSQL database is running
- You have appropriate permissions (CREATE TABLE, ALTER TABLE, INSERT)
- Required schemas exist: `General`, `Missions`, `Gamification`
- The following tables exist:
  - `Missions.Missions`
  - `Gamification.Challenges`
  - `Gamification.Badges`

## Verification

After running the scripts, verify the changes:

```sql
-- Check LearningHours table
SELECT * FROM information_schema.tables 
WHERE table_schema = 'General' AND table_name = 'LearningHours';

-- Check new columns in Missions
SELECT column_name, data_type, column_default
FROM information_schema.columns
WHERE table_schema = 'Missions' 
AND table_name = 'Missions'
AND column_name = 'HoursAwarded';

-- Check new columns in Challenges
SELECT column_name, data_type, column_default
FROM information_schema.columns
WHERE table_schema = 'Gamification' 
AND table_name = 'Challenges'
AND column_name IN ('BadgeId', 'HoursAwarded');

-- Check seeded badges
SELECT COUNT(*) as badge_count, "TargetRole"
FROM "Gamification"."Badges"
WHERE "IsActive" = true
GROUP BY "TargetRole";
```

## Rollback

If you need to rollback the changes, use the rollback section in `001_AddBadgeAndHoursSystem.sql`:

```sql
-- Uncomment and run the rollback section at the end of the file
DROP VIEW IF EXISTS "General"."V_StudentLearningHoursStats";
DROP TABLE IF EXISTS "General"."LearningHours";
ALTER TABLE "Missions"."Missions" DROP COLUMN IF EXISTS "HoursAwarded";
ALTER TABLE "Gamification"."Challenges" DROP COLUMN IF EXISTS "BadgeId";
ALTER TABLE "Gamification"."Challenges" DROP COLUMN IF EXISTS "HoursAwarded";
```

## Customization

### Adjusting Hours Configuration
To change default hours, edit these sections in `001_AddBadgeAndHoursSystem.sql`:

```sql
-- For Missions (lines 105-111)
UPDATE "Missions"."Missions"
SET "HoursAwarded" = CASE 
    WHEN "Number" BETWEEN 1 AND 4 THEN 1.5  -- Change these values
    WHEN "Number" BETWEEN 5 AND 8 THEN 2.0  -- Change these values
    ELSE 1.5
END;

-- For Challenges (lines 147-153)
UPDATE "Gamification"."Challenges"
SET "HoursAwarded" = CASE 
    WHEN "Difficulty" = 0 THEN 0.5  -- Easy - Change this
    WHEN "Difficulty" = 1 THEN 1.0  -- Medium - Change this
    WHEN "Difficulty" = 2 THEN 1.5  -- Hard - Change this
    ELSE 0.5
END;
```

### Adding More Badges
Edit `002_SeedBadgesAndHours.sql` and add new badge INSERT statements following the existing pattern.

## Troubleshooting

### Error: Schema does not exist
Create the required schemas first:
```sql
CREATE SCHEMA IF NOT EXISTS "General";
CREATE SCHEMA IF NOT EXISTS "Missions";
CREATE SCHEMA IF NOT EXISTS "Gamification";
```

### Error: Permission denied
Grant appropriate permissions:
```sql
GRANT ALL PRIVILEGES ON SCHEMA "General" TO your_username;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA "General" TO your_username;
```

### Error: Column already exists
The scripts use `IF NOT EXISTS` checks, but if you get this error, the column was added manually. You can skip that section or drop and re-add it.

## Notes

- The `LearningHours` table uses soft deletes (`IsDeleted` flag)
- All indexes are created with `WHERE "IsDeleted" = false` for better performance
- Badge IDs are auto-generated using `MAX(ID) + 1` pattern
- Company ID defaults to 1 in seed script - adjust if needed
- The statistics view aggregates hours by week, month, and year
- All timestamps use `timestamp with time zone` for proper timezone handling

## Support

For issues or questions, refer to the main `IMPLEMENTATION_SUMMARY.md` file or contact the development team.

