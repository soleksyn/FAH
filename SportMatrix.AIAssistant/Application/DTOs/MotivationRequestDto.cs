using System.ComponentModel.DataAnnotations;
using SportMatrix.AIAssistant.Application.DTOs;

namespace SportMatrix.AIAssistant.Application.DTOs;

public class MotivationRequestDto
{
    public AthleteProfileDto AthleteProfile { get; set; } = new();
    public WorkoutDataDto? LastWorkout { get; set; }
    public string? UpcomingWorkoutType { get; set; }

    [Required]
    public bool IsStruggling { get; set; }
}
