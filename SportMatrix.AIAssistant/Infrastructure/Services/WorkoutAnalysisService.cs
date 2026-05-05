namespace SportMatrix.AIAssistant.Infrastructure.Services;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;

public class WorkoutAnalysisService : IWorkoutAnalysisService
{
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
                "Analyzing workouts for {WorkoutCount} recent workouts, analysis type: {AnalysisType}",
                request.RecentWorkouts?.Count ?? 0, request.AnalysisType ?? "General");

            string prompt = this.BuildAnalysisPrompt(request);

            string aiResponse = request.AnalysisType?.ToLower() switch
            {
                "health" => await this.aiPromptService.GetHealthAnalysisAsync(prompt, cancellationToken),
                "performance" => await this.aiPromptService.GetFitnessAnalysisAsync(prompt, cancellationToken),
                "trends" => await this.aiPromptService.GetFitnessAnalysisAsync(prompt, cancellationToken),
                _ => await this.aiPromptService.GetFitnessAnalysisAsync(prompt, cancellationToken)
            };

            WorkoutAnalysisResponseDto result = this.ParseAnalysisResponse(aiResponse, request.AnalysisType);
            result.Provider = "Gemini-AI";

            this.logger.LogInformation(
                "Successfully generated workout analysis: {InsightCount} insights and {RecommendationCount} recommendations",
                result.KeyInsights?.Count ?? 0, result.Recommendations?.Count ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error analyzing workouts");
            return this.GetFallbackAnalysis(request, "Gemini-AI");
        }
    }

    private string BuildAnalysisPrompt(WorkoutAnalysisRequestDto request)
    {
        if (request.RecentWorkouts == null || !request.RecentWorkouts.Any())
        {
            return "No recent workout data available for analysis. Please provide workout data to generate insights.";
        }

        string workoutsData = string.Join("\n", request.RecentWorkouts.Select(w =>
            $"Date: {w.Date:yyyy-MM-dd}, Type: {w.ActivityType}, Distance: {w.Distance}m, " +
            $"Duration: {TimeSpan.FromSeconds(w.Duration):hh\\:mm\\:ss}, Calories: {w.Calories}"));

        string athleteContext = request.AthleteProfile != null ?
            $"\nAthlete Level: {request.AthleteProfile.FitnessLevel}\nPrimary Goal: {request.AthleteProfile.PrimaryGoal}" : string.Empty;

        return request.AnalysisType?.ToLower() switch
        {
            "health" => this.BuildHealthAnalysisPrompt(workoutsData, athleteContext),
            "performance" => this.BuildPerformanceAnalysisPrompt(workoutsData, athleteContext),
            "trends" => this.BuildTrendsAnalysisPrompt(workoutsData, athleteContext),
            _ => this.BuildGeneralAnalysisPrompt(workoutsData, athleteContext, request.AnalysisType ?? "General")
        };
    }

    private string BuildHealthAnalysisPrompt(string workoutsData, string athleteContext)
    {
        return $@"You are a health and fitness expert. Analyze the following training data for health insights:

TRAINING DATA:
{workoutsData}
{athleteContext}

Create a health-focused analysis with:

HEALTH ANALYSIS:
- Assessment of training load (appropriate/excessive?)
- Recovery patterns and recommendations
- Injury prevention insights
- Cardiovascular health indicators

KEY INSIGHTS:
- 3-4 specific health-related observations
- Warning signs if present

RECOMMENDATIONS:
- Health-oriented actionable advice
- Recovery strategies
- Training modifications for optimal health

Focus on health and injury prevention.";
    }

    private string BuildPerformanceAnalysisPrompt(string workoutsData, string athleteContext)
    {
        return $@"You are a performance coach. Analyze the following training data for performance optimization:

TRAINING DATA:
{workoutsData}
{athleteContext}

Create a performance-focused analysis with:

PERFORMANCE ANALYSIS:
- Progress assessment and trends
- Performance strengths and weaknesses
- Training efficiency evaluation
- Goal achievement potential

KEY INSIGHTS:
- 3-4 specific performance observations
- Identified improvement areas

RECOMMENDATIONS:
- Strategies for performance optimization
- Training intensity adjustments
- Specific techniques for improvement

Focus on athletic performance and competition improvement.";
    }

    private string BuildTrendsAnalysisPrompt(string workoutsData, string athleteContext)
    {
        return $@"You are a data analyst specialized in fitness trends. Analyze the following training patterns:

TRAINING DATA:
{workoutsData}
{athleteContext}

Create a trend-focused analysis with:

TREND ANALYSIS:
- Training consistency patterns over time
- Performance progress or decline
- Weekly/monthly pattern recognition
- Training variety and distribution

KEY INSIGHTS:
- 3-4 significant trend observations
- Pattern recognition findings

RECOMMENDATIONS:
- Trend-based training advice
- Strategies for consistency improvement
- Suggestions for future planning

Focus on patterns, trends, and long-term progress analysis.";
    }

    private string BuildGeneralAnalysisPrompt(string workoutsData, string athleteContext, string analysisType)
    {
        return $@"You are a fitness expert. Create a comprehensive analysis of the following training data:

TRAINING DATA:
{workoutsData}
{athleteContext}

Analysis focus: {analysisType}

Create a detailed fitness analysis with:

ANALYSIS:
- Overall training assessment
- Training effectiveness evaluation
- Progress indicators
- Areas for improvement

KEY INSIGHTS:
- 3-4 specific observations from the data
- Important patterns or trends
- Performance highlights

RECOMMENDATIONS:
- Actionable training advice
- Specific improvement strategies
- Goal-oriented suggestions

Provide practical, actionable insights for fitness improvement.";
    }

    private WorkoutAnalysisResponseDto ParseAnalysisResponse(string aiResponse, string? analysisType)
    {
        return new WorkoutAnalysisResponseDto
        {
            Analysis = this.ExtractAnalysisSection(aiResponse),
            KeyInsights = this.ExtractKeyInsights(aiResponse),
            Recommendations = this.ExtractRecommendations(aiResponse),
            GeneratedAt = DateTime.UtcNow,
        };
    }

    private string ExtractAnalysisSection(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return "Unable to generate analysis at this time. Please try again later.";
        }

        string structuredAnalysis = this.TryExtractStructuredAnalysis(aiResponse);
        if (!string.IsNullOrEmpty(structuredAnalysis))
        {
            return this.LimitAnalysisLength(structuredAnalysis);
        }

        return this.LimitAnalysisLength(this.ExtractFallbackAnalysis(aiResponse));
    }

    private string TryExtractStructuredAnalysis(string aiResponse)
    {
        string[] analysisHeaders = new[]
        {
            "ANALYSIS:", "HEALTH ANALYSIS:", "PERFORMANCE ANALYSIS:", "TRENDS ANALYSIS:",
        };

        foreach (string header in analysisHeaders)
        {
            if (!aiResponse.Contains(header, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string analysisSection = this.ExtractSectionContent(aiResponse, header);
            if (!string.IsNullOrWhiteSpace(analysisSection) && analysisSection.Length > 20)
            {
                return analysisSection;
            }
        }

        return string.Empty;
    }

    private string ExtractSectionContent(string aiResponse, string header)
    {
        string[] analysisParts = aiResponse.Split(header, StringSplitOptions.RemoveEmptyEntries);
        if (analysisParts.Length <= 1)
        {
            return string.Empty;
        }

        string[] stopMarkers = new[] { "KEY INSIGHTS:", "RECOMMENDATIONS:" };
        string analysisSection = analysisParts[1].Split(stopMarkers, StringSplitOptions.RemoveEmptyEntries)[0];

        return analysisSection.Trim();
    }

    private string ExtractFallbackAnalysis(string aiResponse)
    {
        string[] lines = aiResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        List<string> analysisLines = new List<string>();
        string[] stopMarkers = new[] { "KEY INSIGHTS:", "RECOMMENDATIONS:" };

        foreach (string line in lines)
        {
            string cleanLine = line.Trim();
            if (string.IsNullOrWhiteSpace(cleanLine))
            {
                continue;
            }

            if (stopMarkers.Any(marker =>
                cleanLine.StartsWith(marker, StringComparison.OrdinalIgnoreCase)))
            {
                break;
            }

            analysisLines.Add(cleanLine);

            if (analysisLines.Count >= 5)
            {
                break;
            }
        }

        return analysisLines.Any()
            ? string.Join(" ", analysisLines)
            : "Unable to generate analysis at this time. Please try again later.";
    }

    private string LimitAnalysisLength(string analysis)
    {
        if (analysis.Length <= 400)
        {
            return analysis;
        }

        string[] sentences = analysis.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(". ", sentences.Take(4)) + ".";
    }

    private List<string>? ExtractKeyInsights(string aiResponse)
    {
        return this.ExtractListSection(aiResponse, new[] { "KEY INSIGHTS:", "INSIGHTS:" });
    }

    private List<string>? ExtractRecommendations(string aiResponse)
    {
        return this.ExtractListSection(aiResponse, new[] { "RECOMMENDATIONS:", "ADVICE:" });
    }

    private List<string>? ExtractListSection(string aiResponse, string[] sectionHeaders)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return null;
        }

        foreach (string header in sectionHeaders)
        {
            List<string>? extractedItems = this.TryExtractFromHeader(aiResponse, header);
            if (extractedItems?.Any() == true)
            {
                return extractedItems;
            }
        }

        return null;
    }

    private List<string>? TryExtractFromHeader(string aiResponse, string header)
    {
        if (!aiResponse.Contains(header, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        string sectionContent = this.ExtractSectionUntilNextHeader(aiResponse, header);
        if (string.IsNullOrEmpty(sectionContent))
        {
            return null;
        }

        return this.ParseItemsFromSection(sectionContent);
    }

    private string ExtractSectionUntilNextHeader(string aiResponse, string currentHeader)
    {
        int headerIndex = aiResponse.IndexOf(currentHeader, StringComparison.OrdinalIgnoreCase);
        string section = aiResponse.Substring(headerIndex + currentHeader.Length);

        int nextHeaderIndex = this.FindNextHeaderIndex(section, currentHeader);
        if (nextHeaderIndex > 0)
        {
            section = section.Substring(0, nextHeaderIndex);
        }

        return section;
    }

    private int FindNextHeaderIndex(string section, string currentHeader)
    {
        string[] allHeaders = new[]
        {
            "ANALYSIS:", "KEY INSIGHTS:", "RECOMMENDATIONS:", "ADVICE:",
            "HEALTH ANALYSIS:", "PERFORMANCE ANALYSIS:", "TRENDS ANALYSIS:",
        };

        foreach (string nextHeader in allHeaders)
        {
            if (nextHeader == currentHeader)
            {
                continue;
            }

            if (section.Contains(nextHeader, StringComparison.OrdinalIgnoreCase))
            {
                return section.IndexOf(nextHeader, StringComparison.OrdinalIgnoreCase);
            }
        }

        return -1;
    }

    private List<string> ParseItemsFromSection(string sectionContent)
    {
        List<string> items = new List<string>();
        string[] lines = sectionContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string cleanedItem = line.Trim()
                .TrimStart('-', '*', '•', '1', '2', '3', '4', '5', '.', ' ')
                .Trim();

            if (!string.IsNullOrWhiteSpace(cleanedItem) &&
                cleanedItem.Length > 15 &&
                cleanedItem.Length < 200)
            {
                items.Add(cleanedItem);

                if (items.Count >= 5)
                {
                    break;
                }
            }
        }

        return items;
    }

    private WorkoutAnalysisResponseDto GetFallbackAnalysis(WorkoutAnalysisRequestDto request, string provider = "Unknown")
    {
        int workoutCount = request.RecentWorkouts?.Count ?? 0;
        double totalDistance = request.RecentWorkouts?.Sum(w => w.Distance) ?? 0;
        int totalDuration = request.RecentWorkouts?.Sum(w => w.Duration) ?? 0;
        double? avgCalories = request.RecentWorkouts?.Any() == true ?
            request.RecentWorkouts.Average(w => w.Calories) : 0;

        string analysisType = request.AnalysisType ?? "Performance";

        string analysis = analysisType.ToLower() switch
        {
            "health" => $"Based on your {workoutCount} recent training sessions, your training load appears well-balanced. " +
                       $"The total distance of {totalDistance:F1}km over {TimeSpan.FromSeconds(totalDuration):h\\:mm} shows good cardiovascular engagement. " +
                       $"No concerning overtraining patterns detected. Your average calorie consumption of {avgCalories:F0} per session indicates appropriate training intensity.",

            "performance" => $"Your performance data shows {workoutCount} completed training sessions with {totalDistance:F1}km total distance. " +
                           $"Training consistency appears strong with varying training types. " +
                           $"The average session duration of {TimeSpan.FromSeconds(workoutCount > 0 ? totalDuration / workoutCount : 0):h\\:mm} indicates good endurance development. " +
                           $"Performance metrics show steady progress toward your goals.",

            "trends" => $"Training trend analysis shows {workoutCount} training sessions over the recent period. " +
                       $"Total distance progress to {totalDistance:F1}km shows positive training consistency. " +
                       $"Training frequency and duration patterns suggest sustainable training habits. " +
                       $"Calorie consumption trends indicate effective energy management.",

            _ => $"Comprehensive analysis of your {workoutCount} recent training sessions over {totalDistance:F1}km shows excellent training consistency. " +
                $"Your {analysisType.ToLower()} metrics indicate steady progress toward your fitness goals. " +
                $"Training load and recovery balance appear appropriate for continued improvement."
        };

        List<string> insights = analysisType.ToLower() switch
        {
            "health" => new List<string>
            {
                $"{workoutCount} training sessions completed without overtraining indicators",
                $"Average calorie consumption of {avgCalories:F0} indicates appropriate intensity",
                "Training frequency supports good cardiovascular health",
                "No concerning health patterns detected in the training data",
            },
            "performance" => new List<string>
            {
                $"{totalDistance:F1}km total distance achieved over {workoutCount} sessions",
                "Training consistency shows strong commitment to performance goals",
                $"Average training intensity of {avgCalories:F0} calories is performance-oriented",
                "Training variety supports diverse athletic development",
            },
            "trends" => new List<string>
            {
                $"Training frequency of {workoutCount} sessions shows consistent habit formation",
                "Distance and duration trends indicate application of progressive overload",
                "Calorie consumption patterns indicate effective training intensity management",
                "Overall trajectory indicates sustained fitness improvement",
            },
            _ => new List<string>
            {
                $"{workoutCount} training sessions completed with total distance of {totalDistance:F1}km",
                "Training consistency shows commitment to fitness goals",
                "Performance metrics indicate steady improvement trajectory",
                "Training intensity and frequency appear well-balanced",
            }
        };

        List<string> recommendations = analysisType.ToLower() switch
        {
            "health" => new List<string>
            {
                "Continue current training plan to obtain health benefits",
                "Monitor recovery signs and adjust intensity when fatigued",
                "Ensure adequate sleep and nutrition to support training load",
                "Add mobility work to prevent injuries",
            },
            "performance" => new List<string>
            {
                "Gradually increase training intensity by 5-10% for performance gains",
                "Add interval training to promote speed and strength development",
                "Consider performance tests to track specific improvements",
                "Integrate sport-specific exercises for targeted skill development",
            },
            "trends" => new List<string>
            {
                "Maintain current training frequency for sustained positive trends",
                "Plan progressive increases in distance and duration",
                "Track weekly trends to identify optimal training patterns",
                "Set monthly goals based on current progress rate",
            },
            _ => new List<string>
            {
                "Continue with current training plan and intensity",
                "Gradually increase training difficulty by 5-10% every 2-3 weeks",
                "Ensure adequate recovery between intense training sessions",
                "Add training variety for balanced development",
            }
        };

        return new WorkoutAnalysisResponseDto
        {
            Analysis = analysis,
            KeyInsights = insights,
            Recommendations = recommendations,
            GeneratedAt = DateTime.UtcNow,
            Provider = provider,
        };
    }
}
