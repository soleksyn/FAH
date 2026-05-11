using SportMatrix.Frontend.Models;
using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Services;

public class DashboardViewModelFactory
{
    private static readonly string[] MonthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    public DashboardPageViewModel Create(
        AthleteDto? athlete,
        ActivityStatisticsDto? statistics,
        List<ActivityDto> activities,
        AIAnalysisDto? analysis,
        IEnumerable<DashboardAlertViewModel>? alerts = null,
        bool isDemoData = false,
        int currentPage = 1,
        int totalPages = 1,
        int totalActivities = 0,
        List<ActivityDto>? allActivities = null)
    {
        return new DashboardPageViewModel
        {
            Athlete = athlete == null ? null : MapAthlete(athlete),
            Alerts = alerts?.ToList() ?? [],
            StatCards = BuildStatCards(statistics),
            RecentActivities = activities.Select(MapActivity).ToList(),
            Charts = BuildCharts(statistics, allActivities ?? activities),
            AIAnalysis = analysis,
            IsDemoData = isDemoData,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            TotalActivities = totalActivities
        };
    }

    public DashboardAlertViewModel CreateAlert(string message, string type = "danger")
    {
        return new DashboardAlertViewModel
        {
            Message = message,
            Type = type,
            Icon = type == "warning" ? "bi-exclamation-circle-fill" : "bi-exclamation-triangle-fill"
        };
    }

    private static AthleteSummaryViewModel MapAthlete(AthleteDto athlete)
    {
        return new AthleteSummaryViewModel
        {
            Id = athlete.Id,
            FirstName = athlete.FirstName,
            LastName = athlete.LastName
        };
    }

    private static List<DashboardStatCardViewModel> BuildStatCards(ActivityStatisticsDto? statistics)
    {
        if (statistics == null)
        {
            return [];
        }

        return
        [
            new DashboardStatCardViewModel { Label = "Total Activities", Value = statistics.TotalActivities.ToString("N0"), Icon = "bi-activity", Accent = "orange" },
            new DashboardStatCardViewModel { Label = "Total Distance", Value = statistics.TotalDistance.ToString("F1"), Unit = "km", Icon = "bi-map", Accent = "mint" },
            new DashboardStatCardViewModel { Label = "Total Duration", Value = string.IsNullOrWhiteSpace(statistics.TotalDuration) ? "N/A" : statistics.TotalDuration, Icon = "bi-clock", Accent = "orange" },
            new DashboardStatCardViewModel { Label = "Est. Calories", Value = Math.Round(statistics.TotalDistance * 50).ToString("N0"), Icon = "bi-fire", Accent = "mint" }
        ];
    }

    private static DashboardActivityViewModel MapActivity(ActivityDto activity)
    {
        return new DashboardActivityViewModel
        {
            Id = activity.Id,
            Name = string.IsNullOrWhiteSpace(activity.Name) ? "Untitled activity" : activity.Name,
            SportType = activity.ActivityType,
            SportIcon = SportVisuals.GetIcon(activity.ActivityType),
            SportCssClass = SportVisuals.GetCssClass(activity.ActivityType),
            Distance = $"{activity.Distance:F1} km",
            MovingTime = string.IsNullOrWhiteSpace(activity.MovingTime) ? "N/A" : activity.MovingTime,
            StartDate = FormatDate(activity.StartDate),
            AverageHeartRate = activity.AverageHeartRate,
            Calories = activity.Calories
        };
    }

    private static DashboardChartDataViewModel BuildCharts(ActivityStatisticsDto? statistics, List<ActivityDto> allActivities)
    {
        return new DashboardChartDataViewModel
        {
            Weekly = BuildWeeklyChart(allActivities),
            Types = BuildActivityTypeChart(statistics),
            Monthly = BuildMonthlyChart(statistics)
        };
    }

    private static List<WeeklyChartPointViewModel> BuildWeeklyChart(List<ActivityDto> allActivities)
    {
        if (allActivities.Count == 0)
            return [];

        var today = DateTime.UtcNow.Date;
        var weeks = Enumerable.Range(0, 4)
            .Select(i =>
            {
                var weekEnd   = today.AddDays(-(i * 7));
                var weekStart = weekEnd.AddDays(-6);
                var distance  = allActivities
                    .Where(a => DateTime.TryParse(a.StartDate, out var d) && d.Date >= weekStart && d.Date <= weekEnd)
                    .Sum(a => a.Distance);
                return Math.Round(distance, 1);
            })
            .Reverse()
            .ToList();

        var maxDistance = weeks.Count > 0 ? weeks.Max() : 0;
        return weeks
            .Select(distance => new WeeklyChartPointViewModel
            {
                Distance = distance,
                Percentage = maxDistance > 0 ? (int)Math.Round(distance / maxDistance * 100) : 0
            })
            .ToList();
    }

    private static List<ActivityTypeChartPointViewModel> BuildActivityTypeChart(ActivityStatisticsDto? statistics)
    {
        if (statistics?.ActivitiesByType == null || statistics.ActivitiesByType.Count == 0)
            return [];

        var total = statistics.ActivitiesByType.Values.Sum();
        if (total == 0)
            return [];

        return statistics.ActivitiesByType
            .Where(kv => kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
            .Select(kv => new ActivityTypeChartPointViewModel
            {
                Name = kv.Key,
                Count = kv.Value,
                Percentage = (int)Math.Round((double)kv.Value / total * 100)
            })
            .ToList();
    }

    private static List<MonthlyChartPointViewModel> BuildMonthlyChart(ActivityStatisticsDto? statistics)
    {
        if (statistics?.ActivitiesByMonth == null || statistics.ActivitiesByMonth.Count == 0)
            return [];

        var monthCounts = new Dictionary<int, int>();
        foreach (var item in statistics.ActivitiesByMonth)
        {
            if (int.TryParse(item.Key, out var month) && month is >= 1 and <= 12)
            {
                monthCounts[month] = item.Value;
            }
        }

        if (monthCounts.Count == 0)
            return [];

        var maxCount = monthCounts.Values.Max();
        return MonthNames.Select((name, index) =>
        {
            var count = monthCounts.GetValueOrDefault(index + 1, 0);
            return new MonthlyChartPointViewModel
            {
                Name = name,
                Count = count,
                Percentage = maxCount > 0 ? (int)Math.Round((double)count / maxCount * 100) : 0
            };
        }).ToList();
    }

    private static string FormatDate(string value)
    {
        return DateTime.TryParse(value, out var parsed) ? parsed.ToString("yyyy-MM-dd") : "N/A";
    }
}

public static class SportVisuals
{
    public static string GetIcon(string? sportType)
    {
        return Normalize(sportType) switch
        {
            "run" => "bi-person-walking",
            "ride" => "bi-bicycle",
            "swim" => "bi-water",
            "walk" => "bi-person-walking",
            "hike" => "bi-tree",
            "workout" => "bi-lightning-charge",
            "weighttraining" => "bi-lightning-charge",
            "yoga" => "bi-peace",
            "crosstraining" => "bi-stars",
            _ => "bi-activity"
        };
    }

    public static string GetCssClass(string? sportType)
    {
        return Normalize(sportType) switch
        {
            "run" => "sport-run",
            "ride" => "sport-ride",
            "swim" => "sport-swim",
            "walk" => "sport-walk",
            "hike" => "sport-hike",
            "workout" => "sport-workout",
            "weighttraining" => "sport-workout",
            "yoga" => "sport-yoga",
            "crosstraining" => "sport-workout",
            _ => "sport-other"
        };
    }

    private static string Normalize(string? value)
    {
        return value?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
