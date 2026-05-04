using FitnessAnalyticsHub.Application.DTOs;
using FitnessAnalyticsHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitnessAnalyticsHub.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AthleteController : ControllerBase
{
    private readonly IAthleteService athleteService;

    public AthleteController(IAthleteService athleteService)
    {
        this.athleteService = athleteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AthleteDto>>> GetAll(CancellationToken cancellationToken)
    {
        IEnumerable<AthleteDto> athletes = await this.athleteService.GetAllAthletesAsync(cancellationToken);
        return this.Ok(athletes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AthleteDto>> GetById(int id, CancellationToken cancellationToken)
    {
        AthleteDto? athlete = await this.athleteService.GetAthleteByIdAsync(id, cancellationToken);
        return this.Ok(athlete);
    }

    [HttpPost]
    public async Task<ActionResult<AthleteDto>> Create(CreateAthleteDto createAthleteDto, CancellationToken cancellationToken)
    {
        AthleteDto athlete = await this.athleteService.CreateAthleteAsync(createAthleteDto, cancellationToken);
        return this.CreatedAtAction(nameof(this.GetById), new { id = athlete.Id }, athlete);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateAthleteDto updateAthleteDto, CancellationToken cancellationToken)
    {
        if (id != updateAthleteDto.Id)
        {
            return this.BadRequest("ID in URL does not match ID in body.");
        }

        await this.athleteService.UpdateAthleteAsync(updateAthleteDto, cancellationToken);
        return this.NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        {
            await this.athleteService.DeleteAthleteAsync(id, cancellationToken);
            return this.NoContent();
        }
    }
}
