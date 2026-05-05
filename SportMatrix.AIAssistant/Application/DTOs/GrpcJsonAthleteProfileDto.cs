namespace SportMatrix.AIAssistant.Application.DTOs;

/// <summary>
/// AthleteProfile DTO for gRPC-JSON
/// (Mirrors the gRPC AthleteProfile as JSON)
/// </summary>
public class GrpcJsonAthleteProfileDto
{
    public string? Name { get; set; }

    public string? FitnessLevel { get; set; }

    public string? PrimaryGoal { get; set; }
}
