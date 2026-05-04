namespace SportMatrix.AIAssistant.UI.API.Services;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;
using SportMatrix.AIAssistant.Extensions;
using SportMatrix;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Extensions;
using Grpc.Core;

public class WorkoutAnalysisGrpcService : Sportmatrix.WorkoutService.WorkoutServiceBase
{
    private readonly IWorkoutAnalysisService workoutAnalysisService;
    private readonly ILogger<WorkoutAnalysisGrpcService> logger;

    public WorkoutAnalysisGrpcService(
        IWorkoutAnalysisService workoutAnalysisService,
        ILogger<WorkoutAnalysisGrpcService> logger)
    {
        this.workoutAnalysisService = workoutAnalysisService;
        this.logger = logger;
    }

    public override async Task<Sportmatrix.WorkoutAnalysisResponse> GetWorkoutAnalysis(
        Sportmatrix.WorkoutAnalysisRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received workout analysis request for {WorkoutCount} workouts",
                request.RecentWorkouts?.Count ?? 0);

            // Konvertiere gRPC Request zu Application DTO
            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto analysisRequest = request.ToWorkoutAnalysisRequestDto();

            // Rufe den echten HuggingFace/GoogleGemini Service auf!
            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto response;

            // Bestimme welcher AI-Service verwendet werden soll basierend auf Request
            string aiProvider = request.PreferredAiProvider?.ToLower() ?? "huggingface";

            if (aiProvider == "googlegemini")
            {
                response = await this.workoutAnalysisService.AnalyzeGoogleGeminiWorkoutsAsync(analysisRequest, context.CancellationToken);
            }
            else
            {
                response = await this.workoutAnalysisService.AnalyzeHuggingFaceWorkoutsAsync(analysisRequest, context.CancellationToken);
            }

            // Konvertiere zur?ck zu gRPC Response
            Sportmatrix.WorkoutAnalysisResponse grpcResponse = new global::Sportmatrix.WorkoutAnalysisResponse
            {
                Analysis = response.Analysis ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = response.Provider ?? $"{aiProvider}-AI",
            };

            // Key Insights hinzuf?gen
            if (response.KeyInsights != null)
            {
                grpcResponse.KeyInsights.AddRange(response.KeyInsights);
            }

            // Recommendations hinzuf?gen
            if (response.Recommendations != null)
            {
                grpcResponse.Recommendations.AddRange(response.Recommendations);
            }

            this.logger.LogInformation("gRPC: Successfully generated workout analysis response using {Provider}", aiProvider);
            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error generating workout analysis");

            // gRPC Exception werfen
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to generate workout analysis: {ex.Message}"));
        }
    }

    // Zus?tzlicher Service f?r Performance Trends
    public override async Task<global::Sportmatrix.WorkoutAnalysisResponse> GetPerformanceTrends(
        global::Sportmatrix.PerformanceTrendsRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received performance trends request for athlete: {AthleteId}",
                request.AthleteId);

            // Konvertiere zu Sportmatrix.WorkoutAnalysisRequest
            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto analysisRequest = new SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto
            {
                AnalysisType = "Trends",
                RecentWorkouts = this.GetDemoWorkouts(request.AthleteId, request.TimeFrame),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "timeFrame", request.TimeFrame },
                    { "athleteId", request.AthleteId },
                },
            };

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeHuggingFaceWorkoutsAsync(analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = new global::Sportmatrix.WorkoutAnalysisResponse
            {
                Analysis = response.Analysis ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = response.Provider ?? "HuggingFace-AI",
                AnalysisType = "PerformanceTrends",
            };

            if (response.KeyInsights != null)
            {
                grpcResponse.KeyInsights.AddRange(response.KeyInsights);
            }

            if (response.Recommendations != null)
            {
                grpcResponse.Recommendations.AddRange(response.Recommendations);
            }

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting performance trends");
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to get performance trends: {ex.Message}"));
        }
    }

    // Training Recommendations Service
    public override async Task<global::Sportmatrix.WorkoutAnalysisResponse> GetTrainingRecommendations(
        global::Sportmatrix.TrainingRecommendationsRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received training recommendations request for athlete: {AthleteId}",
                request.AthleteId);

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto analysisRequest = new SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto
            {
                AnalysisType = "Recommendations",
                RecentWorkouts = this.GetDemoWorkouts(request.AthleteId, "week"),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "training_optimization" },
                    { "athleteId", request.AthleteId },
                },
            };

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeHuggingFaceWorkoutsAsync(analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = new global::Sportmatrix.WorkoutAnalysisResponse
            {
                Analysis = response.Analysis ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = response.Provider ?? "HuggingFace-AI",
                AnalysisType = "TrainingRecommendations",
            };

            if (response.KeyInsights != null)
            {
                grpcResponse.KeyInsights.AddRange(response.KeyInsights);
            }

            if (response.Recommendations != null)
            {
                grpcResponse.Recommendations.AddRange(response.Recommendations);
            }

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error getting training recommendations");
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to get training recommendations: {ex.Message}"));
        }
    }

    // Health Metrics Analysis Service
    public override async Task<global::Sportmatrix.WorkoutAnalysisResponse> AnalyzeHealthMetrics(
        global::Sportmatrix.HealthAnalysisRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received health metrics analysis request for athlete: {AthleteId}",
                request.AthleteId);

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto analysisRequest = new SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto
            {
                AnalysisType = "Health",
                RecentWorkouts = request.RecentWorkouts
                    .Select(w => w.ToWorkoutDataDto())
                    .ToList(),
                AthleteProfile = this.GetDemoAthleteProfile(request.AthleteId),
                AdditionalContext = new Dictionary<string, object>
                {
                    { "focus", "injury_prevention" },
                    { "health_analysis", true },
                    { "athleteId", request.AthleteId },
                },
            };

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeHuggingFaceWorkoutsAsync(analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = new global::Sportmatrix.WorkoutAnalysisResponse
            {
                Analysis = response.Analysis ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = response.Provider ?? "HuggingFace-AI",
                AnalysisType = "HealthMetrics",
            };

            if (response.KeyInsights != null)
            {
                grpcResponse.KeyInsights.AddRange(response.KeyInsights);
            }

            if (response.Recommendations != null)
            {
                grpcResponse.Recommendations.AddRange(response.Recommendations);
            }

            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error analyzing health metrics");
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to analyze health metrics: {ex.Message}"));
        }
    }

    // Separate GoogleGemini Analysis Service
    public override async Task<global::Sportmatrix.WorkoutAnalysisResponse> AnalyzeGoogleGeminiWorkouts(
        global::Sportmatrix.WorkoutAnalysisRequest request,
        ServerCallContext context)
    {
        try
        {
            this.logger.LogInformation(
                "gRPC: Received GoogleGemini workout analysis request for {WorkoutCount} workouts",
                request.RecentWorkouts?.Count ?? 0);

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto analysisRequest = request.ToWorkoutAnalysisRequestDto();

            // Zwinge GoogleGemini Service
            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto response = await this.workoutAnalysisService.AnalyzeGoogleGeminiWorkoutsAsync(analysisRequest, context.CancellationToken);

            Sportmatrix.WorkoutAnalysisResponse grpcResponse = new global::Sportmatrix.WorkoutAnalysisResponse
            {
                Analysis = response.Analysis ?? string.Empty,
                GeneratedAt = response.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Source = response.Provider ?? "GoogleGemini-AI",
            };

            if (response.KeyInsights != null)
            {
                grpcResponse.KeyInsights.AddRange(response.KeyInsights);
            }

            if (response.Recommendations != null)
            {
                grpcResponse.Recommendations.AddRange(response.Recommendations);
            }

            this.logger.LogInformation("gRPC: Successfully generated GoogleGemini workout analysis response");
            return grpcResponse;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Error generating GoogleGemini workout analysis");
            throw new RpcException(new Status(
                StatusCode.Internal,
                $"Failed to generate GoogleGemini workout analysis: {ex.Message}"));
        }
    }

    // Health Check f?r gRPC
    public override async Task<global::Sportmatrix.HealthCheckResponse> CheckHealth(
        global::Sportmatrix.HealthCheckRequest request,
        ServerCallContext context)
    {
        try
        {
            // Einfacher Test-Request
            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto testRequest = new SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisRequestDto
            {
                AnalysisType = "Health Check",
                RecentWorkouts = new List<WorkoutDataDto>
                {
                    new WorkoutDataDto
                    {
                        Date = DateTime.Now.AddDays(-1),
                        ActivityType = "Run",
                        Distance = 5.0,
                        Duration = 1800,
                        Calories = 350,
                    },
                },
                AthleteProfile = new AthleteProfileDto
                {
                    Name = "Test User",
                    FitnessLevel = "Intermediate",
                    PrimaryGoal = "Health Check",
                },
            };

            SportMatrix.AIAssistant.Application.DTOs.WorkoutAnalysisResponseDto result = await this.workoutAnalysisService.AnalyzeHuggingFaceWorkoutsAsync(testRequest, context.CancellationToken);

            return new Sportmatrix.HealthCheckResponse
            {
                IsHealthy = !string.IsNullOrEmpty(result.Analysis),
                Message = "Workout analysis service is responding",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "gRPC: Health check failed");
            return new Sportmatrix.HealthCheckResponse
            {
                IsHealthy = false,
                Message = $"Health check failed: {ex.Message}",
                Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            };
        }
    }

    private List<WorkoutDataDto> GetDemoWorkouts(int athleteId, string timeFrame)
    {
        return new List<WorkoutDataDto>
        {
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-1),
                ActivityType = "Run",
                Distance = 5.2,
                Duration = 1800,
                Calories = 350,
                MetricsData = new Dictionary<string, double> { { "heartRate", 145 } },
            },
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-3),
                ActivityType = "Ride",
                Distance = 24.8,
                Duration = 4500,
                Calories = 890,
                MetricsData = new Dictionary<string, double> { { "heartRate", 132 } },
            },
            new WorkoutDataDto
            {
                Date = DateTime.Now.AddDays(-5),
                ActivityType = "Run",
                Distance = 3.1,
                Duration = 1080,
                Calories = 245,
                MetricsData = new Dictionary<string, double> { { "heartRate", 128 } },
            },
        };
    }

    private AthleteProfileDto GetDemoAthleteProfile(int athleteId)
    {
        return new AthleteProfileDto
        {
            Id = athleteId.ToString(),
            Name = "Demo User",
            FitnessLevel = "Intermediate",
            PrimaryGoal = "Endurance Improvement",
            Preferences = new Dictionary<string, object>
            {
                { "preferredActivities", new[] { "Run", "Ride" } },
                { "trainingDays", 4 },
            },
        };
    }
}