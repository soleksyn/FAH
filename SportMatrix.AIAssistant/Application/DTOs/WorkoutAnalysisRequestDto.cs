namespace SportMatrix.AIAssistant.Application.DTOs;

using SportMatrix.Domain.Enums;

public class WorkoutAnalysisRequestDto
{
    public List<WorkoutDataDto> RecentWorkouts { get; set; } = new();
    public AnalysisType AnalysisType { get; set; } = AnalysisType.Performance;
    public AthleteProfileDto? AthleteProfile { get; set; }
    public Dictionary<string, object>? AdditionalContext { get; set; }
}
