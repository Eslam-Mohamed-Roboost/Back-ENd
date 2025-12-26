-- Mission Resources Table
CREATE TABLE IF NOT EXISTS "Missions"."MissionResources" (
    "ID" BIGSERIAL PRIMARY KEY,
    "CompanyID" BIGINT NOT NULL DEFAULT 0,
    "MissionId" BIGINT NOT NULL,
    "Type" VARCHAR(50) NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Url" TEXT NOT NULL,
    "Description" TEXT,
    "Order" INTEGER NOT NULL DEFAULT 0,
    "IsRequired" BOOLEAN NOT NULL DEFAULT FALSE,
    "CreatedAt" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
    "CreatedBy" BIGINT,
    "UpdatedAt" TIMESTAMP WITH TIME ZONE,
    "UpdatedBy" BIGINT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "TTL" TIMESTAMP WITH TIME ZONE,
    "RowVersion" BYTEA NOT NULL DEFAULT '\x0000000000000000',
    CONSTRAINT "FK_MissionResources_Mission" FOREIGN KEY ("MissionId")
        REFERENCES "Missions"."Missions"("ID") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "idx_missionresources_missionid" ON "Missions"."MissionResources" ("MissionId");
CREATE INDEX IF NOT EXISTS "idx_missionresources_order" ON "Missions"."MissionResources" ("MissionId", "Order");

