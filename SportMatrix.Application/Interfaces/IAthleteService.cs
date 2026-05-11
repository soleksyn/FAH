namespace SportMatrix.Application.Interfaces;

using SportMatrix.Application.DTOs;

public interface IAthleteService
{
    Task<AthleteDto?> GetAthleteByIdAsync(int id, CancellationToken cancellationToken);

    Task<IEnumerable<AthleteDto>> GetAllAthletesAsync(CancellationToken cancellationToken);

    Task<AthleteDto> CreateAthleteAsync(CreateAthleteDto athleteDto, CancellationToken cancellationToken);

    Task UpdateAthleteAsync(UpdateAthleteDto athleteDto, CancellationToken cancellationToken);

    Task<AthleteDto?> GetAthleteByEmailAsync(string email, CancellationToken cancellationToken);

    Task DeleteAthleteAsync(int id, CancellationToken cancellationToken);
}
