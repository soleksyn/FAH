using System.Text.Json.Serialization;

namespace SportMatrix.Frontend.Models.ApiClient;

public class ActivityStatisticsDto
{
    [JsonPropertyName("totalActivities")]
    public int TotalActivities { get; set; }

    [JsonPropertyName("totalDistance")]
    public double TotalDistance { get; set; }

    [JsonPropertyName("totalDuration")]
    public string TotalDuration { get; set; } = string.Empty;

    [JsonPropertyName("activitiesByType")]
    public Dictionary<string, int> ActivitiesByType { get; set; } = new();

    [JsonPropertyName("activitiesByMonth")]
    public Dictionary<string, int> ActivitiesByMonth { get; set; } = new();

    [JsonPropertyName("averageDistance")]
    public double? AverageDistance { get; set; }

    [JsonPropertyName("longestDistance")]
    public double? LongestDistance { get; set; }

    [JsonPropertyName("mostCommonActivityType")]
    public string? MostCommonActivityType { get; set; }
}
