namespace SportMatrix.AIAssistant.Application.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Request DTO for Performance Trends
/// </summary>
public class GrpcJsonPerformanceTrendsRequestDto
{
    [Required]
    public int AthleteId { get; set; }
    public string TimeFrame { get; set; } = "month";
}
