namespace SportMatrix.AIAssistant.Application.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Request DTO for Health Metrics Analysis
/// </summary>
public class GrpcJsonHealthMetricsRequestDto
{
    [Required]
    public int AthleteId { get; set; }

    public GrpcJsonWorkoutDto[]? RecentWorkouts { get; set; }
}
