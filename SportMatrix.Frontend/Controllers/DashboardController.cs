using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using SportMatrix.Frontend.Models;
using SportMatrix.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class DashboardController : Controller
{
    private readonly AthleteApiService _athleteService;
    private readonly FitnessApiService _fitnessService;
    private readonly AIApiService _aiService;
    private readonly DemoDataService _demoDataService;
    private readonly DashboardViewModelFactory _viewModelFactory;

    public DashboardController(
        AthleteApiService athleteService,
        FitnessApiService fitnessService,
        AIApiService aiService,
        DemoDataService demoDataService,
        DashboardViewModelFactory viewModelFactory)
    {
        _athleteService = athleteService;
        _fitnessService = fitnessService;
        _aiService = aiService;
        _demoDataService = demoDataService;
        _viewModelFactory = viewModelFactory;
    }

    // GET: Dashboard
    public async Task<IActionResult> Index(int id = 1, AnalysisType? analysisType = null, int? activityId = null, int page = 1)
    {
        var alerts = new List<DashboardAlertViewModel>();
        AthleteDto? athlete = null;
        ActivityStatisticsDto? statistics = null;
        var activities = new List<ActivityDto>();
        var allActivitiesForCharts = new List<ActivityDto>();
        AIAnalysisDto? analysis = null;
        var isDemoData = false;

        int totalActivities = 0;
        int totalPages = 1;
        const int pageSize = 7;

        try
        {
            athlete = await _athleteService.GetAthleteByIdAsync(id);
            statistics = await _fitnessService.GetAthleteStatisticsAsync(id);

            var allActivities = await _fitnessService.GetAthleteActivitiesAsync(id);
            allActivitiesForCharts = allActivities;
            totalActivities = allActivities.Count;
            totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            page = Math.Max(1, Math.Min(page, totalPages));
            activities = allActivities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (analysisType.HasValue)
            {
                analysis = await HandleAIAnalysisAsync(id, analysisType.Value, activities, activityId);
                if (analysis == null)
                {
                    alerts.Add(_viewModelFactory.CreateAlert("AI analysis is currently unavailable for this request.", "warning"));
                }
            }
        }
        catch (Exception ex)
        {
            alerts.Add(_viewModelFactory.CreateAlert($"Error loading dashboard data: {ex.Message}"));
        }

        if (athlete == null)
        {
            athlete = _demoDataService.GetDemoAthlete();
            alerts.Add(_viewModelFactory.CreateAlert("Athlete data is unavailable, so demo profile data is shown.", "warning"));
            isDemoData = true;
        }

        if (statistics == null)
        {
            statistics = _demoDataService.GetStatistics();
            alerts.Add(_viewModelFactory.CreateAlert("Statistics are unavailable, so demo chart data is shown.", "warning"));
            isDemoData = true;
        }

        if (activities.Count == 0)
        {
            var demoActivities = _demoDataService.GetActivities();
            allActivitiesForCharts = demoActivities;
            totalActivities = demoActivities.Count;
            totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            
            page = Math.Max(1, Math.Min(page, totalPages));
            activities = demoActivities.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            alerts.Add(_viewModelFactory.CreateAlert("Recent activities are unavailable, so demo activity data is shown.", "warning"));
            isDemoData = true;
        }

        var viewModel = _viewModelFactory.Create(athlete, statistics, activities, analysis, alerts, isDemoData, page, totalPages, totalActivities, allActivitiesForCharts);
        return View(viewModel);
    }

    private async Task<AIAnalysisDto?> HandleAIAnalysisAsync(int id, AnalysisType analysisType, List<ActivityDto> activities, int? activityId)
    {
        return analysisType switch
        {
            AnalysisType.Performance => await GetAnalysisForActivity(id, activities, activityId),
            AnalysisType.Trends => await _aiService.AnalyzePerformanceTrendsAsync(id),
            AnalysisType.Recommendations => await _aiService.GetTrainingRecommendationsAsync(id),
            AnalysisType.Health => await _aiService.AnalyzeHealthMetricsAsync(id, activities.Select(MapWorkout).ToList()),
            AnalysisType.NextDay => await _aiService.GetNextDayRecommendationAsync(id, activities),
            _ => null
        };
    }

    // POST: Dashboard/AnalyzeWorkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeWorkout(int id, int activityId)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.Performance, activityId });
    }

    // POST: Dashboard/GetPerformanceTrends
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPerformanceTrends(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.Trends });
    }

    // POST: Dashboard/GetTrainingRecommendations
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetTrainingRecommendations(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.Recommendations });
    }

    // POST: Dashboard/AnalyzeHealthMetrics
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeHealthMetrics(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.Health });
    }

    // POST: Dashboard/GetNextDayRecommendation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetNextDayRecommendation(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.NextDay });
    }

    private async Task<AIAnalysisDto?> GetAnalysisForActivity(int athleteId, List<ActivityDto> activities, int? activityId)
    {
        if (activities.Count == 0)
            return null;

        var activity = activityId.HasValue
            ? activities.FirstOrDefault(item => item.Id == activityId.Value)
            : activities.FirstOrDefault();

        if (activity == null)
            return null;

        var workout = MapWorkout(activity);

        return await _aiService.AnalyzeWorkoutAsync(athleteId, new List<WorkoutDataDto> { workout }, AnalysisType.Performance);
    }

    private static WorkoutDataDto MapWorkout(ActivityDto activity)
    {
        return new WorkoutDataDto
        {
            Date = activity.StartDate,
            ActivityType = activity.SportType,
            Distance = activity.Distance,
            MovingTime = activity.MovingTime,
            HeartRate = activity.AverageHeartRate,
            Calories = activity.Calories
        };
    }
}
