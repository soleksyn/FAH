namespace SportMatrix.Application.Services;

using AutoMapper;
using SportMatrix.Application.DTOs;
using SportMatrix.Application.Interfaces;
using SportMatrix.Domain.Entities;
using SportMatrix.Domain.Exceptions.Athletes;
using Microsoft.EntityFrameworkCore;

public class AthleteService : IAthleteService
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public AthleteService(
        IApplicationDbContext context,
        IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<AthleteDto> GetAthleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        Athlete? athlete = await this.context.Athletes
            .FirstOrDefaultAsync(a => a.Id == id && a.Id != 0 && a.Email != "admin@sportmatrix.com", cancellationToken);

        if (athlete == null)
        {
            throw new AthleteNotFoundException(id);
        }

        return this.mapper.Map<AthleteDto>(athlete);
    }

    public async Task<IEnumerable<AthleteDto>> GetAllAthletesAsync(CancellationToken cancellationToken)
    {
        List<Athlete> athletes = await this.context.Athletes
            .Where(a => a.Email != "admin@sportmatrix.com" && a.Id != 0)
            .ToListAsync(cancellationToken);
        return this.mapper.Map<IEnumerable<AthleteDto>>(athletes);
    }

    public async Task<AthleteDto> CreateAthleteAsync(CreateAthleteDto athleteDto, CancellationToken cancellationToken)
    {
        Athlete athlete = this.mapper.Map<Athlete>(athleteDto);
        await this.context.Athletes.AddAsync(athlete, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);
        return this.mapper.Map<AthleteDto>(athlete);
    }

    public async Task UpdateAthleteAsync(UpdateAthleteDto athleteDto, CancellationToken cancellationToken)
    {
        Athlete? athlete = await this.context.Athletes.FirstOrDefaultAsync(a => a.Id == athleteDto.Id, cancellationToken);

        if (athlete == null)
        {
            throw new AthleteNotFoundException(athleteDto.Id);
        }

        this.mapper.Map(athleteDto, athlete);
        athlete.UpdatedAt = DateTime.Now;

        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AthleteDto?> GetAthleteByEmailAsync(string email, CancellationToken cancellationToken)
    {
        Athlete? athlete = await this.context.Athletes
            .FirstOrDefaultAsync(a => a.Email == email && a.Email != "admin@sportmatrix.com" && a.Id != 0, cancellationToken);
        return this.mapper.Map<AthleteDto>(athlete);
    }

    public async Task DeleteAthleteAsync(int id, CancellationToken cancellationToken)
    {
        Athlete? athlete = await this.context.Athletes.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (athlete == null)
        {
            throw new AthleteNotFoundException(id);
        }

        this.context.Athletes.Remove(athlete);
        await this.context.SaveChangesAsync(cancellationToken);
    }

}
