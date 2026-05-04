using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Models;

public class FitnessDashboardViewModel
{
    public AthleteDto? Athlete { get; set; }
    public ActivityStatisticsDto? Statistics { get; set; }
    public List<ActivityDto> RecentActivities { get; set; } = new();
    public AIAnalysisDto? AIAnalysis { get; set; }
    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
    public int? AnalyzingActivityId { get; set; }

    public string FullName => Athlete != null ? $"{Athlete.FirstName} {Athlete.LastName}" : "User";

    public string GetActivityEmoji(string sportType)
    {
        return sportType switch
        {
            "Run" => "🏃",
            "Ride" => "🚴",
            "Swim" => "🏊",
            "Walk" => "🚶",
            "Hike" => "🥾",
            "Workout" => "💪",
            "Yoga" => "🧘",
            "WeightTraining" => "🏋️",
            "Crosstraining" => "⚡",
            _ => "🏃"
        };
    }

    public string GetActivityIconClass(string sportType)
    {
        return sportType switch
        {
            "Run" => "run",
            "Ride" => "ride",
            "Swim" => "swim",
            "Walk" => "walk",
            "Hike" => "hike",
            "Workout" => "workout",
            "Yoga" => "yoga",
            "WeightTraining" => "workout",
            "Crosstraining" => "workout",
            _ => "default"
        };
    }

    public string GetActivityColor(string activityType)
    {
        return activityType switch
        {
            "Run" => "linear-gradient(135deg, #28a745, #20c997)",
            "Ride" => "linear-gradient(135deg, #007bff, #6610f2)",
            "Swim" => "linear-gradient(135deg, #17a2b8, #6f42c1)",
            "Walk" => "linear-gradient(135deg, #6c757d, #495057)",
            "Hike" => "linear-gradient(135deg, #795548, #8d6e63)",
            "Workout" => "linear-gradient(135deg, #dc3545, #e83e8c)",
            "Yoga" => "linear-gradient(135deg, #9c27b0, #673ab7)",
            "WeightTraining" => "linear-gradient(135deg, #dc3545, #e83e8c)",
            "Crosstraining" => "linear-gradient(135deg, #fd7e14, #ffc107)",
            _ => "linear-gradient(135deg, #667eea, #764ba2)"
        };
    }

    public string GetScoreClass(int? score)
    {
        if (score == null) return "poor";
        if (score >= 85) return "excellent";
        if (score >= 70) return "good";
        if (score >= 50) return "fair";
        return "poor";
    }

    public string FormatMovingTime(double? movingTime)
    {
        if (movingTime == null) return "N/A";
        var seconds = (int)movingTime.Value;
        if (seconds == 0 || double.IsNaN(movingTime.Value)) return "N/A";

        var hours = seconds / 3600;
        var minutes = (seconds % 3600) / 60;
        var remainingSeconds = seconds % 60;

        if (hours > 0)
            return $"{hours}h {minutes}m {remainingSeconds}s";
        if (minutes > 0)
            return $"{minutes}m {remainingSeconds}s";
        return $"{remainingSeconds}s";
    }

    public string GetEstimatedCalories()
    {
        if (Statistics == null) return "0";
        var estimated = Math.Round(Statistics.TotalDistance * 50);
        return estimated.ToString("N0");
    }

    public List<ActivityTypeInfo> GetActivityTypes()
    {
        if (Statistics == null || Statistics.ActivitiesByType.Count == 0)
        {
            return new List<ActivityTypeInfo>
            {
                new() { Name = "Run", Count = 28, Percentage = 60 },
                new() { Name = "Ride", Count = 15, Percentage = 32 },
                new() { Name = "Swim", Count = 4, Percentage = 8 }
            };
        }

        var total = Statistics.ActivitiesByType.Values.Sum();
        return Statistics.ActivitiesByType
            .Select(kv => new ActivityTypeInfo
            {
                Name = kv.Key,
                Count = kv.Value,
                Percentage = total > 0 ? (int)Math.Round((double)kv.Value / total * 100) : 0
            })
            .OrderByDescending(a => a.Count)
            .ToList();
    }

    public List<WeeklyProgressInfo> GetWeeklyProgress()
    {
        var weeklyData = new List<WeeklyProgressInfo>
        {
            new() { Week = 1, Distance = 15.2 },
            new() { Week = 2, Distance = 23.8 },
            new() { Week = 3, Distance = 31.5 },
            new() { Week = 4, Distance = 28.7 }
        };

        var maxDistance = weeklyData.Max(w => w.Distance);
        foreach (var week in weeklyData)
        {
            week.Percentage = maxDistance > 0 ? (int)(week.Distance / maxDistance * 100) : 0;
        }

        return weeklyData;
    }

    public string GetAverageWeeklyDistance()
    {
        var weeks = GetWeeklyProgress();
        var average = weeks.Average(w => w.Distance);
        return average.ToString("F1");
    }

    public string GetWeeklyTrend()
    {
        var weeks = GetWeeklyProgress();
        var lastWeek = weeks.Last().Distance;
        var previousWeek = weeks[^2].Distance;

        if (lastWeek > previousWeek) return "📈 Improving";
        if (lastWeek < previousWeek) return "📉 Declining";
        return "➡️ Stable";
    }

    public string GetWeekColor(int index)
    {
        var colors = new[]
        {
            "linear-gradient(90deg, #667eea, #764ba2)",
            "linear-gradient(90deg, #f093fb, #f5576c)",
            "linear-gradient(90deg, #4facfe, #00f2fe)",
            "linear-gradient(90deg, #43e97b, #38f9d7)"
        };
        return colors[index % colors.Length];
    }

    public List<MonthlyDataInfo> GetMonthlyData()
    {
        var monthNames = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        var defaultData = new Dictionary<int, int>
        {
            [1] = 8, [2] = 12, [3] = 15, [4] = 12, [5] = 9, [6] = 5
        };

        Dictionary<int, int> data;
        if (Statistics != null && Statistics.ActivitiesByMonth.Count > 0)
        {
            data = new Dictionary<int, int>();
            foreach (var kv in Statistics.ActivitiesByMonth)
            {
                if (int.TryParse(kv.Key, out var month))
                {
                    data[month] = kv.Value;
                }
            }
        }
        else
        {
            data = defaultData;
        }

        var maxCount = data.Count > 0 ? data.Values.Max() : 1;
        return monthNames.Select((name, index) =>
        {
            var monthIndex = index + 1;
            var count = data.GetValueOrDefault(monthIndex, 0);
            return new MonthlyDataInfo
            {
                Name = name,
                Count = count,
                Percentage = maxCount > 0 ? (int)((double)count / maxCount * 100) : 0
            };
        }).ToList();
    }

    public string GetMonthHeatColor(int count)
    {
        if (count == 0) return "#eee";
        if (count <= 3) return "#c6e48b";
        if (count <= 6) return "#7bc96f";
        if (count <= 10) return "#239a3b";
        return "#196127";
    }

    public string FormatMarkdown(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        return text
            .Replace("## ", "<h4 class=\"analysis-header\">", StringComparison.Ordinal)
            .Replace("**", "<strong class=\"analysis-bold\">", StringComparison.Ordinal)
            .Replace("\n\n", "</p><p class=\"analysis-paragraph\">");
    }

    public string GetMotivationText()
    {
        if (AIAnalysis?.PerformanceScore > 70)
            return $"\"Fantastic! Your performance score is {AIAnalysis.PerformanceScore}/100. You're crushing your goals!\"";
        if (AIAnalysis?.PerformanceScore <= 70)
            return "\"Keep going! Every workout counts. Your dedication will pay off!\"";
        return "\"Ready for today's challenge? Let's analyze your latest activities and unlock new insights!\"";
    }
}

public class ActivityTypeInfo
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}

public class WeeklyProgressInfo
{
    public int Week { get; set; }
    public double Distance { get; set; }
    public int Percentage { get; set; }
}

public class MonthlyDataInfo
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
    public int Percentage { get; set; }
}
