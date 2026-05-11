using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using SportMatrix.Frontend.Models;
using SportMatrix.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

[Authorize]
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
    public async Task<IActionResult> Index(int? id = null, AnalysisType? analysisType = null, int? activityId = null, int page = 1)
    {
        var alerts = new List<DashboardAlertViewModel>();
        AthleteDto? athlete = null;

        // If user is not an admin, they can ONLY see their own data
        if (!User.IsInRole("Admin"))
        {
            var userEmail = User.Identity!.Name!;
            athlete = await _athleteService.GetAthleteByEmailAsync(userEmail);
            
            if (athlete == null)
            {
                // Self-healing: Create a profile if it's missing (e.g. registered before this feature)
                athlete = await _athleteService.CreateAthleteAsync(new CreateAthleteRequest
                {
                    FirstName = "New",
                    LastName = "Member",
                    Email = userEmail,
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    Weight = 75,
                    Height = 180
                });
                
                alerts.Add(_viewModelFactory.CreateAlert("A new athlete profile has been created for your account.", "info"));
            }
        }
        else
        {
            // Admin can see anyone, default to id 1 if not specified
            int targetId = id ?? 1;
            athlete = await _athleteService.GetAthleteByIdAsync(targetId);
        }

        int athleteId = athlete?.Id ?? (id ?? 1);
        
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
            // athlete is already fetched above
            statistics = await _fitnessService.GetAthleteStatisticsAsync(athleteId);

            var allActivities = await _fitnessService.GetAthleteActivitiesAsync(athleteId);
            allActivitiesForCharts = allActivities;
            totalActivities = allActivities.Count;
            totalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            page = Math.Max(1, Math.Min(page, totalPages));
            activities = allActivities.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            if (analysisType.HasValue)
            {
                analysis = await HandleAIAnalysisAsync(athleteId, analysisType.Value);
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

    private async Task<AIAnalysisDto?> HandleAIAnalysisAsync(int id, AnalysisType analysisType)
    {
        return analysisType switch
        {
            AnalysisType.PerformanceTrends => await _aiService.GetPerformanceTrendsAsync(id),
            AnalysisType.TrainingRecommendations => await _aiService.GetTrainingRecommendationsAsync(id),
            AnalysisType.HealthMetrics => await _aiService.GetHealthMetricsAsync(id),
            AnalysisType.NextDayRecommendation => await _aiService.GetNextDayRecommendationAsync(id),
            _ => null
        };
    }



    // POST: Dashboard/GetPerformanceTrends
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPerformanceTrends(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.PerformanceTrends });
    }

    // POST: Dashboard/GetPerformanceTrendsJson
    [HttpPost]
    public async Task<IActionResult> GetPerformanceTrendsJson(int id)
    {
        try
        {
            var analysis = await _aiService.GetPerformanceTrendsAsync(id);

            if (analysis == null)
            {
                return Json(new { success = false, error = "AI analysis failed" });
            }

            return Json(new { success = true, data = analysis });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // POST: Dashboard/GetTrainingRecommendations
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetTrainingRecommendations(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.TrainingRecommendations });
    }

    // POST: Dashboard/GetTrainingRecommendationsJson
    [HttpPost]
    public async Task<IActionResult> GetTrainingRecommendationsJson(int id)
    {
        try
        {
            var analysis = await _aiService.GetTrainingRecommendationsAsync(id);

            if (analysis == null)
            {
                return Json(new { success = false, error = "AI analysis failed" });
            }

            return Json(new { success = true, data = analysis });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetHealthMetrics(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.HealthMetrics });
    }

    // POST: Dashboard/GetHealthMetricsJson
    [HttpPost]
    public async Task<IActionResult> GetHealthMetricsJson(int id)
    {
        try
        {
            var analysis = await _aiService.GetHealthMetricsAsync(id);

            if (analysis == null)
            {
                return Json(new { success = false, error = "AI analysis failed" });
            }

            return Json(new { success = true, data = analysis });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }

    // POST: Dashboard/GetNextDayRecommendation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetNextDayRecommendation(int id)
    {
        return RedirectToAction(nameof(Index), new { id, analysisType = AnalysisType.NextDayRecommendation });
    }

    // POST: Dashboard/GetNextDayRecommendationJson
    [HttpPost]
    public async Task<IActionResult> GetNextDayRecommendationJson(int id)
    {
        try
        {
            var analysis = await _aiService.GetNextDayRecommendationAsync(id);

            if (analysis == null)
            {
                return Json(new { success = false, error = "AI analysis failed" });
            }

            return Json(new { success = true, data = analysis });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, error = ex.Message });
        }
    }



    private static WorkoutDataDto MapWorkout(ActivityDto activity)
    {
        return new WorkoutDataDto
        {
            Date = activity.StartDate,
            ActivityType = activity.ActivityType,
            Distance = activity.Distance,
            MovingTime = activity.MovingTime,
            HeartRate = activity.AverageHeartRate,
            Calories = activity.Calories
        };
    }
}
