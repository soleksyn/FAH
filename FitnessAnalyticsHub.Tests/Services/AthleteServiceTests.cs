namespace FitnessAnalyticsHub.Tests.Services;

using AutoMapper;
using FitnessAnalyticsHub.Application.DTOs;
using FitnessAnalyticsHub.Application.Mapping;
using FitnessAnalyticsHub.Application.Services;
using FitnessAnalyticsHub.Domain.Entities;
using FitnessAnalyticsHub.Domain.Exceptions.Athletes;
using FitnessAnalyticsHub.Domain.Interfaces;
using FitnessAnalyticsHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

public class AthleteServiceTests
{
    private readonly ApplicationDbContext context;
            this.mapper);
    }

    public void Dispose()
    {
        this.context.Dispose();
    }

    [Fact]
    public async Task GetAthleteByIdAsync_ShouldReturnAthlete_WhenAthleteExists()
    {
        // Arrange
        Athlete athlete = new Athlete
        {
            Id = 1,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@test.com",
            City = "Berlin",
            Country = "Germany",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        await this.context.Athletes.AddAsync(athlete);
        await this.context.SaveChangesAsync();

        // Act
        AthleteDto result = await this.athleteService.GetAthleteByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Max", result.FirstName);
        Assert.Equal("Mustermann", result.LastName);
        Assert.Equal("max@test.com", result.Email);
        Assert.Equal("Berlin", result.City);
        Assert.Equal("Germany", result.Country);
    }

    [Fact]
    public async Task GetAthleteByIdAsync_ShouldThrowAthleteNotFoundException_WhenAthleteDoesNotExist()
    {
        // Arrange - Keine Daten in DB

        // Act & Assert
        AthleteNotFoundException exception = await Assert.ThrowsAsync<AthleteNotFoundException>(
            () => this.athleteService.GetAthleteByIdAsync(999, CancellationToken.None));

        Assert.Equal(999, exception.AthleteId);
    }

    [Fact]
    public async Task GetAllAthletesAsync_ShouldReturnAllAthletes()
    {
        // Arrange
        List<Athlete> athletes = new List<Athlete>
    {
        new Athlete
        {
            Id = 1,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@test.com",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
        new Athlete
        {
            Id = 2,
            FirstName = "Anna",
            LastName = "Schmidt",
            Email = "anna@test.com",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        },
    };

        await this.context.Athletes.AddRangeAsync(athletes);
        await this.context.SaveChangesAsync();

        // Act
        IEnumerable<AthleteDto> result = await this.athleteService.GetAllAthletesAsync(CancellationToken.None);

        // Assert
        List<AthleteDto> resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.Contains(resultList, a => a.FirstName == "Max" && a.LastName == "Mustermann");
        Assert.Contains(resultList, a => a.FirstName == "Anna" && a.LastName == "Schmidt");
    }

    [Fact]
    public async Task CreateAthleteAsync_ShouldCreateAthlete_WhenValidData()
    {
        // Arrange
        CreateAthleteDto createDto = new CreateAthleteDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@test.com",
            City = "Munich",
            Country = "Germany",
        };

        // Act
        AthleteDto result = await this.athleteService.CreateAthleteAsync(createDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal("test@test.com", result.Email);
        Assert.True(result.Id > 0);

        // Verify in database
        Athlete? athleteInDb = await this.context.Athletes.FindAsync(result.Id);
        Assert.NotNull(athleteInDb);
        Assert.Equal("Test", athleteInDb.FirstName);
    }

    [Fact]
    public async Task UpdateAthleteAsync_ShouldUpdateAthlete_WhenAthleteExists()
    {
        // Arrange
        Athlete athlete = new Athlete
        {
            Id = 1,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@test.com",
            City = "Berlin",
            Country = "Germany",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        await this.context.Athletes.AddAsync(athlete);
        await this.context.SaveChangesAsync();

        UpdateAthleteDto updateDto = new UpdateAthleteDto
        {
            Id = 1,
            FirstName = "Maximilian",
            LastName = "Mustermann",
            City = "Munich",
            Country = "Germany",
        };

        // Act
        await this.athleteService.UpdateAthleteAsync(updateDto, CancellationToken.None);

        // Assert
        Athlete? updatedAthlete = await this.context.Athletes.FindAsync(1);
        Assert.NotNull(updatedAthlete);
        Assert.Equal("Maximilian", updatedAthlete.FirstName);
        Assert.Equal("Munich", updatedAthlete.City);
    }

    [Fact]
    public async Task UpdateAthleteAsync_ShouldThrowAthleteNotFoundException_WhenAthleteDoesNotExist()
    {
        // Arrange
        UpdateAthleteDto updateDto = new UpdateAthleteDto
        {
            Id = 999,
            FirstName = "Test",
            LastName = "User",
        };

        // Act & Assert
        AthleteNotFoundException exception = await Assert.ThrowsAsync<AthleteNotFoundException>(
            () => this.athleteService.UpdateAthleteAsync(updateDto, CancellationToken.None));

        Assert.Equal(999, exception.AthleteId);
    }

    [Fact]
    public async Task DeleteAthleteAsync_ShouldDeleteAthlete_WhenAthleteExists()
    {
        // Arrange
        Athlete athlete = new Athlete
        {
            Id = 1,
            FirstName = "Max",
            LastName = "Mustermann",
            Email = "max@test.com",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        await this.context.Athletes.AddAsync(athlete);
        await this.context.SaveChangesAsync();

        // Act
        await this.athleteService.DeleteAthleteAsync(1, CancellationToken.None);

        // Assert
        Athlete? deletedAthlete = await this.context.Athletes.FindAsync(1);
        Assert.Null(deletedAthlete);
    }

    [Fact]
    public async Task DeleteAthleteAsync_ShouldThrowAthleteNotFoundException_WhenAthleteDoesNotExist()
    {
        // Arrange - Keine Daten in DB

        // Act & Assert
        AthleteNotFoundException exception = await Assert.ThrowsAsync<AthleteNotFoundException>(
            () => this.athleteService.DeleteAthleteAsync(999, CancellationToken.None));

        Assert.Equal(999, exception.AthleteId);
    }

    [Fact]
