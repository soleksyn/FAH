namespace SportMatrix.AIAssistant.Application.Interfaces;

using SportMatrix.AIAssistant.Application.DTOs;

public interface IWorkoutAnalysisService
{
    Task<WorkoutAnalysisResponseDto> AnalyzeWorkoutsAsync(
        WorkoutAnalysisRequestDto request, CancellationToken cancellationToken);
}
