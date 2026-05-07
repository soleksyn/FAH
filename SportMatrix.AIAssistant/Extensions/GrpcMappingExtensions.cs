using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Domain.Models;
using SportMatrix.Domain.Enums;

namespace SportMatrix.AIAssistant.Extensions;

public static class GrpcMappingExtensions
{
    public static AIAssistant.Domain.Models.AthleteProfile ToAIAssistantAthleteProfile(
        this global::Sportmatrix.AthleteProfile grpcProfile)
    {
        return new AIAssistant.Domain.Models.AthleteProfile
        {
            Id = Guid.NewGuid().ToString(),
            Name = grpcProfile?.Name ?? string.Empty,
            FitnessLevel = grpcProfile?.FitnessLevel ?? string.Empty,
            PrimaryGoal = grpcProfile?.PrimaryGoal ?? string.Empty,
            Preferences = new Dictionary<string, object>(),
        };
    }


    public static WorkoutAnalysisRequestDto ToWorkoutAnalysisRequestDto(
    this global::Sportmatrix.WorkoutAnalysisRequest grpcRequest)
    {
        return new WorkoutAnalysisRequestDto
        {
            AnalysisType = Enum.TryParse<AnalysisType>(grpcRequest.AnalysisType, ignoreCase: true, out var type) ? type : AnalysisType.Performance,
            RecentWorkouts = grpcRequest.RecentWorkouts
                .Select(w => w.ToWorkoutDataDto())
                .ToList(),
            AthleteProfile = grpcRequest.AthleteProfile?.ToAthleteProfileDto() ?? new AthleteProfileDto(),
            AdditionalContext = new Dictionary<string, object>(),
        };
    }

    public static WorkoutData ToAIAssistantWorkoutData(
    this global::Sportmatrix.Workout grpcWorkout)
    {
        return new WorkoutData
        {
            Date = DateTime.Parse(grpcWorkout.Date),
            ActivityType = grpcWorkout.ActivityType,
            Distance = grpcWorkout.Distance,
            Duration = (int)grpcWorkout.Duration,
            Calories = (int)grpcWorkout.Calories,
        };
    }
}
