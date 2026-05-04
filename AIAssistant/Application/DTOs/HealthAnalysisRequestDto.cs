using System.ComponentModel.DataAnnotations;
using FitnessAnalyticsHub.AIAssistant.Application.DTOs;

namespace AIAssistant.Application.DTOs;

public class HealthAnalysisRequestDto
{
    [Required]
    public int AthleteId { get; set; }
    public List<WorkoutDataDto> RecentWorkouts { get; set; } = new();

    /// <summary>
    /// Zusätzliche Gesundheitsmetriken (optional)
    /// </summary>
    public Dictionary<string, object>? HealthMetrics { get; set; }

    /// <summary>
    /// Specific areas for analysis (e.g. "injury_prevention", "recovery", "overtraining")
    /// </summary>
    public List<string>? FocusAreas { get; set; }

    /// <summary>
    /// Known injuries or health limitations
    /// </summary>
    public List<string>? KnownIssues { get; set; }
}