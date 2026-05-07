namespace SportMatrix.AIAssistant.Application.Interfaces;

using SportMatrix.AIAssistant.Application.DTOs;

public interface IWorkoutDataProvider
{
    Task<List<WorkoutDataDto>> GetRecentWorkoutsAsync(int athleteId, TimeSpan lookback, CancellationToken cancellationToken);

    Task<List<WorkoutDataDto>> GetLatestWorkoutsAsync(int athleteId, int count, CancellationToken cancellationToken);

    AthleteProfileDto BuildAthleteProfile(int athleteId, IReadOnlyCollection<WorkoutDataDto> workouts);
}
