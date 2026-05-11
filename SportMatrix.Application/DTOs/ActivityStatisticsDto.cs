using SportMatrix.Domain.Enums;

namespace SportMatrix.Application.DTOs;

public class ActivityStatisticsDto
{
    public int TotalActivities { get; set; }
    public double TotalDistance { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public double TotalElevationGain { get; set; }
    public Dictionary<ActivityType, int> ActivitiesByType { get; set; } = new Dictionary<ActivityType, int>();
    public Dictionary<int, int> ActivitiesByMonth { get; set; } = new Dictionary<int, int>();
    public double? AverageDistance { get; set; }
    public double? LongestDistance { get; set; }
    public ActivityType? MostCommonActivityType { get; set; }
}
