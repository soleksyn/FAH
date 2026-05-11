-- ============================================================
-- SportMatrix HIGH-FIDELITY MOCK DATA SEED SCRIPT (v3 - Diversified Activities)
-- ============================================================
-- Password for all athletes: Sport123!
-- ============================================================

-- 1. CLEAR EXISTING DATA
DELETE FROM "PlannedActivities";
DELETE FROM "TrainingPlans";
DELETE FROM "Activities";
DELETE FROM "Athletes";

-- 2. INSERT ATHLETES
INSERT INTO "Athletes" ("Id", "FirstName", "LastName", "Username", "Email", "DateOfBirth", "Weight", "Height", "City", "Country", "ProfilePictureUrl", "CreatedAt", "UpdatedAt") VALUES 
(0, 'System', 'Admin', 'admin', 'admin@sportmatrix.com', '1980-01-01', 80.0, 180, 'Kyiv', 'Ukraine', NULL, '2026-01-01', '2026-01-01'),
(1, 'Dmytro', 'Kovalenko', 'dkovalenko', 'd.kovalenko@sport.ua', '1992-04-12', 74.2, 178, 'Kyiv', 'Ukraine', NULL, '2026-01-01', '2026-01-01'),
(2, 'Olena', 'Shevchenko', 'oshevchenko', 'o.shevchenko@cycling.ua', '1995-08-23', 58.5, 168, 'Lviv', 'Ukraine', NULL, '2026-01-05', '2026-01-05'),
(3, 'Andriy', 'Melnyk', 'amelnyk', 'a.melnyk@fit.ua', '1988-11-05', 88.0, 185, 'Odesa', 'Ukraine', NULL, '2026-01-10', '2026-01-10'),
(4, 'Viktoriya', 'Kravchenko', 'vkravchenko', 'v.kravchenko@swim.ua', '1997-02-28', 62.3, 172, 'Kharkiv', 'Ukraine', NULL, '2026-01-15', '2026-01-15'),
(5, 'Ivan', 'Bondarenko', 'ibondarenko', 'i.bondarenko@power.ua', '1990-06-17', 95.5, 182, 'Dnipro', 'Ukraine', NULL, '2026-02-01', '2026-02-01'),
(6, 'Nataliya', 'Moroz', 'nmoroz', 'n.moroz@yoga.ua', '1993-03-14', 54.0, 164, 'Kyiv', 'Ukraine', NULL, '2026-02-10', '2026-02-10'),
(7, 'Serhiy', 'Tkachenko', 'stkachenko', 's.tkachenko@tri.ua', '1991-05-20', 78.0, 180, 'Lviv', 'Ukraine', NULL, '2026-02-15', '2026-02-15'),
(8, 'Oksana', 'Lysenko', 'olysenko', 'o.lysenko@trail.ua', '1994-11-02', 56.5, 166, 'Yaremche', 'Ukraine', NULL, '2026-02-20', '2026-02-20'),
(9, 'Maksym', 'Kravchuk', 'mkravchuk', 'm.kravchuk@hiit.ua', '1989-07-08', 84.5, 183, 'Poltava', 'Ukraine', NULL, '2026-03-01', '2026-03-01');

-- 3. HELPER FOR RANDOM GENERATION
-- We generate 35 varied activities for EACH athlete (5 of each type)
INSERT INTO "Activities" (
    "AthleteId", "Name", "Description", "ActivityType", 
    "Distance", "MovingTime", "ElapsedTime", "TotalElevationGain", 
    "StartDate", "StartDateLocal", "Timezone", 
    "AverageSpeed", "MaxSpeed", "AverageHeartRate", "MaxHeartRate", "AverageCadence",
    "CreatedAt", "UpdatedAt"
)
SELECT 
    A.Id,
    -- Name based on Type
    CASE T.Type
        WHEN 1 THEN (CASE (ABS(RANDOM()) % 3) WHEN 0 THEN 'Morning Run' WHEN 1 THEN 'Tempo Run' ELSE 'Interval Session' END)
        WHEN 2 THEN (CASE (ABS(RANDOM()) % 3) WHEN 0 THEN 'Road Ride' WHEN 1 THEN 'Hill Climbs' ELSE 'Long Spin' END)
        WHEN 3 THEN (CASE (ABS(RANDOM()) % 2) WHEN 0 THEN 'Morning Swim' ELSE 'Pool Intervals' END)
        WHEN 4 THEN (CASE (ABS(RANDOM()) % 3) WHEN 0 THEN 'Gym Workout' WHEN 1 THEN 'HIIT Session' ELSE 'Strength Training' END)
        WHEN 5 THEN (CASE (ABS(RANDOM()) % 2) WHEN 0 THEN 'Vinyasa Flow' ELSE 'Yin Yoga' END)
        WHEN 6 THEN (CASE (ABS(RANDOM()) % 2) WHEN 0 THEN 'Mountain Hike' ELSE 'Scenic Trail' END)
        WHEN 7 THEN (CASE (ABS(RANDOM()) % 2) WHEN 0 THEN 'Brick Session (B/R)' ELSE 'Transition Training' END)
    END,
    'High quality training session',
    T.Type,
    -- Distance
    CASE T.Type
        WHEN 1 THEN 5000 + (ABS(RANDOM()) % 15000)      -- Run: 5-20km
        WHEN 2 THEN 20000 + (ABS(RANDOM()) % 60000)     -- Cycle: 20-80km
        WHEN 3 THEN 1000 + (ABS(RANDOM()) % 3000)       -- Swim: 1-4km
        WHEN 6 THEN 8000 + (ABS(RANDOM()) % 12000)      -- Hike: 8-20km
        WHEN 7 THEN 25000 + (ABS(RANDOM()) % 40000)     -- Brick: 25-65km
        ELSE 0                                          -- Others: 0
    END,
    -- Moving Time
    CASE T.Type
        WHEN 1 THEN 1200 + (ABS(RANDOM()) % 5400)       -- Run: 20-110 min
        WHEN 2 THEN 3600 + (ABS(RANDOM()) % 10800)      -- Cycle: 1-4 hours
        WHEN 3 THEN 1800 + (ABS(RANDOM()) % 3600)       -- Swim: 30-90 min
        WHEN 4 THEN 2700 + (ABS(RANDOM()) % 2700)       -- Workout: 45-90 min
        WHEN 5 THEN 3600                                -- Yoga: 60 min
        WHEN 6 THEN 7200 + (ABS(RANDOM()) % 14400)      -- Hike: 2-6 hours
        WHEN 7 THEN 5400 + (ABS(RANDOM()) % 7200)       -- Brick: 1.5-3.5 hours
    END,
    -- Elapsed Time (slightly more than moving)
    CASE T.Type
        WHEN 1 THEN 1300 + (ABS(RANDOM()) % 5500)
        WHEN 2 THEN 4000 + (ABS(RANDOM()) % 11000)
        WHEN 3 THEN 2000 + (ABS(RANDOM()) % 4000)
        WHEN 4 THEN 3000 + (ABS(RANDOM()) % 3000)
        WHEN 5 THEN 3600
        WHEN 6 THEN 8000 + (ABS(RANDOM()) % 15000)
        WHEN 7 THEN 6000 + (ABS(RANDOM()) % 8000)
    END,
    -- Elevation
    CASE T.Type
        WHEN 1 THEN 20 + (ABS(RANDOM()) % 300)
        WHEN 2 THEN 100 + (ABS(RANDOM()) % 1500)
        WHEN 6 THEN 500 + (ABS(RANDOM()) % 2000)
        WHEN 7 THEN 200 + (ABS(RANDOM()) % 1000)
        ELSE 0
    END,
    -- Dates (Spread over last 60 days)
    datetime('now', '-' || (1 + (ABS(RANDOM()) % 60)) || ' days', '+' || (ABS(RANDOM()) % 1440) || ' minutes'),
    datetime('now', '-' || (1 + (ABS(RANDOM()) % 60)) || ' days', '+' || (ABS(RANDOM()) % 1440) || ' minutes'),
    'Europe/Kiev',
    -- Speeds
    CASE T.Type
        WHEN 1 THEN 2.8 + (ABS(RANDOM()) % 20 / 10.0)
        WHEN 2 THEN 6.5 + (ABS(RANDOM()) % 50 / 10.0)
        WHEN 3 THEN 0.6 + (ABS(RANDOM()) % 10 / 10.0)
        WHEN 6 THEN 1.1 + (ABS(RANDOM()) % 10 / 10.0)
        WHEN 7 THEN 5.5 + (ABS(RANDOM()) % 30 / 10.0)
        ELSE 0
    END,
    CASE T.Type
        WHEN 1 THEN 4.5 + (ABS(RANDOM()) % 15 / 10.0)
        WHEN 2 THEN 12.0 + (ABS(RANDOM()) % 80 / 10.0)
        WHEN 3 THEN 1.2 + (ABS(RANDOM()) % 5 / 10.0)
        WHEN 6 THEN 2.5 + (ABS(RANDOM()) % 10 / 10.0)
        WHEN 7 THEN 10.0 + (ABS(RANDOM()) % 50 / 10.0)
        ELSE 0
    END,
    -- Heart Rate
    110 + (ABS(RANDOM()) % 50),
    150 + (ABS(RANDOM()) % 40),
    -- Cadence
    CASE T.Type
        WHEN 1 THEN 160 + (ABS(RANDOM()) % 25)
        WHEN 2 THEN 80 + (ABS(RANDOM()) % 20)
        ELSE NULL
    END,
    datetime('now', '-60 days'),
    datetime('now', '-60 days')
FROM "Athletes" A
CROSS JOIN (
    -- Generates 35 records per athlete
    SELECT 1 as n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 
    UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9 UNION SELECT 10
    UNION SELECT 11 UNION SELECT 12 UNION SELECT 13 UNION SELECT 14 UNION SELECT 15
    UNION SELECT 16 UNION SELECT 17 UNION SELECT 18 UNION SELECT 19 UNION SELECT 20
    UNION SELECT 21 UNION SELECT 22 UNION SELECT 23 UNION SELECT 24 UNION SELECT 25
    UNION SELECT 26 UNION SELECT 27 UNION SELECT 28 UNION SELECT 29 UNION SELECT 30
    UNION SELECT 31 UNION SELECT 32 UNION SELECT 33 UNION SELECT 34 UNION SELECT 35
) N
JOIN (
    -- Maps to ActivityType (1-7)
    SELECT 1 as Type UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 
    UNION SELECT 5 UNION SELECT 6 UNION SELECT 7
) T ON ( (N.n + A.Id) % 7 + 1 ) = T.Type
WHERE A.Id <> 0;

-- 4. ADD SOME UP-TO-DATE "LIVE" ACTIVITIES (One for today per athlete)
INSERT INTO "Activities" (
    "AthleteId", "Name", "Description", "ActivityType", 
    "Distance", "MovingTime", "ElapsedTime", "TotalElevationGain", 
    "StartDate", "StartDateLocal", "Timezone", 
    "AverageSpeed", "MaxSpeed", "AverageHeartRate", "MaxHeartRate", "AverageCadence",
    "CreatedAt", "UpdatedAt"
)
SELECT 
    Id, 
    'Recent Recovery Session', 
    'Today''s training data', 
    ((Id % 7) + 1), -- Varied type based on Id
    3000, 1200, 1200, 10, 
    datetime('now'), datetime('now', 'localtime'), 'Europe/Kiev', 
    2.5, 3.2, 130, 155, 170,
    datetime('now'), datetime('now')
FROM "Athletes"
WHERE Id <> 0;
