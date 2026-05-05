namespace SportMatrix.AIAssistant.Infrastructure.Providers;

using SportMatrix.AIAssistant.Application.DTOs;

/// <summary>
/// Provides consistent demo/fallback data for development and testing.
/// Used by controllers and gRPC services when no real data is available.
/// </summary>
public static class DemoDataProvider
{
    public static List<WorkoutDataDto> GetDemoWorkouts()
    {
        return new List<WorkoutDataDto>
        {
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-1),
                ActivityType = "Run",
                Distance = 5.2,
                Duration = 1800,
                Calories = 350,
                MetricsData = new Dictionary<string, double> { { "heartRate", 145 } },
            },
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-3),
                ActivityType = "Ride",
                Distance = 24.8,
                Duration = 4500,
                Calories = 890,
                MetricsData = new Dictionary<string, double> { { "heartRate", 132 } },
            },
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-5),
                ActivityType = "Run",
                Distance = 3.1,
                Duration = 1080,
                Calories = 245,
                MetricsData = new Dictionary<string, double> { { "heartRate", 128 } },
            },
        };
    }

    public static AthleteProfileDto GetDemoAthleteProfile(int athleteId)
    {
        return new AthleteProfileDto
        {
            Id = athleteId.ToString(),
            Name = "Demo User",
            FitnessLevel = "Intermediate",
            PrimaryGoal = "Endurance Improvement",
            Preferences = new Dictionary<string, object>
            {
                { "preferredActivities", new[] { "Run", "Ride" } },
                { "trainingDays", 4 },
            },
        };
    }
}
