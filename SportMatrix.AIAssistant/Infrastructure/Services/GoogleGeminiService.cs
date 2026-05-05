namespace SportMatrix.AIAssistant.Infrastructure.Services;

using global::Google.GenAI;
using global::Google.GenAI.Types;
using global::SportMatrix.AIAssistant.Application.Interfaces;
using Microsoft.Extensions.Options;
using SportMatrix.AIAssistant.Infrastructure.Configuration;

public class GoogleGeminiService : IAIPromptService
{
    private readonly Client client;
    private readonly GoogleAIOptions options;
    private readonly ILogger<GoogleGeminiService> logger;

    public GoogleGeminiService(
        IOptions<GoogleAIOptions> options,
        ILogger<GoogleGeminiService> logger)
    {
        this.options = options.Value;
        this.logger = logger;
        this.client = new Client(apiKey: this.options.ApiKey);
    }

    public Task<string> GetFitnessAnalysisAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, this.options.Models.Fitness, "You are a fitness expert who analyzes training data. Respond in Markdown format.", cancellationToken);
    }

    public Task<string> GetHealthAnalysisAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, this.options.Models.Health, "You are a health expert who analyzes fitness data for wellness insights. Respond in Markdown format.", cancellationToken);
    }

    public Task<string> GetMotivationAsync(string prompt, CancellationToken cancellationToken)
    {
        return this.GetGeminiCompletionAsync(prompt, this.options.Models.Motivation, "You are an enthusiastic fitness trainer. Provide motivational and encouraging responses in Markdown.", cancellationToken);
    }

    private async Task<string> GetGeminiCompletionAsync(string prompt, string model, string systemInstruction, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Calling Google Gemini API with model: {Model}", model);

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                },
                Temperature = this.options.GenerationConfig.Temperature,
                MaxOutputTokens = this.options.GenerationConfig.MaxOutputTokens,
                TopP = this.options.GenerationConfig.TopP,
            };

            var response = await this.client.Models.GenerateContentAsync(
                model: model,
                contents: prompt,
                config: config,
                cancellationToken: cancellationToken);

            var resultText = response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (!string.IsNullOrEmpty(resultText))
            {
                this.logger.LogInformation("Successfully received response from Google Gemini API");
                return resultText.Trim();
            }

            this.logger.LogWarning("Unexpected empty response from Google Gemini API");
            return this.GetFallbackResponse(model, "empty_response");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            this.logger.LogWarning("Google Gemini request timeout for model: {Model}", model);
            return this.GetFallbackResponse(model, "timeout");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error calling Google Gemini API for model: {Model}", model);
            return this.GetFallbackResponse(model, ex.Message);
        }
    }

    private string GetFallbackResponse(string model, string errorType)
    {
        this.logger.LogInformation("Generating fallback response for {Model} due to: {Error}", model, errorType);
        return $"System currently unavailable. Please try again later. (Error: {errorType})";
    }
}
