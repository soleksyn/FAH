namespace SportMatrix.AIAssistant.Infrastructure.Services;

using System.Text.Json;
using Google.GenAI.Types;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.Domain.Enums;

public class WorkoutAnalysisService : IWorkoutAnalysisService
{
    // Typed schema — Gemini constrains the response to exactly these three fields.
    private static readonly Schema ResponseSchema = new Schema
    {
        Type = Type.Object,
        Properties = new Dictionary<string, Schema>
        {
            ["analysis"] = new Schema
            {
                Type = Type.String,
                Description = "2-4 sentence summary of the training data.",
            },
            ["keyInsights"] = new Schema
            {
                Type = Type.Array,
                Items = new Schema { Type = Type.String },
                Description = "3-5 concise, actionable insight bullets.",
            },
            ["recommendations"] = new Schema
            {
                Type = Type.Array,
                Items = new Schema { Type = Type.String },
                Description = "2-4 specific, prioritized training recommendations.",
            },
        },
        Required = new List<string> { "analysis", "keyInsights", "recommendations" },
    };

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IAIPromptService aiPromptService;
    private readonly ILogger<WorkoutAnalysisService> logger;

    public WorkoutAnalysisService(
        IAIPromptService aiPromptService,
        ILogger<WorkoutAnalysisService> logger)
    {
        this.aiPromptService = aiPromptService;
        this.logger = logger;
    }

    public async Task<WorkoutAnalysisResponseDto> AnalyzeWorkoutsAsync(
        WorkoutAnalysisRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {

            this.logger.LogInformation(
                "Analyzing {WorkoutCount} workouts, type: {AnalysisType}",
                request.RecentWorkouts?.Count ?? 0, request.AnalysisType);

            string systemInstruction = this.GetSystemInstruction(request.AnalysisType);
            string prompt = this.BuildPrompt(request);

            string json = await this.aiPromptService.GetStructuredAnalysisAsync(
                prompt, systemInstruction, ResponseSchema, cancellationToken);

            WorkoutAnalysisResponseDto result = this.DeserializeResponse(json);
            result.Provider = "Gemini-AI";

            this.logger.LogInformation(
                "Analysis complete: {InsightCount} insights, {RecommendationCount} recommendations",
                result.KeyInsights?.Count ?? 0, result.Recommendations?.Count ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error analyzing workouts");
            return this.GetFallbackAnalysis(request.AnalysisType);
        }
    }

    private string GetSystemInstruction(AnalysisType analysisType)
    {
        return analysisType switch
        {
            AnalysisType.Health => "You are a sports medicine and recovery specialist. Analyze the athlete's training load for health risks, recovery status, and injury prevention. Be evidence-based and concise.",
            AnalysisType.Performance => "You are an elite performance coach. Analyze the athlete's training data for performance trends, strengths, weaknesses, and opportunities to improve speed, power, or endurance.",
            AnalysisType.Trends => "You are a fitness data analyst. Identify macro-level training patterns, consistency trends, and long-term trajectory from the provided workout history.",
            AnalysisType.NextDay => "You are a recovery planning expert. Based on the athlete's recent workload, prescribe the optimal next-day activity and a specific recovery protocol.",
            _ => "You are a comprehensive fitness coach. Analyze the training data and provide actionable, data-driven insights."
        };
    }

    private string BuildPrompt(WorkoutAnalysisRequestDto request)
    {
        if (request.RecentWorkouts == null || request.RecentWorkouts.Count == 0)
        {
            return "No workout data was provided. Return a message in the 'analysis' field asking the user to log some activities first.";
        }

        string workoutLines = string.Join("\n", request.RecentWorkouts.Select(w =>
            $"- {w.Date:yyyy-MM-dd} | {w.ActivityType} | {w.Distance / 1000.0:F1} km | {TimeSpan.FromSeconds(w.Duration):hh\\:mm\\:ss} | {w.Calories} kcal"));

        string athleteContext = request.AthleteProfile is not null
            ? $"\nAthlete profile: Fitness level = {request.AthleteProfile.FitnessLevel}, Goal = {request.AthleteProfile.PrimaryGoal}."
            : string.Empty;

        return $"Analyze the following recent workouts and return a structured JSON response.{athleteContext}\n\nWorkouts:\n{workoutLines}";
    }

    private WorkoutAnalysisResponseDto DeserializeResponse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return this.GetFallbackAnalysis(AnalysisType.Performance);
        }

        try
        {
            var dto = JsonSerializer.Deserialize<WorkoutAnalysisResponseDto>(json, JsonOptions);
            if (dto is not null && !string.IsNullOrWhiteSpace(dto.Analysis))
            {
                dto.GeneratedAt = DateTime.UtcNow;
                return dto;
            }

            this.logger.LogWarning("Gemini returned valid JSON but an empty analysis field");
            return this.GetFallbackAnalysis(AnalysisType.Performance);
        }
        catch (JsonException ex)
        {
            this.logger.LogError(ex, "Failed to deserialize Gemini JSON response: {Json}", json);
            return this.GetFallbackAnalysis(AnalysisType.Performance);
        }
    }

    private WorkoutAnalysisResponseDto GetFallbackAnalysis(AnalysisType analysisType)
    {
        this.logger.LogWarning("Using fallback analysis for type: {AnalysisType}", analysisType);

        // TODO: DEMO DATA - This is hardcoded fallback logic, not using AI.
        return new WorkoutAnalysisResponseDto
        {
            Analysis = "AI analysis is temporarily unavailable. Please try again in a moment.",
            KeyInsights = new List<string>(),
            Recommendations = new List<string>(),
            GeneratedAt = DateTime.UtcNow,
        };
    }
}
