ALTER TABLE "Users"."ItineraryItem" -- (Replace "User" with whatever schema this lives in)
    ADD COLUMN "IconName" VARCHAR(100),
    ADD COLUMN "IconColour" VARCHAR(50);

-- Drop the old confusing ItemType column
ALTER TABLE "Users"."ItineraryItem"
DROP COLUMN "ItemType";