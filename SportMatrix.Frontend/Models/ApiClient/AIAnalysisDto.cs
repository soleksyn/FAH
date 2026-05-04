using System.Text.Json.Serialization;

namespace SportMatrix.Frontend.Models.ApiClient;

public class AIAnalysisDto
{
    [JsonPropertyName("analysis")]
    public string? Analysis { get; set; }

    [JsonPropertyName("keyInsights")]
    public List<string>? KeyInsights { get; set; }

    [JsonPropertyName("recommendations")]
    public List<string>? Recommendations { get; set; }

    [JsonPropertyName("performanceScore")]
    public int? PerformanceScore { get; set; }

    [JsonPropertyName("trends")]
    public TrendDto? Trends { get; set; }
}

public class TrendDto
{
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}
