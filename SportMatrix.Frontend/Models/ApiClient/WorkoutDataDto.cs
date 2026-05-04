using System.Text.Json.Serialization;

namespace SportMatrix.Frontend.Models.ApiClient;

public class WorkoutDataDto
{
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    [JsonPropertyName("activityType")]
    public string ActivityType { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    [JsonPropertyName("movingTime")]
    public string MovingTime { get; set; } = string.Empty;

    [JsonPropertyName("heartRate")]
    public int? HeartRate { get; set; }

    [JsonPropertyName("calories")]
    public int? Calories { get; set; }
}
