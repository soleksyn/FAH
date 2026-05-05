using System.Text.Json.Serialization;

namespace SportMatrix.AIAssistant.Infrastructure.Models;

public class GeminiRequest
{
    [JsonPropertyName("system_instruction")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public GeminiContent? SystemInstruction { get; set; }

    [JsonPropertyName("contents")]
    public List<GeminiContent> Contents { get; set; } = new();

    [JsonPropertyName("generationConfig")]
    public GeminiGenerationConfig? GenerationConfig { get; set; }
}

public class GeminiContent
{
    [JsonPropertyName("role")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Role { get; set; } // "user" or "model" or null for system

    [JsonPropertyName("parts")]
    public List<GeminiPart> Parts { get; set; } = new();
}

public class GeminiPart
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class GeminiGenerationConfig
{
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    [JsonPropertyName("maxOutputTokens")]
    public int MaxOutputTokens { get; set; }

    [JsonPropertyName("topP")]
    public float TopP { get; set; }

    [JsonPropertyName("topK")]
    public int TopK { get; set; }
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<GeminiCandidate>? Candidates { get; set; }
}

public class GeminiCandidate
{
    [JsonPropertyName("content")]
    public GeminiContent? Content { get; set; }

    [JsonPropertyName("finishReason")]
    public string? FinishReason { get; set; }
}
