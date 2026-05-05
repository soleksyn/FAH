namespace SportMatrix.AIAssistant.Application.Interfaces;

using SportMatrix.AIAssistant.Application.DTOs;

public interface IMotivationCoachService
{
    Task<MotivationResponseDto> GenerateMotivationAsync(
        MotivationRequestDto request, CancellationToken cancellationToken);
}
