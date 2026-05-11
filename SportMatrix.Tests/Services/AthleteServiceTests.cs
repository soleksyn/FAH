namespace SportMatrix.Tests.Services;

using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SportMatrix.Application.DTOs;
using SportMatrix.Application.Mapping;
using SportMatrix.Application.Services;
using SportMatrix.Domain.Entities;
using SportMatrix.Domain.Exceptions.Athletes;
using SportMatrix.Infrastructure.Persistence;
using Xunit;

public class AthleteServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly AthleteService _athleteService;

    public AthleteServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        
        _athleteService = new AthleteService(_context, _mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetAthleteByIdAsync_WhenAthleteExists_ShouldReturnAthleteDto()
    {
        // Arrange
        var athlete = new Athlete
        {
            Id = 1,
            FirstName = "Stanislav",
            LastName = "Oleksyn",
            Email = "stanislav@test.com",
            CreatedAt = DateTime.UtcNow
        };
        await _context.Athletes.AddAsync(athlete);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _athleteService.GetAthleteByIdAsync(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("Stanislav");
        result.LastName.Should().Be("Oleksyn");
    }

    [Fact]
    public async Task GetAthleteByIdAsync_WhenAthleteDoesNotExist_ShouldThrowAthleteNotFoundException()
    {
        // Act
        var act = () => _athleteService.GetAthleteByIdAsync(99, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AthleteNotFoundException>()
            .Where(e => e.AthleteId == 99);
    }

    [Fact]
    public async Task CreateAthleteAsync_WithValidData_ShouldCreateAndReturnAthleteDto()
    {
        // Arrange
        var createDto = new CreateAthleteDto
        {
            FirstName = "New",
            LastName = "Athlete",
            Email = "new@test.com"
        };

        // Act
        var result = await _athleteService.CreateAthleteAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("New");
        result.Email.Should().Be("new@test.com");
        
        var athleteInDb = await _context.Athletes.FirstOrDefaultAsync(a => a.Email == "new@test.com");
        athleteInDb.Should().NotBeNull();
        athleteInDb!.FirstName.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAthleteAsync_WhenAthleteExists_ShouldUpdateAthlete()
    {
        // Arrange
        var athlete = new Athlete
        {
            Id = 1,
            FirstName = "Old",
            LastName = "Name",
            Email = "old@test.com"
        };
        await _context.Athletes.AddAsync(athlete);
        await _context.SaveChangesAsync(CancellationToken.None);

        var updateDto = new UpdateAthleteDto
        {
            Id = 1,
            FirstName = "Updated",
            LastName = "Name"
        };

        // Act
        await _athleteService.UpdateAthleteAsync(updateDto, CancellationToken.None);

        // Assert
        var updatedAthlete = await _context.Athletes.FindAsync(1);
        updatedAthlete!.FirstName.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAthleteAsync_WhenAthleteExists_ShouldRemoveAthlete()
    {
        // Arrange
        var athlete = new Athlete
        {
            Id = 1,
            FirstName = "To",
            LastName = "Delete",
            Email = "delete@test.com"
        };
        await _context.Athletes.AddAsync(athlete);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        await _athleteService.DeleteAthleteAsync(1, CancellationToken.None);

        // Assert
        var deletedAthlete = await _context.Athletes.FindAsync(1);
        deletedAthlete.Should().BeNull();
    }
}
