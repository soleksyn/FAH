using SportMatrix.AIAssistant.Domain.Models;

namespace SportMatrix.AIAssistant.Domain.Interfaces;

public interface IWorkoutAnalysisDomainService
{
    string FormatWorkoutDataForAnalysis(IEnumerable<WorkoutData> workouts);

    string FormatAthleteProfileForAnalysis(AthleteProfile profile);
}
