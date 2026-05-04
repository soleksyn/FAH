using System.Text.Json.Serialization;

namespace FitnessAnalyticsHub.Frontend.Models.ApiClient;

public class ActivityDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("sportType")]
    public string SportType { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("athleteFullName")]
    public string AthleteFullName { get; set; } = string.Empty;

    [JsonPropertyName("movingTime")]
    public double? MovingTime { get; set; }

    [JsonPropertyName("averageHeartRate")]
    public int? AverageHeartRate { get; set; }

    [JsonPropertyName("maxHeartRate")]
    public int? MaxHeartRate { get; set; }

    [JsonPropertyName("calories")]
    public int? Calories { get; set; }
}
