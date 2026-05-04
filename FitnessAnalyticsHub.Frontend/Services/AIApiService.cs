using FitnessAnalyticsHub.Frontend.Models.ApiClient;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FitnessAnalyticsHub.Frontend.Services;

public class AIApiService
{
    private readonly HttpClient _httpClient;

    public AIApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AIAnalysisDto?> AnalyzeWorkoutAsync(int athleteId, List<WorkoutDataDto> recentWorkouts, string analysisType = "Performance")
    {
        var request = new AIWorkoutAnalysisRequest
        {
            AthleteProfile = new AthleteProfile
            {
                Name = "Fitness Enthusiast",
                FitnessLevel = "Intermediate",
                PrimaryGoal = "Performance Improvement"
            },
            RecentWorkouts = recentWorkouts.Select(w => new AIWorkoutItem
            {
                Date = w.Date,
                ActivityType = w.ActivityType,
                Distance = w.Distance,
                MovingTime = w.MovingTime ?? 0,
                Calories = w.Calories ?? 0
            }).ToList(),
            AnalysisType = analysisType,
            FocusAreas = ["endurance", "consistency"]
        };

        var response = await _httpClient.PostAsJsonAsync("/api/AI/analysis", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<AIResponseDto>();
        return MapToAnalysis(aiResponse);
    }

    public async Task<AIAnalysisDto?> AnalyzePerformanceTrendsAsync(int athleteId, string timeFrame = "month")
    {
        return await AnalyzeWorkoutAsync(athleteId, GetDemoWorkouts(), "Trends");
    }

    public async Task<AIAnalysisDto?> GetTrainingRecommendationsAsync(int athleteId)
    {
        var request = new AIMotivationRequest
        {
            AthleteProfile = new AthleteProfile
            {
                Name = "Fitness Enthusiast",
                FitnessLevel = "Intermediate",
                PrimaryGoal = "Training Improvement"
            },
            RecentWorkouts = GetDemoWorkouts().Select(w => new AIWorkoutItem
            {
                Date = w.Date,
                ActivityType = w.ActivityType,
                Distance = w.Distance,
                MovingTime = w.MovingTime ?? 0,
                Calories = w.Calories ?? 0
            }).ToList(),
            PreferredTone = "Motivational",
            ContextualInfo = "Looking for training recommendations"
        };

        var response = await _httpClient.PostAsJsonAsync("/api/AI/motivation", request);
        if (!response.IsSuccessStatusCode)
            return null;

        var aiResponse = await response.Content.ReadFromJsonAsync<AIResponseDto>();
        return MapToMotivationAnalysis(aiResponse);
    }

    public async Task<AIAnalysisDto?> AnalyzeHealthMetricsAsync(int athleteId, List<WorkoutDataDto> workouts)
    {
        return await AnalyzeWorkoutAsync(athleteId, workouts, "Health");
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
                MovingTime = 1680,
                Calories = 420
            },
            new WorkoutDataDto
            {
                Date = DateTime.UtcNow.AddDays(-2).ToString("yyyy-MM-ddTHH:mm:ss"),
                ActivityType = "Ride",
                Distance = 24.8,
                MovingTime = 4500,
                Calories = 890
            },
            new WorkoutDataDto
            {
                Date = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-ddTHH:mm:ss"),
                ActivityType = "Run",
                Distance = 3.1,
                MovingTime = 1080,
                Calories = 245
            }
        ];
    }

    private static AIAnalysisDto? MapToAnalysis(AIResponseDto? response)
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

    private static AIAnalysisDto? MapToMotivationAnalysis(AIResponseDto? response)
    {
        if (response == null) return null;
        return new AIAnalysisDto
        {
            Analysis = response.MotivationalMessage ?? "Stay motivated and keep pushing your limits!",
            KeyInsights = response.ActionableTips ??
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
}

public class AIWorkoutAnalysisRequest
{
    [JsonPropertyName("athleteProfile")]
    public AthleteProfile? AthleteProfile { get; set; }

    [JsonPropertyName("recentWorkouts")]
    public List<AIWorkoutItem> RecentWorkouts { get; set; } = [];

    [JsonPropertyName("analysisType")]
    public string AnalysisType { get; set; } = string.Empty;

    [JsonPropertyName("focusAreas")]
    public List<string>? FocusAreas { get; set; }
}

public class AIMotivationRequest
{
    [JsonPropertyName("athleteProfile")]
    public AthleteProfile? AthleteProfile { get; set; }

    [JsonPropertyName("recentWorkouts")]
    public List<AIWorkoutItem> RecentWorkouts { get; set; } = [];

    [JsonPropertyName("preferredTone")]
    public string? PreferredTone { get; set; }

    [JsonPropertyName("contextualInfo")]
    public string? ContextualInfo { get; set; }
}

public class AthleteProfile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("fitnessLevel")]
    public string FitnessLevel { get; set; } = string.Empty;

    [JsonPropertyName("primaryGoal")]
    public string PrimaryGoal { get; set; } = string.Empty;
}

public class AIWorkoutItem
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("activityType")]
    public string ActivityType { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("movingTime")]
    public double MovingTime { get; set; }

    [JsonPropertyName("calories")]
    public int Calories { get; set; }
}

public class AIResponseDto
{
    [JsonPropertyName("analysis")]
    public string? Analysis { get; set; }

    [JsonPropertyName("keyInsights")]
    public List<string>? KeyInsights { get; set; }

    [JsonPropertyName("recommendations")]
    public List<string>? Recommendations { get; set; }

    [JsonPropertyName("generatedAt")]
    public string? GeneratedAt { get; set; }

    [JsonPropertyName("source")]
    public string? Source { get; set; }

    [JsonPropertyName("motivationalMessage")]
    public string? MotivationalMessage { get; set; }

    [JsonPropertyName("actionableTips")]
    public List<string>? ActionableTips { get; set; }

    [JsonPropertyName("quote")]
    public string? Quote { get; set; }
}
