namespace SportMatrix.Infrastructure.Services;

using SportMatrix;
using SportMatrix.Application.DTOs;
using SportMatrix.Application.Interfaces;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sportmatrix;

public class GrpcAIAssistantClientService : IAIAssistantClientService, IDisposable
{
    private readonly GrpcChannel channel;
    private readonly ILogger<GrpcAIAssistantClientService> logger;
    private readonly Sportmatrix.MotivationService.MotivationServiceClient motivationClient;
    private readonly Sportmatrix.WorkoutService.WorkoutServiceClient workoutServiceClient;

    public GrpcAIAssistantClientService(
        ILogger<GrpcAIAssistantClientService> logger,
        IConfiguration configuration)
    {
        this.logger = logger;

        // gRPC Channel erstellen (wie HttpClient, aber f�r gRPC)
        string grpcUrl = configuration["AIAssistant:GrpcUrl"] ?? "http://localhost:5001";
        this.channel = GrpcChannel.ForAddress(grpcUrl);

        // Client erstellen!
        this.motivationClient = new Sportmatrix.MotivationService.MotivationServiceClient(this.channel);
        this.workoutServiceClient = new Sportmatrix.WorkoutService.WorkoutServiceClient(this.channel);

        this.logger.LogInformation("gRPC Channel created for: {GrpcUrl}", grpcUrl);
    }

    public async Task<AIMotivationResponseDto> GetMotivationAsync(AIMotivationRequestDto request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "gRPC: Requesting motivation for athlete: {AthleteName}",
            request.AthleteProfile?.Name ?? "Unknown");

        try
        {
            MotivationRequest grpcRequest = new MotivationRequest
            {
                AthleteProfile = new AthleteProfile
                {
                    Name = request.AthleteProfile?.Name ?? string.Empty,
                    FitnessLevel = request.AthleteProfile?.FitnessLevel ?? string.Empty,
                    PrimaryGoal = request.AthleteProfile?.PrimaryGoal ?? string.Empty,
                },
                PreferredTone = request.PreferredTone ?? string.Empty,
                ContextualInfo = request.ContextualInfo ?? string.Empty,
            };

            // gRPC-Call
            MotivationResponse grpcResponse = await this.motivationClient.GetMotivationAsync(grpcRequest, cancellationToken: cancellationToken);

            // gRPC-Response zu DTO konvertieren
            AIMotivationResponseDto response = new AIMotivationResponseDto
            {
                MotivationalMessage = grpcResponse.MotivationalMessage,
                Quote = grpcResponse.Quote,
                ActionableTips = grpcResponse.ActionableTips.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,
            };

            this.logger.LogInformation("gRPC: Motivation response received successfully");
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting motivation");
            throw;
        }
    }

    public async Task<AIWorkoutAnalysisResponseDto> GetWorkoutAnalysisAsync(AIWorkoutAnalysisRequestDto request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "gRPC: Requesting workout analysis for {WorkoutCount} workouts",
            request.RecentWorkouts?.Count ?? 0);

        try
        {
            // Konvertiere DTO zu gRPC Request
            WorkoutAnalysisRequest grpcRequest = new WorkoutAnalysisRequest
            {
                AnalysisType = request.AnalysisType ?? "General",

                // PreferredAiProvider = request.PreferredAiProvider ?? "huggingface"
                PreferredAiProvider = "googlegemini",
            };

            // AthleteProfile hinzuf�gen
            if (request.AthleteProfile != null)
            {
                grpcRequest.AthleteProfile = new AthleteProfile
                {
                    Name = request.AthleteProfile.Name ?? string.Empty,
                    FitnessLevel = request.AthleteProfile.FitnessLevel ?? string.Empty,
                    PrimaryGoal = request.AthleteProfile.PrimaryGoal ?? string.Empty,
                };
            }

            // Workouts hinzuf�gen
            if (request.RecentWorkouts != null)
            {
                foreach (AIWorkoutDataDto workout in request.RecentWorkouts)
                {
                    Workout grpcWorkout = new Workout
                    {
                        Date = workout.Date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ActivityType = workout.ActivityType ?? string.Empty,
                        Distance = workout.Distance,
                        Duration = workout.Duration,
                        Calories = workout.Calories,
                    };

                    // MetricsData hinzuf�gen
                    // if (workout.MetricsData != null)
                    // {
                    //    foreach (var metric in workout.MetricsData)
                    //    {
                    //        grpcWorkout.MetricsData.Add(metric.Key, metric.Value);
                    //    }
                    // }
                    grpcRequest.RecentWorkouts.Add(grpcWorkout);
                }
            }

            // gRPC-Call durchf�hren!
            WorkoutAnalysisResponse grpcResponse = await this.workoutServiceClient.GetWorkoutAnalysisAsync(grpcRequest, cancellationToken: cancellationToken);

            // gRPC-Response zu DTO konvertieren
            AIWorkoutAnalysisResponseDto response = new AIWorkoutAnalysisResponseDto
            {
                Analysis = grpcResponse.Analysis,
                KeyInsights = grpcResponse.KeyInsights.ToList(),
                Recommendations = grpcResponse.Recommendations.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                    ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,

                // AnalysisType = grpcResponse.AnalysisType
            };

            this.logger.LogInformation("gRPC: Workout analysis response received successfully from {Source}", response.Source);
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting workout analysis");
            throw;
        }
    }

    public async Task<AIWorkoutAnalysisResponseDto> GetPerformanceTrendsAsync(int athleteId, CancellationToken cancellationToken, string timeFrame = "month")
    {
        this.logger.LogInformation(
            "gRPC: Requesting performance trends for athlete: {AthleteId}, timeFrame: {TimeFrame}",
            athleteId, timeFrame);

        try
        {
            PerformanceTrendsRequest grpcRequest = new PerformanceTrendsRequest
            {
                AthleteId = athleteId,
                TimeFrame = timeFrame,
            };

            WorkoutAnalysisResponse grpcResponse = await this.workoutServiceClient.GetPerformanceTrendsAsync(grpcRequest, cancellationToken: cancellationToken);

            AIWorkoutAnalysisResponseDto response = new AIWorkoutAnalysisResponseDto
            {
                Analysis = grpcResponse.Analysis,
                KeyInsights = grpcResponse.KeyInsights.ToList(),
                Recommendations = grpcResponse.Recommendations.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                    ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,

                // AnalysisType = grpcResponse.AnalysisType
            };

            this.logger.LogInformation("gRPC: Performance trends response received successfully");
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting performance trends");
            throw;
        }
    }

    public async Task<AIWorkoutAnalysisResponseDto> GetTrainingRecommendationsAsync(int athleteId, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("gRPC: Requesting training recommendations for athlete: {AthleteId}", athleteId);

        try
        {
            TrainingRecommendationsRequest grpcRequest = new TrainingRecommendationsRequest
            {
                AthleteId = athleteId,
            };

            WorkoutAnalysisResponse grpcResponse = await this.workoutServiceClient.GetTrainingRecommendationsAsync(grpcRequest, cancellationToken: cancellationToken);

            AIWorkoutAnalysisResponseDto response = new AIWorkoutAnalysisResponseDto
            {
                Analysis = grpcResponse.Analysis,
                KeyInsights = grpcResponse.KeyInsights.ToList(),
                Recommendations = grpcResponse.Recommendations.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                    ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,

                // AnalysisType = grpcResponse.AnalysisType
            };

            this.logger.LogInformation("gRPC: Training recommendations response received successfully");
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting training recommendations");
            throw;
        }
    }

    public async Task<AIWorkoutAnalysisResponseDto> AnalyzeHealthMetricsAsync(int athleteId, List<AIWorkoutDataDto> recentWorkouts, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("gRPC: Requesting health metrics analysis for athlete: {AthleteId}", athleteId);

        try
        {
            HealthAnalysisRequest grpcRequest = new HealthAnalysisRequest
            {
                AthleteId = athleteId,
            };

            // Workouts hinzuf�gen
            if (recentWorkouts != null)
            {
                foreach (AIWorkoutDataDto workout in recentWorkouts)
                {
                    Workout grpcWorkout = new Workout
                    {
                        Date = workout.Date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ActivityType = workout.ActivityType ?? string.Empty,
                        Distance = workout.Distance,
                        Duration = workout.Duration,
                        Calories = workout.Calories,
                    };

                    // if (workout.MetricsData != null)
                    // {
                    //    foreach (var metric in workout.MetricsData)
                    //    {
                    //        grpcWorkout.MetricsData.Add(metric.Key, metric.Value);
                    //    }
                    // }
                    grpcRequest.RecentWorkouts.Add(grpcWorkout);
                }
            }

            WorkoutAnalysisResponse grpcResponse = await this.workoutServiceClient.AnalyzeHealthMetricsAsync(grpcRequest, cancellationToken: cancellationToken);

            AIWorkoutAnalysisResponseDto response = new AIWorkoutAnalysisResponseDto
            {
                Analysis = grpcResponse.Analysis,
                KeyInsights = grpcResponse.KeyInsights.ToList(),
                Recommendations = grpcResponse.Recommendations.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                    ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,

                // AnalysisType = grpcResponse.AnalysisType
            };

            this.logger.LogInformation("gRPC: Health metrics analysis response received successfully");
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error analyzing health metrics");
            throw;
        }
    }

    // GoogleGemini Workout Analysis
    public async Task<AIWorkoutAnalysisResponseDto> GetGoogleGeminiWorkoutAnalysisAsync(AIWorkoutAnalysisRequestDto request, CancellationToken cancellationToken)
    {
        this.logger.LogInformation(
            "gRPC: Requesting GoogleGemini workout analysis for {WorkoutCount} workouts",
            request.RecentWorkouts?.Count ?? 0);

        try
        {
            // Konvertiere DTO zu gRPC Request
            WorkoutAnalysisRequest grpcRequest = new WorkoutAnalysisRequest
            {
                AnalysisType = request.AnalysisType ?? "General",
                PreferredAiProvider = "googlegemini", // Zwinge GoogleGemini
            };

            // AthleteProfile hinzuf�gen
            if (request.AthleteProfile != null)
            {
                grpcRequest.AthleteProfile = new AthleteProfile
                {
                    Name = request.AthleteProfile.Name ?? string.Empty,
                    FitnessLevel = request.AthleteProfile.FitnessLevel ?? string.Empty,
                    PrimaryGoal = request.AthleteProfile.PrimaryGoal ?? string.Empty,
                };
            }

            // Workouts hinzuf�gen
            if (request.RecentWorkouts != null)
            {
                foreach (AIWorkoutDataDto workout in request.RecentWorkouts)
                {
                    Workout grpcWorkout = new Workout
                    {
                        Date = workout.Date.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        ActivityType = workout.ActivityType ?? string.Empty,
                        Distance = workout.Distance,
                        Duration = workout.Duration,
                        Calories = workout.Calories,
                    };

                    // if (workout.MetricsData != null)
                    // {
                    //    foreach (var metric in workout.MetricsData)
                    //    {
                    //        grpcWorkout.MetricsData.Add(metric.Key, metric.Value);
                    //    }
                    // }
                    grpcRequest.RecentWorkouts.Add(grpcWorkout);
                }
            }

            // Verwende die GoogleGemini-spezifische Methode
            WorkoutAnalysisResponse grpcResponse = await this.workoutServiceClient.AnalyzeGoogleGeminiWorkoutsAsync(grpcRequest, cancellationToken: cancellationToken);

            AIWorkoutAnalysisResponseDto response = new AIWorkoutAnalysisResponseDto
            {
                Analysis = grpcResponse.Analysis,
                KeyInsights = grpcResponse.KeyInsights.ToList(),
                Recommendations = grpcResponse.Recommendations.ToList(),
                GeneratedAt = DateTime.TryParse(grpcResponse.GeneratedAt, out DateTime parsedDate)
                    ? parsedDate : DateTime.UtcNow,
                Source = grpcResponse.Source,

                // AnalysisType = grpcResponse.AnalysisType
            };

            this.logger.LogInformation("gRPC: GoogleGemini workout analysis response received successfully");
            return response;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting GoogleGemini workout analysis");
            throw;
        }
    }

    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken)
    {
        try
        {
            // gRPC Health-Check durchf�hren
            HealthCheckRequest grpcRequest = new HealthCheckRequest();
            HealthCheckResponse grpcResponse = await this.workoutServiceClient.CheckHealthAsync(grpcRequest, cancellationToken: cancellationToken);

            this.logger.LogInformation(
                "gRPC: Health check completed - Healthy: {IsHealthy}, Message: {Message}",
                grpcResponse.IsHealthy, grpcResponse.Message);

            return grpcResponse.IsHealthy;
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "gRPC: Health check failed");
            return false;
        }
    }

    public void Dispose()
    {
        this.channel?.Dispose();
        GC.SuppressFinalize(this);
    }
}