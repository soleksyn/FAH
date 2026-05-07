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

    public Task<string> GetStructuredAnalysisAsync(
        string prompt,
        string systemInstruction,
        Schema responseSchema,
        CancellationToken cancellationToken)
    {
        return this.GetGeminiJsonCompletionAsync(prompt, this.options.Models.Fitness, systemInstruction, responseSchema, cancellationToken);
    }


    private async Task<string> GetGeminiJsonCompletionAsync(
        string prompt,
        string model,
        string systemInstruction,
        Schema responseSchema,
        CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Calling Google Gemini API (JSON mode) with model: {Model}", model);

            var config = new GenerateContentConfig
            {
                SystemInstruction = new Content
                {
                    Parts = new List<Part> { new Part { Text = systemInstruction } }
                },
                Temperature = this.options.GenerationConfig.Temperature,
                MaxOutputTokens = this.options.GenerationConfig.MaxOutputTokens,
                TopP = this.options.GenerationConfig.TopP,
                ResponseMimeType = "application/json",
                ResponseSchema = responseSchema,
            };

            var response = await this.client.Models.GenerateContentAsync(
                model: model,
                contents: prompt,
                config: config,
                cancellationToken: cancellationToken);

            var resultText = response?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            if (!string.IsNullOrEmpty(resultText))
            {
                this.logger.LogInformation("Successfully received JSON response from Google Gemini API");
                return resultText.Trim();
            }

            this.logger.LogWarning("Empty JSON response from Google Gemini API for model: {Model}", model);
            return string.Empty;
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            this.logger.LogWarning("Google Gemini request timeout for model: {Model}", model);
            return string.Empty;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error calling Google Gemini API (JSON mode) for model: {Model}", model);
            return string.Empty;
        }
    }


}

