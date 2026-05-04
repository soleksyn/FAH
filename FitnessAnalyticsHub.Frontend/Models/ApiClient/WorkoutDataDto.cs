using System.Text.Json.Serialization;

namespace FitnessAnalyticsHub.Frontend.Models.ApiClient;

public class WorkoutDataDto
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("activityType")]
    public string ActivityType { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("movingTime")]
    public double? MovingTime { get; set; }

    [JsonPropertyName("heartRate")]
    public int? HeartRate { get; set; }

    [JsonPropertyName("calories")]
    public int? Calories { get; set; }
}
