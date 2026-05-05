using System.ComponentModel.DataAnnotations;
using SportMatrix.AIAssistant.Application.DTOs;

namespace SportMatrix.AIAssistant.Application.DTOs;

public class HealthAnalysisRequestDto
{
    [Required]
    public int AthleteId { get; set; }
    public List<WorkoutDataDto> RecentWorkouts { get; set; } = new();

    /// <summary>
    /// Additional health metrics (optional)
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
