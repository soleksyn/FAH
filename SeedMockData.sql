-- ============================================================
-- SportMatrix Mock Data Insertion Script
-- ============================================================
-- NOTE: This script completely clears all existing data and 
-- seeds the database with a rich set of detailed mock data 
-- for multiple athletes to populate the frontend dashboards.
-- ============================================================

-- 1. CLEAR EXISTING DATA
DELETE FROM "PlannedActivities";
DELETE FROM "TrainingPlans";
DELETE FROM "Activities";
DELETE FROM "Athletes";

-- 2. INSERT ATHLETES
INSERT INTO "Athletes" ("Id", "FirstName", "LastName", "Username", "Email", "City", "Country", "ProfilePictureUrl", "CreatedAt", "UpdatedAt")
VALUES 
(1, 'John', 'Doe', 'johndoe', 'john.doe@example.com', 'Kyiv', 'Ukraine', 'https://example.com/john.jpg', '2026-01-01 00:00:00', '2026-01-01 00:00:00'),
(2, 'Jane', 'Smith', 'janesmith', 'jane.smith@example.com', 'Lviv', 'Ukraine', 'https://example.com/jane.jpg', '2026-01-05 00:00:00', '2026-01-05 00:00:00'),
(3, 'Oleksandr', 'Petrenko', 'oleksandrp', 'oleksandr.petrenko@example.com', 'Odessa', 'Ukraine', 'https://example.com/oleksandr.jpg', '2026-01-15 00:00:00', '2026-01-15 00:00:00'),
(4, 'Maria', 'Kovalenko', 'mariak', 'maria.kovalenko@example.com', 'Kharkiv', 'Ukraine', 'https://example.com/maria.jpg', '2026-01-20 00:00:00', '2026-01-20 00:00:00'),
(5, 'Andriy', 'Bohdan', 'andriyb', 'andriy.bohdan@example.com', 'Dnipro', 'Ukraine', 'https://example.com/andriy.jpg', '2026-02-01 00:00:00', '2026-02-01 00:00:00'),
(6, 'Anna', 'Schmidt', 'annas', 'anna.schmidt@example.com', 'Berlin', 'Germany', 'https://example.com/anna.jpg', '2026-02-15 00:00:00', '2026-02-15 00:00:00');

-- 3. INSERT ACTIVITIES
-- We are creating a rich history for each athlete over the past few weeks (April - May 2026)
INSERT INTO "Activities" ("Id", "AthleteId", "Name", "Description", "Distance", "MovingTime", "ElapsedTime", "TotalElevationGain", "SportType", "StartDate", "StartDateLocal", "Timezone", "CreatedAt", "UpdatedAt", "AverageSpeed", "MaxSpeed", "AverageHeartRate", "MaxHeartRate", "AverageCadence")
VALUES 
-- ATHLETE 1 (John Doe) - Mixed training (Run, Ride, Swim)
(1, 1, 'Morning Run', '5km morning run in the park', 5000, 1500, 1550, 45, 'Run', '2026-04-20 06:00:00', '2026-04-20 09:00:00', 'Europe/Kiev', '2026-04-20 06:00:00', '2026-04-20 06:00:00', 3.33, 4.1, 142, 160, 165),
(2, 1, 'Cycling Training', 'Long weekend ride', 45000, 5400, 5600, 450, 'Ride', '2026-04-22 08:00:00', '2026-04-22 11:00:00', 'Europe/Kiev', '2026-04-22 08:00:00', '2026-04-22 08:00:00', 8.33, 15.0, 135, 155, 88),
(3, 1, 'Pool Session', 'Intervals in the pool', 2000, 2700, 3000, 0, 'Swim', '2026-04-24 18:00:00', '2026-04-24 21:00:00', 'Europe/Kiev', '2026-04-24 18:00:00', '2026-04-24 18:00:00', 0.74, 1.1, 125, 140, NULL),
(4, 1, 'Tempo Run', '8km tempo pace', 8000, 2200, 2250, 60, 'Run', '2026-04-26 17:30:00', '2026-04-26 20:30:00', 'Europe/Kiev', '2026-04-26 17:30:00', '2026-04-26 17:30:00', 3.63, 4.5, 155, 172, 172),
(5, 1, 'Recovery Ride', 'Easy spin along the river', 20000, 3000, 3100, 100, 'Ride', '2026-04-28 07:00:00', '2026-04-28 10:00:00', 'Europe/Kiev', '2026-04-28 07:00:00', '2026-04-28 07:00:00', 6.66, 11.2, 115, 130, 85),
(6, 1, 'Long Run', 'Sunday long distance', 15000, 4800, 4900, 120, 'Run', '2026-05-01 06:30:00', '2026-05-01 09:30:00', 'Europe/Kiev', '2026-05-01 06:30:00', '2026-05-01 06:30:00', 3.12, 3.8, 145, 160, 168),
(7, 1, 'HIIT Workout', 'Indoor circuit training', 0, 1800, 1800, 0, 'Workout', '2026-05-03 18:00:00', '2026-05-03 21:00:00', 'Europe/Kiev', '2026-05-03 18:00:00', '2026-05-03 18:00:00', NULL, NULL, 158, 180, NULL),

-- ATHLETE 2 (Jane Smith) - Heavy runner
(8, 2, 'Track Intervals', '400m repeats', 6000, 1500, 2000, 20, 'Run', '2026-04-21 17:00:00', '2026-04-21 20:00:00', 'Europe/Kiev', '2026-04-21 17:00:00', '2026-04-21 17:00:00', 4.0, 5.5, 165, 185, 180),
(9, 2, 'Base Run', 'Easy aerobic effort', 10000, 3300, 3350, 85, 'Run', '2026-04-23 06:00:00', '2026-04-23 09:00:00', 'Europe/Kiev', '2026-04-23 06:00:00', '2026-04-23 06:00:00', 3.03, 3.5, 138, 150, 165),
(10, 2, 'Hill Repeats', 'Short steep hills', 7500, 2400, 2500, 250, 'Run', '2026-04-25 18:00:00', '2026-04-25 21:00:00', 'Europe/Kiev', '2026-04-25 18:00:00', '2026-04-25 18:00:00', 3.12, 4.8, 158, 178, 168),
(11, 2, 'Yoga Session', 'Active recovery flow', 0, 2700, 2700, 0, 'Yoga', '2026-04-27 07:00:00', '2026-04-27 10:00:00', 'Europe/Kiev', '2026-04-27 07:00:00', '2026-04-27 07:00:00', NULL, NULL, 95, 115, NULL),
(12, 2, 'Long Endurance Run', 'Weekend trail run', 21000, 7200, 7500, 450, 'Run', '2026-04-30 08:00:00', '2026-04-30 11:00:00', 'Europe/Kiev', '2026-04-30 08:00:00', '2026-04-30 08:00:00', 2.91, 4.0, 142, 162, 162),
(13, 2, 'Fartlek Run', 'Unstructured speed play', 8500, 2600, 2650, 70, 'Run', '2026-05-02 18:30:00', '2026-05-02 21:30:00', 'Europe/Kiev', '2026-05-02 18:30:00', '2026-05-02 18:30:00', 3.26, 5.0, 152, 175, 170),
(14, 2, 'Core Workout', 'Ab and back strength', 0, 1200, 1200, 0, 'Workout', '2026-05-04 07:00:00', '2026-05-04 10:00:00', 'Europe/Kiev', '2026-05-04 07:00:00', '2026-05-04 07:00:00', NULL, NULL, 110, 135, NULL),

-- ATHLETE 3 (Oleksandr) - Cycling focused
(15, 3, 'Commute to Work', 'Morning commute', 12000, 1800, 1900, 50, 'Ride', '2026-04-20 07:30:00', '2026-04-20 10:30:00', 'Europe/Kiev', '2026-04-20 07:30:00', '2026-04-20 07:30:00', 6.66, 12.5, 120, 140, 80),
(16, 3, 'Commute Home', 'Evening commute', 12000, 1850, 2000, 60, 'Ride', '2026-04-20 17:30:00', '2026-04-20 20:30:00', 'Europe/Kiev', '2026-04-20 17:30:00', '2026-04-20 17:30:00', 6.48, 11.5, 122, 142, 82),
(17, 3, 'Gravel Grinder', 'Off-road exploration', 65000, 9500, 10000, 850, 'Ride', '2026-04-23 08:00:00', '2026-04-23 11:00:00', 'Europe/Kiev', '2026-04-23 08:00:00', '2026-04-23 08:00:00', 6.84, 16.2, 148, 172, 85),
(18, 3, 'Mountain Bike Session', 'Technical trails', 25000, 4800, 5200, 650, 'Ride', '2026-04-26 09:00:00', '2026-04-26 12:00:00', 'Europe/Kiev', '2026-04-26 09:00:00', '2026-04-26 09:00:00', 5.20, 14.5, 155, 180, 75),
(19, 3, 'Group Road Ride', 'Fast paceline with club', 80000, 8400, 8800, 600, 'Ride', '2026-04-29 07:00:00', '2026-04-29 10:00:00', 'Europe/Kiev', '2026-04-29 07:00:00', '2026-04-29 07:00:00', 9.52, 18.0, 140, 168, 92),
(20, 3, 'Evening Jog', 'Easy leg shakeout', 4000, 1400, 1400, 20, 'Run', '2026-05-02 19:00:00', '2026-05-02 22:00:00', 'Europe/Kiev', '2026-05-02 19:00:00', '2026-05-02 19:00:00', 2.85, 3.5, 130, 145, 160),

-- ATHLETE 4 (Maria) - Hiking, Walking, and Yoga
(21, 4, 'Morning Walk', 'Brisk walk in the park', 3500, 2400, 2400, 30, 'Walk', '2026-04-21 07:00:00', '2026-04-21 10:00:00', 'Europe/Kiev', '2026-04-21 07:00:00', '2026-04-21 07:00:00', 1.45, 1.8, 105, 120, 110),
(22, 4, 'Vinyasa Flow', 'Power yoga class', 0, 3600, 3600, 0, 'Yoga', '2026-04-23 18:00:00', '2026-04-23 21:00:00', 'Europe/Kiev', '2026-04-23 18:00:00', '2026-04-23 18:00:00', NULL, NULL, 115, 135, NULL),
(23, 4, 'Weekend Hike', 'Mountain trail hike', 14000, 14400, 18000, 1200, 'Hike', '2026-04-26 08:00:00', '2026-04-26 11:00:00', 'Europe/Kiev', '2026-04-26 08:00:00', '2026-04-26 08:00:00', 0.97, 1.5, 125, 160, NULL),
(24, 4, 'Evening Stroll', 'City walk', 4000, 2700, 2800, 10, 'Walk', '2026-04-29 19:30:00', '2026-04-29 22:30:00', 'Europe/Kiev', '2026-04-29 19:30:00', '2026-04-29 19:30:00', 1.48, 1.9, 100, 115, 112),
(25, 4, 'Pilates', 'Core stability training', 0, 2700, 2700, 0, 'Workout', '2026-05-01 17:30:00', '2026-05-01 20:30:00', 'Europe/Kiev', '2026-05-01 17:30:00', '2026-05-01 17:30:00', NULL, NULL, 108, 125, NULL),
(26, 4, 'Long Hike', 'National park exploration', 18000, 18000, 21600, 1450, 'Hike', '2026-05-04 07:30:00', '2026-05-04 10:30:00', 'Europe/Kiev', '2026-05-04 07:30:00', '2026-05-04 07:30:00', 1.0, 1.8, 130, 165, NULL),

-- ATHLETE 5 (Andriy) - General fitness
(27, 5, 'Gym Session - Push', 'Chest, shoulders, triceps', 0, 3600, 4200, 0, 'Workout', '2026-04-20 18:00:00', '2026-04-20 21:00:00', 'Europe/Kiev', '2026-04-20 18:00:00', '2026-04-20 18:00:00', NULL, NULL, 125, 160, NULL),
(28, 5, 'Gym Session - Pull', 'Back and biceps', 0, 3600, 4000, 0, 'Workout', '2026-04-22 18:00:00', '2026-04-22 21:00:00', 'Europe/Kiev', '2026-04-22 18:00:00', '2026-04-22 18:00:00', NULL, NULL, 128, 158, NULL),
(29, 5, 'Gym Session - Legs', 'Heavy squats and deadlifts', 0, 4200, 4800, 0, 'Workout', '2026-04-24 17:30:00', '2026-04-24 20:30:00', 'Europe/Kiev', '2026-04-24 17:30:00', '2026-04-24 17:30:00', NULL, NULL, 135, 175, NULL),
(30, 5, 'Weekend Run', '5km pushing the pace', 5000, 1400, 1400, 40, 'Run', '2026-04-26 09:00:00', '2026-04-26 12:00:00', 'Europe/Kiev', '2026-04-26 09:00:00', '2026-04-26 09:00:00', 3.57, 4.5, 160, 182, 165),
(31, 5, 'Rowing Machine', '30 min steady state', 7500, 1800, 1800, 0, 'Workout', '2026-04-28 07:00:00', '2026-04-28 10:00:00', 'Europe/Kiev', '2026-04-28 07:00:00', '2026-04-28 07:00:00', 4.16, 4.8, 145, 165, NULL),
(32, 5, 'CrossFit WOD', 'High intensity functional training', 0, 2400, 2700, 0, 'Workout', '2026-05-01 18:30:00', '2026-05-01 21:30:00', 'Europe/Kiev', '2026-05-01 18:30:00', '2026-05-01 18:30:00', NULL, NULL, 165, 188, NULL),
(33, 5, 'Recovery Walk', 'Active recovery', 4000, 2800, 2800, 20, 'Walk', '2026-05-03 10:00:00', '2026-05-03 13:00:00', 'Europe/Kiev', '2026-05-03 10:00:00', '2026-05-03 10:00:00', 1.42, 1.8, 105, 120, 108),

-- ATHLETE 6 (Anna) - Mix of Swim and Run
(34, 6, 'Pool Swim', 'Endurance sets', 2500, 3600, 3900, 0, 'Swim', '2026-04-21 06:30:00', '2026-04-21 07:30:00', 'Europe/Berlin', '2026-04-21 06:30:00', '2026-04-21 06:30:00', 0.69, 1.0, 135, 150, NULL),
(35, 6, 'Lunch Run', 'Quick 6k', 6000, 1900, 1950, 45, 'Run', '2026-04-23 12:15:00', '2026-04-23 13:15:00', 'Europe/Berlin', '2026-04-23 12:15:00', '2026-04-23 12:15:00', 3.15, 4.0, 148, 168, 168),
(36, 6, 'Open Water Swim', 'Lake swimming session', 3000, 4200, 4300, 0, 'Swim', '2026-04-26 10:00:00', '2026-04-26 11:00:00', 'Europe/Berlin', '2026-04-26 10:00:00', '2026-04-26 10:00:00', 0.71, 1.2, 140, 155, NULL),
(37, 6, 'Track Workout', 'Speed repeats', 7000, 2100, 2500, 15, 'Run', '2026-04-29 18:00:00', '2026-04-29 19:00:00', 'Europe/Berlin', '2026-04-29 18:00:00', '2026-04-29 18:00:00', 3.33, 5.2, 160, 182, 172),
(38, 6, 'Pool Sprints', 'Anaerobic capacity', 1500, 2400, 3000, 0, 'Swim', '2026-05-02 06:30:00', '2026-05-02 07:30:00', 'Europe/Berlin', '2026-05-02 06:30:00', '2026-05-02 06:30:00', 0.62, 1.5, 145, 175, NULL),
(39, 6, 'Long Run', 'Half marathon distance', 21097, 6800, 7000, 180, 'Run', '2026-05-05 08:00:00', '2026-05-05 09:00:00', 'Europe/Berlin', '2026-05-05 08:00:00', '2026-05-05 08:00:00', 3.10, 4.2, 152, 170, 166);

-- 4. INSERT TRAINING PLANS
INSERT INTO "TrainingPlans" ("Id", "AthleteId", "Name", "Description", "StartDate", "EndDate", "Goal", "Notes", "CreatedAt", "UpdatedAt")
VALUES 
(1, 1, 'Ironman Prep', '16-week comprehensive plan', '2026-04-01 00:00:00', '2026-07-20 00:00:00', 1, 'Focus on nutrition during long rides', '2026-03-25 00:00:00', '2026-03-25 00:00:00'),
(2, 2, 'Sub-4 Marathon', '12-week pacing focused plan', '2026-04-10 00:00:00', '2026-07-01 00:00:00', 2, 'Incorporate more tempo runs', '2026-04-05 00:00:00', '2026-04-05 00:00:00'),
(3, 5, 'Strength Building Phase', '8-week hypertrophy block', '2026-04-15 00:00:00', '2026-06-10 00:00:00', 3, 'Progressive overload on major lifts', '2026-04-12 00:00:00', '2026-04-12 00:00:00');

-- 5. INSERT PLANNED ACTIVITIES
INSERT INTO "PlannedActivities" ("Id", "TrainingPlanId", "Title", "Description", "SportType", "PlannedDate", "PlannedDuration", "PlannedDistance", "CompletedActivityId")
VALUES 
-- Training Plan 1 (John Doe)
(1, 1, 'Base Run', 'Zone 2 aerobic effort', 'Run', '2026-04-20 00:00:00', 1800, 5000, 1),
(2, 1, 'Long Brick Workout', '4h ride + 30m run off the bike', 'Workout', '2026-04-22 00:00:00', 16200, 100000, 2),
(3, 1, 'Intervals Swim', '8x100m hard', 'Swim', '2026-04-24 00:00:00', 3600, 2500, 3),
(4, 1, 'Tempo Run', '3x10m tempo pace', 'Run', '2026-04-26 00:00:00', 2700, 8000, 4),
(5, 1, 'Recovery Spin', 'Zone 1 active recovery', 'Ride', '2026-04-28 00:00:00', 3600, 20000, 5),
(6, 1, 'Long Run', 'Build endurance', 'Run', '2026-05-01 00:00:00', 7200, 20000, 6),
(7, 1, 'Upcoming Race Sim', 'Race pace simulation', 'Workout', '2026-05-10 00:00:00', 10800, 0, NULL),

-- Training Plan 2 (Jane Smith)
(8, 2, 'Speed Work', 'Yasso 800s', 'Run', '2026-04-21 00:00:00', 2400, 6000, 8),
(9, 2, 'Easy Miles', 'Conversation pace', 'Run', '2026-04-23 00:00:00', 3600, 10000, 9),
(10, 2, 'Hill Repeats', 'Build power', 'Run', '2026-04-25 00:00:00', 2400, 8000, 10),
(11, 2, 'Recovery Yoga', 'Stretch and mobilize', 'Yoga', '2026-04-27 00:00:00', 2700, 0, 11),
(12, 2, 'Long Endurance', 'Time on feet', 'Run', '2026-04-30 00:00:00', 9000, 24000, 12),
(13, 2, 'Future Tempo', 'Marathon pace block', 'Run', '2026-05-08 00:00:00', 3600, 12000, NULL),

-- Training Plan 3 (Andriy)
(14, 3, 'Heavy Push', '5x5 bench press focus', 'Workout', '2026-04-20 00:00:00', 4500, 0, 27),
(15, 3, 'Heavy Pull', 'Deadlift and rows', 'Workout', '2026-04-22 00:00:00', 4500, 0, 28),
(16, 3, 'Leg Day', 'Squats and lunges', 'Workout', '2026-04-24 00:00:00', 4500, 0, 29),
(17, 3, 'Active Recovery', 'Light cardio', 'Run', '2026-04-26 00:00:00', 1800, 5000, 30),
(18, 3, 'Future PR Attempt', '1RM testing week', 'Workout', '2026-05-15 00:00:00', 5400, 0, NULL);

