namespace SportMatrix.AIAssistant.Application.Interfaces;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Application.DTOs;

public interface IMotivationCoachService
{
    Task<MotivationResponseDto> GetHuggingFaceMotivationalMessageAsync(
        MotivationRequestDto request, CancellationToken cancellationToken);
}
