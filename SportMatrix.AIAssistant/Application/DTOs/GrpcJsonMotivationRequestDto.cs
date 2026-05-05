namespace SportMatrix.AIAssistant.Application.DTOs;

/// <summary>
/// Request DTO for gRPC-JSON MotivationService
/// (Mirrors the gRPC MotivationRequest as JSON)
/// </summary>
public class GrpcJsonMotivationRequestDto
{
    public GrpcJsonAthleteProfileDto? AthleteProfile { get; set; }

    public bool IsStruggling { get; set; } = false;

    public string? UpcomingWorkoutType { get; set; }
}
