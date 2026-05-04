using FitnessAnalyticsHub.Frontend.Models.ApiClient;

namespace FitnessAnalyticsHub.Frontend.Services;

public class DemoDataService
{
    public ActivityStatisticsDto GetStatistics()
    {
        return new ActivityStatisticsDto
        {
            TotalActivities = 47,
            TotalDistance = 342.5,
            TotalDuration = "28h 15m",
            AverageDistance = 7.3,
            LongestDistance = 21.1,
            MostCommonSport = "Run",
            ActivitiesByType = new Dictionary<string, int>
            {
                ["Run"] = 28,
                ["Ride"] = 15,
                ["Swim"] = 4
            },
            ActivitiesByMonth = new Dictionary<string, int>
            {
                ["1"] = 8,
                ["2"] = 12,
                ["3"] = 15,
                ["4"] = 12,
                ["5"] = 8,
                ["6"] = 5
            }
        };
    }

    public List<ActivityDto> GetActivities()
    {
        return new List<ActivityDto>
        {
            new() { Id = 1, Name = "Morning Power Run", SportType = "Run", Distance = 5.2, StartDate = DateTime.UtcNow.AddHours(-2).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 1680, AverageHeartRate = 145, MaxHeartRate = 168, Calories = 420 },
            new() { Id = 2, Name = "Weekend Bike Adventure", SportType = "Ride", Distance = 24.8, StartDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 4500, AverageHeartRate = 132, MaxHeartRate = 156, Calories = 890 },
            new() { Id = 3, Name = "Pool Training Session", SportType = "Swim", Distance = 1.5, StartDate = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 2700, AverageHeartRate = 125, MaxHeartRate = 148, Calories = 380 },
            new() { Id = 4, Name = "Evening Recovery Jog", SportType = "Run", Distance = 3.1, StartDate = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 1080, AverageHeartRate = 128, MaxHeartRate = 142, Calories = 245 },
            new() { Id = 5, Name = "Mountain Trail Run", SportType = "Hike", Distance = 8.7, StartDate = DateTime.UtcNow.AddDays(-4).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 3240, AverageHeartRate = 138, MaxHeartRate = 162, Calories = 580 },
            new() { Id = 6, Name = "Strength & Cardio Combo", SportType = "Workout", Distance = 0, StartDate = DateTime.UtcNow.AddDays(-5).ToString("yyyy-MM-ddTHH:mm:ss"), AthleteFullName = "Demo User", MovingTime = 2400, AverageHeartRate = 142, MaxHeartRate = 165, Calories = 350 }
        };
    }

    public List<WorkoutDataDto> GetWorkouts()
    {
        return new List<WorkoutDataDto>
        {
            new() { Date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"), ActivityType = "Run", Distance = 5.2, MovingTime = 1680, Calories = 420 },
            new() { Date = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss"), ActivityType = "Ride", Distance = 24.8, MovingTime = 4500, Calories = 890 },
            new() { Date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss"), ActivityType = "Run", Distance = 3.1, MovingTime = 1080, Calories = 245 }
        };
    }

    public AthleteDto GetDemoAthlete()
    {
        return new AthleteDto { Id = 1, FirstName = "Demo", LastName = "User" };
    }
}
