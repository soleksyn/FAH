namespace SportMatrix.AIAssistant.Infrastructure.Services;

using System.Text.RegularExpressions;
using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.Interfaces;

public class MotivationCoachService : IMotivationCoachService
{
    private readonly IAIPromptService aiPromptService;
    private readonly ILogger<MotivationCoachService> logger;

    public MotivationCoachService(
        IAIPromptService aiPromptService,
        ILogger<MotivationCoachService> logger)
    {
        this.aiPromptService = aiPromptService;
        this.logger = logger;
    }

    public async Task<MotivationResponseDto> GenerateMotivationAsync(MotivationRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation(
                "Generating motivational message for athlete: {Name}",
                request.AthleteProfile?.Name ?? "Unknown");

            string prompt = this.BuildMotivationPrompt(request);

            string aiResponse = await this.aiPromptService.GetMotivationAsync(prompt, cancellationToken);

            MotivationResponseDto result = this.ParseMotivationResponse(aiResponse);

            this.logger.LogInformation(
                "Successfully generated motivational message with {MessageLength} characters",
                result.MotivationalMessage?.Length ?? 0);

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error generating motivational message");

            return new MotivationResponseDto
            {
                MotivationalMessage = this.GetFallbackMotivation(request),
                Quote = "\"Success is the sum of small efforts repeated day in and day out.\" - Robert Collier",
                ActionableTips = new List<string>
                {
                    "Set small, achievable goals for today",
                    "Focus on consistency over perfection",
                    "Celebrate every small victory",
                },
                GeneratedAt = DateTime.UtcNow,
            };
        }
    }

    private string BuildMotivationPrompt(MotivationRequestDto request)
    {
        string athleteName = request.AthleteProfile?.Name ?? "Champion";
        string fitnessLevel = request.AthleteProfile?.FitnessLevel ?? "Beginner";
        string primaryGoal = request.AthleteProfile?.PrimaryGoal ?? "General Fitness";

        string lastWorkoutInfo = request.LastWorkout != null ?
            $"Last workout: {request.LastWorkout.ActivityType}, {request.LastWorkout.Distance}km, " +
            $"{TimeSpan.FromSeconds(request.LastWorkout.Duration):hh\\:mm\\:ss}" :
            "No recent workout data available";

        string motivationLevel = request.IsStruggling ?
            "The athlete is currently struggling with motivation and needs extra encouragement." :
            "The athlete is looking for additional motivation to stay on track.";

        return $@"Create a motivational fitness message for {athleteName}.

Athlete Profile:
- Fitness Level: {fitnessLevel}
- Primary Goal: {primaryGoal}
- {lastWorkoutInfo}
- {motivationLevel}

Generate a motivational response with:
1. Personal encouragement (2-3 sentences)
2. An inspiring fitness quote
3. 2-3 actionable tips

Keep it positive, personal, and energizing!

Response:";
    }

    private MotivationResponseDto ParseMotivationResponse(string aiResponse)
    {
        return new MotivationResponseDto
        {
            MotivationalMessage = this.ExtractMotivationalMessage(aiResponse),
            Quote = this.ExtractQuote(aiResponse),
            ActionableTips = this.ExtractTips(aiResponse),
            GeneratedAt = DateTime.UtcNow,
        };
    }

    private string ExtractMotivationalMessage(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return "You're doing great! Keep up the excellent work with your fitness journey.";
        }

        // Extract main message (first paragraph or until Quote/Tips section)
        string[] lines = aiResponse.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        List<string> messageLines = new List<string>();

        foreach (string line in lines)
        {
            string cleanLine = line.Trim();

            // Stop at structured sections
            if (cleanLine.StartsWith("Quote:", StringComparison.OrdinalIgnoreCase) ||
                cleanLine.StartsWith("Tips:", StringComparison.OrdinalIgnoreCase) ||
                cleanLine.StartsWith("Actionable", StringComparison.OrdinalIgnoreCase) ||
                cleanLine.StartsWith("1.", StringComparison.OrdinalIgnoreCase) ||
                cleanLine.StartsWith("2.", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            // Skip standalone quoted strings
            if (cleanLine.StartsWith('"') && cleanLine.EndsWith('"') && cleanLine.Length > 20)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(cleanLine))
            {
                messageLines.Add(cleanLine);
            }
        }

        string result = messageLines.Any() ? string.Join(" ", messageLines) :
                    "You're doing great! Keep up the excellent work with your fitness journey.";

        // Limit length
        if (result.Length > 300)
        {
            string[] sentences = result.Split('.', StringSplitOptions.RemoveEmptyEntries);
            result = string.Join(". ", sentences.Take(3)) + ".";
        }

        return result;
    }

    private string? ExtractQuote(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return null;
        }

        // Search for quoted strings
        MatchCollection quoteMatches = System.Text.RegularExpressions.Regex.Matches(
            aiResponse, @"""([^""]{10,})""", RegexOptions.None, TimeSpan.FromMilliseconds(100));

        foreach (System.Text.RegularExpressions.Match match in quoteMatches)
        {
            string quote = match.Groups[1].Value.Trim();

            // Filter motivational quotes (exclude technical texts)
            if (quote.Length >= 15 && quote.Length <= 150 &&
                (quote.Contains("success") || quote.Contains("achieve") || quote.Contains("goal") ||
                 quote.Contains("dream") || quote.Contains("believe") || quote.Contains("strong") ||
                 quote.Contains("push") || quote.Contains("better") || quote.Contains("begin") ||
                 quote.Contains("real")))
            {
                return quote;
            }
        }

        if (aiResponse.Contains("Quote:", StringComparison.OrdinalIgnoreCase))
        {
            string[] quoteParts = aiResponse.Split("Quote:", StringSplitOptions.RemoveEmptyEntries);
            if (quoteParts.Length > 1)
            {
                string quoteLine = quoteParts[1].Split('\n')[0].Trim().Trim('"', '-', '*').Trim();
                if (!string.IsNullOrWhiteSpace(quoteLine) && quoteLine.Length >= 15)
                {
                    return quoteLine;
                }
            }
        }

        return null;
    }

    private List<string>? ExtractTips(string aiResponse)
    {
        if (string.IsNullOrWhiteSpace(aiResponse))
        {
            return null;
        }

        List<string> tips = new List<string>();

        // Search for "Tips:" or similar labels
        string tipsSection = string.Empty;
        string lowerResponse = aiResponse.ToLower();

        if (lowerResponse.Contains("tips:"))
        {
            tipsSection = aiResponse.Substring(lowerResponse.IndexOf("tips:") + 5);
        }
        else if (lowerResponse.Contains("actionable"))
        {
            int actionableIndex = lowerResponse.IndexOf("actionable");
            tipsSection = aiResponse.Substring(actionableIndex);
        }

        if (!string.IsNullOrWhiteSpace(tipsSection))
        {
            string[] lines = tipsSection.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string cleanLine = line.Trim()
                    .TrimStart('-', '*', '•', '1', '2', '3', '4', '5', '.', ' ')
                    .Trim();

                if (!string.IsNullOrWhiteSpace(cleanLine) &&
                    cleanLine.Length > 15 &&
                    cleanLine.Length < 150 &&
                    !cleanLine.StartsWith("Quote", StringComparison.OrdinalIgnoreCase))
                {
                    tips.Add(cleanLine);
                    if (tips.Count >= 3)
                    {
                        break;
                    }
                }
            }
        }

        return tips.Any() ? tips : null;
    }

    private string GetFallbackMotivation(MotivationRequestDto request)
    {
        string athleteName = request.AthleteProfile?.Name ?? "Champion";
        string primaryGoal = request.AthleteProfile?.PrimaryGoal ?? "fitness goals";
        string fitnessLevel = request.AthleteProfile?.FitnessLevel ?? "current";

        string[] motivations = new[]
        {
            $"Great job, {athleteName}! Your consistency in training is inspiring. Every workout brings you closer to your {primaryGoal}.",
            $"You're making excellent progress, {athleteName}! Your dedication to fitness shows real commitment to your health and goals.",
            $"Keep pushing forward, {athleteName}! Your {fitnessLevel} level shows you have what it takes to achieve great things.",
            $"Amazing work, {athleteName}! Your commitment to {primaryGoal} is paying off. Stay strong and keep moving forward!",
        };

        int randomIndex = Random.Shared.Next(motivations.Length);
        return motivations[randomIndex];
    }
}
