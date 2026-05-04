namespace FitnessAnalyticsHub.AIAssistant.Infrastructure.Services;

using global::AIAssistant.Application.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

public class GoogleGeminiService : IAIPromptService
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;
    private readonly ILogger<GoogleGeminiService> logger;

    // Google Gemini Modelle
    private readonly Dictionary<string, string> models = new()
    {
        { "fitness", "gemini-1.5-flash" },      // Kostenlos, schnell
        { "health", "gemini-1.5-flash" },       // Kostenlos, schnell
        { "motivation", "gemini-1.5-flash" },   // Kostenlos, schnell
        { "analysis", "gemini-1.5-pro" },       // Bessere Qualität (falls verfügbar)
    };

    public GoogleGeminiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GoogleGeminiService> logger)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
        this.logger = logger;

        // Base URL für Google Gemini API
        this.httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    }

    public Task<string> GetFitnessAnalysisAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, "fitness", cancellationToken);
    }

    public Task<string> GetHealthAnalysisAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, "health", cancellationToken);
    }

    public Task<string> GetMotivationAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, "motivation", cancellationToken);
    }

    private async Task<string> GetGeminiCompletionAsync(string prompt, string modelType, CancellationToken cancellationToken)
    {
        string enhancedPrompt = this.CreateEnhancedPrompt(prompt, modelType);

        try
        {
            string model = this.models.GetValueOrDefault(modelType, "gemini-1.5-flash");
            string apiKey = this.GetApiKey();

            this.logger.LogInformation(
                "Calling Google Gemini API with model: {Model} for type: {ModelType}",
                model, modelType);

            // Google Gemini API Request Format
            var requestPayload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = enhancedPrompt },
                        },
                    },
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 1000,
                    topP = 0.95,
                    topK = 40,
                },
            };

            string jsonContent = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

            StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Google Gemini API Endpoint
            string endpoint = $"v1beta/models/{model}:generateContent?key={apiKey}";

            this.logger.LogDebug("Request URL: {Url}", endpoint);
            this.logger.LogDebug("Request payload: {Payload}", jsonContent);

            HttpResponseMessage response = await this.httpClient.PostAsync(endpoint, content, cancellationToken);
            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            this.logger.LogDebug("Response status: {StatusCode}", response.StatusCode);
            this.logger.LogDebug("Response content: {Content}", responseContent);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("Google Gemini API error: {StatusCode} - {Error}", response.StatusCode, responseContent);

                // Spezifische Fehlerbehandlung
                this.LogGeminiStatus(response.StatusCode);

                return this.GetFallbackResponse(modelType, ((int)response.StatusCode).ToString());
            }

            // Parse Google Gemini Response
            JsonElement responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);

            if (responseJson.TryGetProperty("candidates", out JsonElement candidates) &&
                candidates.GetArrayLength() > 0)
            {
                JsonElement firstCandidate = candidates[0];
                if (firstCandidate.TryGetProperty("content", out JsonElement content_) &&
                    content_.TryGetProperty("parts", out JsonElement parts) &&
                    parts.GetArrayLength() > 0)
                {
                    JsonElement firstPart = parts[0];
                    if (firstPart.TryGetProperty("text", out JsonElement text))
                    {
                        string result = text.GetString() ?? string.Empty;
                        this.logger.LogInformation("Successfully received response from Google Gemini API");
                        return result.Trim();
                    }
                }
            }

            this.logger.LogWarning("Unexpected response format from Google Gemini API");
            this.logger.LogDebug("Full response: {Response}", responseContent);
            return this.GetFallbackResponse(modelType, "unexpected_format");
        }
        catch (JsonException ex)
        {
            this.logger.LogWarning("Invalid JSON response from Google Gemini API: {Error}", ex.Message);
            return this.GetFallbackResponse(modelType, "invalid_json");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            this.logger.LogWarning("Google Gemini request timeout for model type: {ModelType}", modelType);
            return this.GetFallbackResponse(modelType, "timeout");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error calling Google Gemini API for {ModelType}", modelType);
            return this.GetFallbackResponse(modelType, ex.Message);
        }
    }

    private void LogGeminiStatus(HttpStatusCode statusCode)
    {
        switch (statusCode)
        {
            case HttpStatusCode.Unauthorized:
                this.logger.LogError("UNAUTHORIZED: Check your Google AI API key!");
                break;
            case HttpStatusCode.TooManyRequests:
                this.logger.LogWarning("RATE LIMITED: Google Gemini rate limit exceeded");
                break;
            case HttpStatusCode.BadRequest:
                this.logger.LogError("BAD REQUEST: Check your request format or API key");
                break;
        }
    }

    private string GetApiKey()
    {
        string? apiKey = this.configuration["GoogleAI:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("Google AI API Key not configured. Please set 'GoogleAI:ApiKey' in configuration.");
        }

        return apiKey;
    }

    private string CreateEnhancedPrompt(string originalPrompt, string modelType)
    {
        string systemContext = modelType.ToLower() switch
        {
            "motivation" => @"You are an enthusiastic fitness trainer.

Respond in the following structured Markdown format:

## 💪 MOTIVATION
[Short motivational introduction]

## ✨ FOCUS TODAY
• **[Point 1]** - [Short explanation]
• **[Point 2]** - [Short explanation]
• **[Point 3]** - [Short explanation]

## 🎯 GOAL IN SIGHT
[Motivational closing words with concrete goal]

Use emojis and format with Markdown for better readability.",

            "fitness" => @"You are a fitness expert who analyzes training data.

Create a structured analysis in the following Markdown format:

## 📊 TRAINING ANALYSIS
**Overall assessment:** [1-2 sentences on general performance]
**Training volume:** [Assessment of frequency and duration]
**Intensity:** [Assessment of training intensity]

## 💡 KEY INSIGHTS
• **[Insight 1]** - [Detailed explanation]
• **[Insight 2]** - [Detailed explanation]
• **[Insight 3]** - [Detailed explanation]

## 🚀 RECOMMENDATIONS
1. **Immediately actionable:** [Concrete action for this week]
2. **Medium-term:** [Strategic adjustment for next month]
3. **Long-term:** [Goal-oriented recommendation for 3+ months]

Use precise fitness terminology and concrete numbers.",

            "health" => @"You are a health expert who analyzes fitness data for wellness insights.

Create a structured health analysis in Markdown format:

## 🏥 HEALTH ANALYSIS
**Load management:** [Assessment of training load]
**Recovery:** [Estimation of recovery phases]
**Injury risk:** [Risk assessment based on data]

## ⚠️ HEALTH INDICATORS
• **[Indicator 1]** - [Health significance]
• **[Indicator 2]** - [Health significance]
• **[Indicator 3]** - [Health significance]

## 🌱 WELLNESS RECOMMENDATIONS
1. **Recovery:** [Concrete recovery measures]
2. **Prevention:** [Injury prevention]
3. **Long-term health:** [Sustainable training approaches]

Focus on health and sustainable training habits.",

            "analysis" => @"You are a sports scientist who analyzes athletic performance data.

Create a detailed performance analysis in Markdown format:

## 📈 PERFORMANCE ANALYSIS
**Performance trend:** [Development of performance over time]
**Efficiency:** [Ratio of effort to result]
**Strengths/weaknesses:** [Identified performance areas]

## 🔍 DATA INSIGHTS
• **[Metric 1]** - [Sports science interpretation]
• **[Metric 2]** - [Sports science interpretation]
• **[Metric 3]** - [Sports science interpretation]

## ⚡ PERFORMANCE OPTIMIZATION
1. **Technique:** [Improvements in execution]
2. **Training:** [Adjustments in training plan]
3. **Periodization:** [Long-term planning]

Use sports science terminology and quantitative analyses.",

        _ => @"You are a helpful fitness assistant.

Respond in structured Markdown format:

## 📝 ANALYSIS
[Main analysis of the situation]

## 💡 INSIGHTS
• **[Point 1]** - [Explanation]
• **[Point 2]** - [Explanation]
• **[Point 3]** - [Explanation]

## 🎯 RECOMMENDATIONS
1. [Concrete action 1]
2. [Concrete action 2]
3. [Concrete action 3]

Be helpful and informative.",
    };

    return $"{systemContext}\n\n{originalPrompt}";
}

private string GetFallbackResponse(string modelType, string errorType)
{
    this.logger.LogInformation("Generating fallback response for {ModelType} due to: {Error}", modelType, errorType);

    return modelType.ToLower() switch
        {
            "motivation" => $"Keep going! Every workout brings you closer to your goals. You've got this! 💪 (Note: AI analysis temporarily unavailable - {errorType})",

            "fitness" => $"Your training data shows consistent training patterns and positive progress. Continue your current approach and focus on gradual improvement. (Note: Detailed analysis temporarily unavailable - {errorType})",

            "health" => $"Your training patterns indicate a healthy fitness approach. Continue to maintain a good balance between activity and recovery. (Note: Health analysis temporarily unavailable - {errorType})",

            "analysis" => $"Your performance data shows steady improvement and good training consistency. Focus on maintaining your current momentum. (Note: Detailed analysis temporarily unavailable - {errorType})",

            _ => $"Your fitness journey shows excellent progress! Continue your dedicated approach and maintain consistency in your training routine. (Note: AI analysis temporarily unavailable - {errorType})"
        };
    }
}