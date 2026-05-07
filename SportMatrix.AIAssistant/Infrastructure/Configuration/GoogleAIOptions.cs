namespace SportMatrix.AIAssistant.Infrastructure.Configuration;

public class GoogleAIOptions
{
    public const string SectionName = "GoogleAI";

    public string ApiKey { get; set; } = string.Empty;

    public ModelsOptions Models { get; set; } = new ModelsOptions();
    
    public GenerationConfigOptions GenerationConfig { get; set; } = new GenerationConfigOptions();
}

public class ModelsOptions
{
    public string Fitness { get; set; } = "gemini-3.1-flash-lite-preview";
    public string Health { get; set; } = "gemini-3.1-flash-lite-preview";
    public string Analysis { get; set; } = "gemini-3.1-flash-lite-preview";
}

public class GenerationConfigOptions
{
    public float Temperature { get; set; } = 0.7f;
    public int MaxOutputTokens { get; set; } = 1000;
    public float TopP { get; set; } = 0.95f;
    public int TopK { get; set; } = 40;
}
