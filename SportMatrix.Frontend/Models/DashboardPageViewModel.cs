using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Models;

public class DashboardPageViewModel
{
    public AthleteSummaryViewModel? Athlete { get; set; }
    public List<DashboardAlertViewModel> Alerts { get; set; } = [];
    public List<DashboardStatCardViewModel> StatCards { get; set; } = [];
    public List<DashboardActivityViewModel> RecentActivities { get; set; } = [];
    public DashboardChartDataViewModel Charts { get; set; } = new();
    public AIAnalysisDto? AIAnalysis { get; set; }
    public bool IsDemoData { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalActivities { get; set; } = 0;
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;

    public bool HasCharts => Charts.Weekly.Count > 0 || Charts.Types.Count > 0 || Charts.Monthly.Count > 0;
}

public class AthleteSummaryViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => string.Join(" ", new[] { FirstName, LastName }.Where(value => !string.IsNullOrWhiteSpace(value)));
}

public class DashboardAlertViewModel
{
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "danger";
    public string Icon { get; set; } = "bi-exclamation-triangle-fill";
}

public class DashboardStatCardViewModel
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Accent { get; set; } = "orange";
}

public class DashboardActivityViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SportType { get; set; } = string.Empty;
    public string SportIcon { get; set; } = "bi-activity";
    public string SportCssClass { get; set; } = "sport-other";
    public string Distance { get; set; } = string.Empty;
    public string MovingTime { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
}

public class DashboardChartDataViewModel
{
    public List<WeeklyChartPointViewModel> Weekly { get; set; } = [];
    public List<ActivityTypeChartPointViewModel> Types { get; set; } = [];
    public List<MonthlyChartPointViewModel> Monthly { get; set; } = [];
}

public class WeeklyChartPointViewModel
{
    public double Distance { get; set; }
    public int Percentage { get; set; }
}

public class ActivityTypeChartPointViewModel
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}

public class MonthlyChartPointViewModel
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}
