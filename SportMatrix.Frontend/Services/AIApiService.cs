using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Domain.Enums;
using System.Net.Http.Json;

namespace SportMatrix.Frontend.Services;

public class AIApiService
{
    private readonly HttpClient _httpClient;
    private readonly DemoDataService _demoDataService;

    public AIApiService(HttpClient httpClient, DemoDataService demoDataService)
    {
        _httpClient = httpClient;
        _demoDataService = demoDataService;
    }

    public async Task<AIAnalysisDto?> AnalyzeWorkoutAsync(int athleteId, List<WorkoutDataDto> recentWorkouts, AnalysisType analysisType = AnalysisType.Performance)
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

    public async Task<AIAnalysisDto?> GetNextDayRecommendationAsync(int athleteId, List<ActivityDto> activities)
    {
        if (activities.Count == 0)
            return null;

        var lastActivity = activities.First();
        var workout = new WorkoutDataDto
        {
            Date = lastActivity.StartDate,
            ActivityType = lastActivity.SportType,
            Distance = lastActivity.Distance,
            MovingTime = lastActivity.MovingTime,
            HeartRate = lastActivity.AverageHeartRate,
            Calories = lastActivity.Calories
        };

        return await AnalyzeWorkoutAsync(athleteId, new List<WorkoutDataDto> { workout }, AnalysisType.NextDay);
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

    private List<WorkoutDataDto> GetDemoWorkouts()
    {
        return _demoDataService.GetWorkouts();
    }


    private static AIAnalysisDto? MapToAnalysis(WorkoutAnalysisResponseDto? response)
    {
        if (response == null) return null;
        return new AIAnalysisDto
        {
            Analysis = response.Analysis,
            KeyInsights = response.KeyInsights,
            Recommendations = response.Recommendations,
            PerformanceScore = null,
            Trends = null
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
    public AnalysisType AnalysisType { get; set; } = AnalysisType.Performance;
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
