using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using SportMatrix.Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class DashboardController : Controller
{
    private readonly AthleteApiService _athleteService;
    private readonly FitnessApiService _fitnessService;
    private readonly AIApiService _aiService;
    private readonly DemoDataService _demoDataService;

    public DashboardController(AthleteApiService athleteService, FitnessApiService fitnessService, AIApiService aiService, DemoDataService demoDataService)
    {
        _athleteService = athleteService;
        _fitnessService = fitnessService;
        _aiService = aiService;
        _demoDataService = demoDataService;
    }

    // GET: Dashboard
    public async Task<IActionResult> Index(int id = 1, string? action = null)
    {
        var viewModel = new FitnessDashboardViewModel { Loading = true };
        try
        {
            viewModel.Athlete = await LoadAthleteWithFallbackAsync(id);
            viewModel.Statistics = await LoadStatisticsAsync(id);
            viewModel.RecentActivities = await LoadActivitiesAsync(id);

            if (action != null)
            {
                viewModel.AIAnalysis = await HandleAIAnalysisAsync(id, action, viewModel.RecentActivities);
            }
        }
        catch (Exception ex)
        {
            viewModel = BuildFallbackViewModel(ex.Message);
        }
        finally
        {
            viewModel.Loading = false;
        }
        return View(viewModel);
    }

    private async Task<AthleteDto> LoadAthleteWithFallbackAsync(int id)
    {
        var athlete = await _athleteService.GetAthleteByIdAsync(id);
        if (athlete == null)
        {
            athlete = _demoDataService.GetDemoAthlete();
        }
        return athlete;
    }

    private async Task<ActivityStatisticsDto> LoadStatisticsAsync(int id)
    {
        var stats = await _fitnessService.GetAthleteStatisticsAsync(id);
        return stats ?? _demoDataService.GetStatistics();
    }

    private async Task<List<ActivityDto>> LoadActivitiesAsync(int id)
    {
        var activities = await _fitnessService.GetRecentActivitiesAsync(id, 6);
        return activities.Count == 0 ? _demoDataService.GetActivities() : activities;
    }

    private async Task<AIAnalysisDto?> HandleAIAnalysisAsync(int id, string action, List<ActivityDto> activities)
    {
        return action switch
        {
            "analyze" => await GetAnalysisForFirstActivity(activities),
            "trends" => await _aiService.AnalyzePerformanceTrendsAsync(id),
            "recommendations" => await _aiService.GetTrainingRecommendationsAsync(id),
            "health" => await _aiService.AnalyzeHealthMetricsAsync(id, _demoDataService.GetWorkouts()),
            _ => null
        };
    }

    private FitnessDashboardViewModel BuildFallbackViewModel(string errorMessage)
    {
        return new FitnessDashboardViewModel
        {
            Error = $"Error loading dashboard data: {errorMessage}",
            Athlete = _demoDataService.GetDemoAthlete(),
            Statistics = _demoDataService.GetStatistics(),
            RecentActivities = _demoDataService.GetActivities()
        };
    }

    // POST: Dashboard/AnalyzeWorkout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeWorkout(int id, int activityId)
    {
        return RedirectToAction(nameof(Index), new { id, action = "analyze" });
    }

    // POST: Dashboard/GetPerformanceTrends
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPerformanceTrends(int id)
    {
        return RedirectToAction(nameof(Index), new { id, action = "trends" });
    }

    // POST: Dashboard/GetTrainingRecommendations
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetTrainingRecommendations(int id)
    {
        return RedirectToAction(nameof(Index), new { id, action = "recommendations" });
    }

    // POST: Dashboard/AnalyzeHealthMetrics
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeHealthMetrics(int id)
    {
        return RedirectToAction(nameof(Index), new { id, action = "health" });
    }

    private async Task<AIAnalysisDto?> GetAnalysisForFirstActivity(List<ActivityDto> activities)
    {
        if (activities.Count == 0)
            return null;

        var activity = activities.First();
        var workout = new WorkoutDataDto
        {
            Date = activity.StartDate,
            ActivityType = activity.SportType,
            Distance = activity.Distance,
            MovingTime = activity.MovingTime,
            HeartRate = activity.AverageHeartRate,
            Calories = activity.Calories
        };

        return await _aiService.AnalyzeWorkoutAsync(1, new List<WorkoutDataDto> { workout }, "Performance");
    }
}
