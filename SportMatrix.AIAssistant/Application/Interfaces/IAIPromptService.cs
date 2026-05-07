namespace SportMatrix.AIAssistant.Application.Interfaces;

using Google.GenAI.Types;

public interface IAIPromptService
{
    Task<string> GetStructuredAnalysisAsync(string prompt, string systemInstruction, Schema responseSchema, CancellationToken cancellationToken);
}
