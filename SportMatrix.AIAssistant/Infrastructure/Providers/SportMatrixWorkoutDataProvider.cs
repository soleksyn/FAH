namespace SportMatrix.AIAssistant.Infrastructure.Providers;

using System.Net.Http.Json;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;

public class SportMatrixWorkoutDataProvider : IWorkoutDataProvider
{
    private readonly HttpClient httpClient;
    private readonly ILogger<SportMatrixWorkoutDataProvider> logger;

    public SportMatrixWorkoutDataProvider(
        HttpClient httpClient,
        ILogger<SportMatrixWorkoutDataProvider> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<List<WorkoutDataDto>> GetRecentWorkoutsAsync(
        int athleteId,
        TimeSpan lookback,
        CancellationToken cancellationToken)
    {
        DateTime cutoff = DateTime.UtcNow.Subtract(lookback);
        List<WorkoutDataDto> workouts = await this.GetAllWorkoutsAsync(athleteId, cancellationToken);

        return workouts
            .Where(workout => workout.Date >= cutoff)
            .OrderByDescending(workout => workout.Date)
            .ToList();
    }

    public async Task<List<WorkoutDataDto>> GetLatestWorkoutsAsync(
        int athleteId,
        int count,
        CancellationToken cancellationToken)
    {
        List<WorkoutDataDto> workouts = await this.GetAllWorkoutsAsync(athleteId, cancellationToken);

        return workouts
            .OrderByDescending(workout => workout.Date)
            .Take(count)
            .ToList();
    }

    public AthleteProfileDto BuildAthleteProfile(int athleteId, IReadOnlyCollection<WorkoutDataDto> workouts)
    {
        string[] preferredActivities = workouts
            .GroupBy(workout => workout.ActivityType)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .Where(activityType => !string.IsNullOrWhiteSpace(activityType))
            .ToArray();

        return new AthleteProfileDto
        {
            Id = athleteId.ToString(),
            Name = $"Athlete {athleteId}",
            Preferences = new Dictionary<string, object>
            {
                { "preferredActivities", preferredActivities },
                { "recentWorkoutCount", workouts.Count },
            },
        };
    }

    private async Task<List<WorkoutDataDto>> GetAllWorkoutsAsync(int athleteId, CancellationToken cancellationToken)
    {
        try
        {
            List<ActivityDto>? activities = await this.httpClient.GetFromJsonAsync<List<ActivityDto>>(
                $"api/activity/athlete/{athleteId}",
                cancellationToken);

            return activities?
                .Select(this.MapToWorkoutData)
                .OrderByDescending(workout => workout.Date)
                .ToList() ?? new List<WorkoutDataDto>();
        }
        catch (HttpRequestException ex)
        {
            this.logger.LogError(ex, "Failed to fetch activities for athlete {AthleteId}", athleteId);
            throw;
        }
    }

    private WorkoutDataDto MapToWorkoutData(ActivityDto activity)
    {
        Dictionary<string, double> metricsData = new Dictionary<string, double>();

        AddMetric(metricsData, "averageHeartRate", activity.AverageHeartRate);
        AddMetric(metricsData, "maxHeartRate", activity.MaxHeartRate);
        AddMetric(metricsData, "averageSpeed", activity.AverageSpeed);
        AddMetric(metricsData, "maxSpeed", activity.MaxSpeed);
        AddMetric(metricsData, "averagePower", activity.AveragePower);
        AddMetric(metricsData, "maxPower", activity.MaxPower);
        AddMetric(metricsData, "averageCadence", activity.AverageCadence);
        AddMetric(metricsData, "totalElevationGain", activity.TotalElevationGain);

        return new WorkoutDataDto
        {
            Date = activity.StartDate,
            ActivityType = activity.ActivityType.ToString(),
            Distance = activity.Distance,
            Duration = Math.Max(1, (int)activity.MovingTime.TotalSeconds),
            Calories = EstimateCalories(activity),
            AverageHeartRate = activity.AverageHeartRate,
            MetricsData = metricsData.Count > 0 ? metricsData : null,
        };
    }

    private static int EstimateCalories(ActivityDto activity)
    {
        double durationHours = Math.Max(1, activity.MovingTime.TotalSeconds) / 3600.0;
        double weightKg = activity.AthleteWeight.GetValueOrDefault(75);
        double met = GetMetValue(activity.ActivityType, activity.Distance, activity.MovingTime);

        return Math.Max(1, (int)Math.Round(met * weightKg * durationHours));
    }

    private static double GetMetValue(SportMatrix.Domain.Enums.ActivityType activityType, double distance, TimeSpan movingTime)
    {
        string normalizedSportType = activityType.ToString().ToLowerInvariant();
        double speedKmh = movingTime.TotalHours > 0 ? (distance / 1000.0) / movingTime.TotalHours : 0;

        return normalizedSportType switch
        {
            "run" or "running" => speedKmh >= 12 ? 12.5 : speedKmh >= 9.5 ? 10.0 : 8.3,
            "ride" or "cycling" or "bike" or "biking" => speedKmh >= 25 ? 10.0 : speedKmh >= 16 ? 8.0 : 6.8,
            "swim" or "swimming" => 8.0,
            "walk" or "walking" => 3.8,
            "hike" or "hiking" => 6.0,
            "workout" or "weighttraining" or "crosstraining" => 6.0,
            "yoga" => 2.5,
            _ => 5.0,
        };
    }

    private static void AddMetric(Dictionary<string, double> metricsData, string name, double? value)
    {
        if (value.HasValue)
        {
            metricsData[name] = value.Value;
        }
    }

    private class ActivityDto
    {
        public double Distance { get; set; }

        public TimeSpan MovingTime { get; set; }

        public double TotalElevationGain { get; set; }

        public SportMatrix.Domain.Enums.ActivityType ActivityType { get; set; }

        public DateTime StartDate { get; set; }

        public double? AverageSpeed { get; set; }

        public double? MaxSpeed { get; set; }

        public int? AverageHeartRate { get; set; }

        public int? MaxHeartRate { get; set; }

        public double? AveragePower { get; set; }

        public double? MaxPower { get; set; }

        public double? AverageCadence { get; set; }

        public double? AthleteWeight { get; set; }
    }
}
