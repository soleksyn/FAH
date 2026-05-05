using SportMatrix.AIAssistant.Domain.Models;
using SportMatrix.AIAssistant.Application.DTOs;

namespace SportMatrix.AIAssistant.Extensions;

public static class AthleteProfileExtensions
{
    // Domain to DTO
    public static AthleteProfileDto ToDto(this AthleteProfile domain)
    {
        return new AthleteProfileDto
        {
            Id = domain.Id,
            Name = domain.Name,
            FitnessLevel = domain.FitnessLevel,
            PrimaryGoal = domain.PrimaryGoal,
            Preferences = domain.Preferences,
        };
    }

    // DTO to Domain
    public static AthleteProfile ToDomain(this AthleteProfileDto dto)
    {
        return new AthleteProfile
        {
            Id = dto.Id ?? Guid.NewGuid().ToString(),
            Name = dto.Name,
            FitnessLevel = dto.FitnessLevel,
            PrimaryGoal = dto.PrimaryGoal,
            Preferences = dto.Preferences,
        };
    }

    // GrpcJson to DTO
    public static AthleteProfileDto ToAthleteProfileDto(this GrpcJsonAthleteProfileDto grpcProfile)
    {
        return new AthleteProfileDto
        {
            Id = Guid.NewGuid().ToString(),
            Name = grpcProfile.Name ?? string.Empty,
            FitnessLevel = grpcProfile.FitnessLevel,
            PrimaryGoal = grpcProfile.PrimaryGoal,
            Preferences = null,
        };
    }

    public static AthleteProfileDto ToAthleteProfileDto(
    this global::Sportmatrix.AthleteProfile grpcProfile)
    {
        return new AthleteProfileDto
        {
            Id = Guid.NewGuid().ToString(),
            Name = grpcProfile.Name ?? string.Empty,
            FitnessLevel = grpcProfile.FitnessLevel ?? string.Empty,
            PrimaryGoal = grpcProfile.PrimaryGoal ?? string.Empty,
            Preferences = null,
        };
    }
}
