-- Mock Data Insertion Script
-- This script adds approximately 15 new rows to existing seed data
-- Total will be around 25 rows across all tables

-- Insert additional Athletes (3 new athletes)
INSERT INTO "Athletes" ("Id", "FirstName", "LastName", "Username", "Email", "City", "Country", "ProfilePictureUrl", "CreatedAt", "UpdatedAt")
VALUES 
(3, 'Oleksandr', 'Petrenko', 'oleksandrp', 'oleksandr.petrenko@example.com', 'Odessa', 'Ukraine', 'https://example.com/oleksandr.jpg', '2026-01-15 00:00:00', '2026-01-15 00:00:00'),
(4, 'Maria', 'Kovalenko', 'mariak', 'maria.kovalenko@example.com', 'Kharkiv', 'Ukraine', 'https://example.com/maria.jpg', '2026-01-20 00:00:00', '2026-01-20 00:00:00'),
(5, 'Andriy', 'Bohdan', 'andriyb', 'andriy.bohdan@example.com', 'Dnipro', 'Ukraine', 'https://example.com/andriy.jpg', '2026-02-01 00:00:00', '2026-02-01 00:00:00');

-- Insert additional Activities (5 new activities)
INSERT INTO "Activities" ("Id", "AthleteId", "Name", "Description", "Distance", "MovingTime", "ElapsedTime", "TotalElevationGain", "SportType", "StartDate", "StartDateLocal", "Timezone", "CreatedAt", "UpdatedAt", "AverageSpeed", "MaxSpeed", "AverageHeartRate", "MaxHeartRate", "AverageCadence")
VALUES 
(4, 3, 'Evening Jog', '3km evening jog around the neighborhood', 3000, 1200, 1200, 30, 'Run', '2026-05-01 18:00:00', '2026-05-01 21:00:00', 'Europe/Kiev', '2026-05-01 18:00:00', '2026-05-01 18:00:00', 2.5, 3.2, 135, 155, 170),
(5, 3, 'Mountain Biking', '15km mountain bike trail', 15000, 2700, 3000, 350, 'Ride', '2026-05-02 08:00:00', '2026-05-02 11:00:00', 'Europe/Kiev', '2026-05-02 08:00:00', '2026-05-02 08:00:00', 5.56, 12.0, 145, 170, 85),
(6, 4, 'Yoga Session', '60-minute yoga practice', 0, 3600, 3600, 0, 'Yoga', '2026-05-03 07:00:00', '2026-05-03 10:00:00', 'Europe/Kiev', '2026-05-03 07:00:00', '2026-05-03 07:00:00', NULL, NULL, 90, 110, NULL),
(7, 4, 'HIIT Workout', 'High intensity interval training', 0, 1800, 1800, 0, 'Workout', '2026-05-04 18:30:00', '2026-05-04 21:30:00', 'Europe/Kiev', '2026-05-04 18:30:00', '2026-05-04 18:30:00', NULL, NULL, 155, 175, NULL),
(8, 5, 'Long Distance Run', '10km long distance run', 10000, 3600, 3600, 80, 'Run', '2026-05-05 06:00:00', '2026-05-05 09:00:00', 'Europe/Kiev', '2026-05-05 06:00:00', '2026-05-05 06:00:00', 2.78, 3.8, 150, 172, 175);

-- Insert additional TrainingPlans (2 new training plans)
INSERT INTO "TrainingPlans" ("Id", "AthleteId", "Name", "Description", "StartDate", "EndDate", "Goal", "Notes", "CreatedAt", "UpdatedAt")
VALUES 
(3, 3, 'Weight Loss Program', '6-week weight loss focused training', '2026-05-06 00:00:00', '2026-06-17 00:00:00', 1, 'Focus on cardio and HIIT', '2026-05-06 00:00:00', '2026-05-06 00:00:00'),
(4, 4, 'Strength Building', '8-week strength improvement plan', '2026-05-06 00:00:00', '2026-07-01 00:00:00', 3, 'Progressive overload approach', '2026-05-06 00:00:00', '2026-05-06 00:00:00');

-- Insert additional PlannedActivities (5 new planned activities)
INSERT INTO "PlannedActivities" ("Id", "TrainingPlanId", "Title", "Description", "SportType", "PlannedDate", "PlannedDuration", "PlannedDistance", "CompletedActivityId")
VALUES 
(4, 3, 'Cardio Session', '45-minute cardio workout', 'Run', '2026-05-08 00:00:00', 2700, 5000, NULL),
(5, 3, 'HIIT Training', '30-minute high intensity intervals', 'Workout', '2026-05-10 00:00:00', 1800, NULL, NULL),
(6, 3, 'Recovery Run', 'Easy 3km recovery run', 'Run', '2026-05-12 00:00:00', 1200, 3000, NULL),
(7, 4, 'Upper Body Strength', 'Strength training focus on upper body', 'Workout', '2026-05-09 00:00:00', 3600, NULL, NULL),
(8, 4, 'Lower Body Strength', 'Strength training focus on lower body', 'Workout', '2026-05-11 00:00:00', 3600, NULL, NULL);
