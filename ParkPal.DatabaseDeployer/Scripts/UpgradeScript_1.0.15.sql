ALTER TABLE "Static"."Attraction"
    ADD COLUMN "ExternalId" character varying(100) NULL;

-- (Optional) Add an index to it, because if you ever do need to search by it, you'll want it to be fast!
CREATE INDEX "IX_Attraction_ExternalId" ON "Static"."Attraction" ("ExternalId");