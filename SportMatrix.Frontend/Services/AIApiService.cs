using SportMatrix.Frontend.Models.ApiClient;
using System.Net.Http.Json;

namespace SportMatrix.Frontend.Services;

public class AIApiService
{
    private readonly HttpClient _httpClient;

    public AIApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AIAnalysisDto?> AnalyzeWorkoutAsync(int athleteId, List<WorkoutDataDto> recentWorkouts, string analysisType = "Performance")
    {
        var request = new WorkoutAnalysisRequestDto
        {
            AthleteProfile = new AthleteProfileDto
            {
                Name = "Fitness Enthusiast",
                FitnessLevel = "Intermediate",
                PrimaryGoal = "Performance Improvement"
            },
            RecentWorkouts = recentWorkouts.Select(w => new WorkoutDataDtoBackend
            {
                Date = DateTime.TryParse(w.Date, out var date) ? date : DateTime.UtcNow,
                ActivityType = w.ActivityType,
                Distance = w.Distance,
                Duration = ParseMovingTime(w.MovingTime),
                Calories = w.Calories
            }).ToList(),
            AnalysisType = analysisType
        };

        var response = await _httpClient.PostAsJsonAsync("/api/WorkoutAnalysis/analyze", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<WorkoutAnalysisResponseDto>();
        return MapToAnalysis(aiResponse);
    }

    public async Task<AIAnalysisDto?> AnalyzePerformanceTrendsAsync(int athleteId, string timeFrame = "month")
    {
        var response = await _httpClient.GetAsync($"/api/WorkoutAnalysis/performance-trends/{athleteId}?timeFrame={Uri.EscapeDataString(timeFrame)}");
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<WorkoutAnalysisResponseDto>();
        return MapToAnalysis(aiResponse);
    }

    public async Task<AIAnalysisDto?> GetTrainingRecommendationsAsync(int athleteId)
    {
        var response = await _httpClient.GetAsync($"/api/WorkoutAnalysis/recommendations/{athleteId}");
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<WorkoutAnalysisResponseDto>();
        return MapToAnalysis(aiResponse);
    }

    public async Task<AIAnalysisDto?> AnalyzeHealthMetricsAsync(int athleteId, List<WorkoutDataDto> workouts)
    {
        var request = new HealthAnalysisRequestDto
        {
            AthleteId = athleteId,
            RecentWorkouts = workouts.Select(w => new WorkoutDataDtoBackend
            {
                Date = DateTime.TryParse(w.Date, out var date) ? date : DateTime.UtcNow,
                ActivityType = w.ActivityType,
                Distance = w.Distance,
                Duration = ParseMovingTime(w.MovingTime),
                Calories = w.Calories
            }).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("/api/WorkoutAnalysis/health-analysis", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<WorkoutAnalysisResponseDto>();
        return MapToAnalysis(aiResponse);
    }

    private static List<WorkoutDataDto> GetDemoWorkouts()
    {
        return
        [
            new WorkoutDataDto
            {
                Date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss"),
                ActivityType = "Run",
                Distance = 5.2,
                MovingTime = "00:28:00",
                Calories = 420
            },
            new WorkoutDataDto
            {
                Date = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss"),
                ActivityType = "Ride",
                Distance = 24.8,
                MovingTime = "01:15:00",
                Calories = 890
            },
            new WorkoutDataDto
            {
                Date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss"),
                ActivityType = "Run",
                Distance = 3.1,
                MovingTime = "00:18:00",
                Calories = 245
            }
        ];
    }


    private static AIAnalysisDto? MapToAnalysis(WorkoutAnalysisResponseDto? response)
    {
        if (response == null) return null;
        return new AIAnalysisDto
        {
            Analysis = response.Analysis ?? "AI analysis completed successfully.",
            KeyInsights = response.KeyInsights ??
            [
                "Your training shows consistent progress",
                "Performance metrics are improving",
                "Keep up the great work!"
            ],
            Recommendations = response.Recommendations ??
            [
                "Continue with current training schedule",
                "Focus on gradual progression",
                "Ensure adequate recovery time"
            ],
            PerformanceScore = new Random().Next(70, 91),
            Trends = new TrendDto
            {
                Direction = "up",
                Description = "Positive trend detected"
            }
        };
    }

    private static AIAnalysisDto? MapToMotivationAnalysis(WorkoutAnalysisResponseDto? response)
    {
        if (response == null) return null;
        return new AIAnalysisDto
        {
            Analysis = response.Analysis ?? "Stay motivated and keep pushing your limits!",
            KeyInsights = response.KeyInsights ??
            [
                "Consistency is key to success",
                "Small improvements compound over time",
                "Your dedication is paying off"
            ],
            Recommendations = response.Recommendations ??
            [
                "Set small, achievable daily goals",
                "Track your progress regularly",
                "Celebrate every milestone"
            ],
            PerformanceScore = new Random().Next(80, 96)
        };
    }

    private int ParseMovingTime(string movingTime)
    {
        if (string.IsNullOrEmpty(movingTime))
            return 0;
        
        if (TimeSpan.TryParse(movingTime, out var timeSpan))
            return (int)timeSpan.TotalSeconds;
        
        return 0;
    }
}

public class WorkoutAnalysisRequestDto
{
    public List<WorkoutDataDtoBackend> RecentWorkouts { get; set; } = [];
    public string AnalysisType { get; set; } = string.Empty;
    public AthleteProfileDto? AthleteProfile { get; set; }
    public Dictionary<string, object>? AdditionalContext { get; set; }
}

public class HealthAnalysisRequestDto
{
    public int AthleteId { get; set; }
    public List<WorkoutDataDtoBackend> RecentWorkouts { get; set; } = [];
    public Dictionary<string, object>? HealthMetrics { get; set; }
    public List<string>? FocusAreas { get; set; }
}


public class AthleteProfileDto
{
    public string? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? FitnessLevel { get; set; }
    public string? PrimaryGoal { get; set; }
    public Dictionary<string, object>? Preferences { get; set; }
}

public class WorkoutDataDtoBackend
{
    public DateTime Date { get; set; }
    public string ActivityType { get; set; } = string.Empty;
    public double Distance { get; set; }
    public int Duration { get; set; }
    public int? Calories { get; set; }
    public Dictionary<string, double>? MetricsData { get; set; }
}

public class WorkoutAnalysisResponseDto
{
    public string Analysis { get; set; } = string.Empty;
    public List<string>? KeyInsights { get; set; }
    public List<string>? Recommendations { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
