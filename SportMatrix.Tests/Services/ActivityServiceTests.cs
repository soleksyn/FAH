namespace SportMatrix.Tests.Services;

using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SportMatrix.Application.DTOs;
using SportMatrix.Application.Mapping;
using SportMatrix.Application.Services;
using SportMatrix.Domain.Entities;
using SportMatrix.Domain.Enums;
using SportMatrix.Domain.Exceptions.Activities;
using SportMatrix.Infrastructure.Persistence;
using Xunit;

public class ActivityServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ActivityService _activityService;

    public ActivityServiceTests()
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

        _activityService = new ActivityService(_context, _mapper);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetActivityByIdAsync_WhenActivityExists_ShouldReturnActivityDto()
    {
        // Arrange
        var athlete = new Athlete { Id = 1, FirstName = "Test", LastName = "User", Email = "test@user.com" };
        var activity = new Activity
        {
            Id = 1,
            AthleteId = 1,
            Name = "Morning Run",
            Distance = 5000,
            MovingTime = 1800,
            ActivityType = ActivityType.Run,
            StartDate = DateTime.UtcNow,
            Athlete = athlete
        };
        await _context.Athletes.AddAsync(athlete);
        await _context.Activities.AddAsync(activity);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _activityService.GetActivityByIdAsync(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Morning Run");
        result.AthleteFullName.Should().Be("Test User");
    }

    [Fact]
    public async Task GetActivityByIdAsync_WhenActivityDoesNotExist_ShouldThrowActivityNotFoundException()
    {
        // Act
        var act = () => _activityService.GetActivityByIdAsync(999, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ActivityNotFoundException>();
    }

    [Fact]
    public async Task GetAthleteActivityStatisticsAsync_WhenActivitiesExist_ShouldCalculateCorrectly()
    {
        // Arrange
        var athleteId = 1;
        var athlete = new Athlete { Id = athleteId, FirstName = "Stats", LastName = "User", Email = "stats@user.com" };
        var activities = new List<Activity>
        {
            new Activity { Id = 1, AthleteId = athleteId, Distance = 10000, MovingTime = 3600, ActivityType = ActivityType.Run, StartDateLocal = DateTime.UtcNow },
            new Activity { Id = 2, AthleteId = athleteId, Distance = 20000, MovingTime = 7200, ActivityType = ActivityType.Ride, StartDateLocal = DateTime.UtcNow }
        };
        await _context.Athletes.AddAsync(athlete);
        await _context.Activities.AddRangeAsync(activities);
        await _context.SaveChangesAsync(CancellationToken.None);

        // Act
        var stats = await _activityService.GetAthleteActivityStatisticsAsync(athleteId, CancellationToken.None);

        // Assert
        stats.TotalActivities.Should().Be(2);
        stats.TotalDistance.Should().Be(30.0); // 10000 + 20000 = 30000m = 30km
        stats.ActivitiesByType.Should().ContainKey(ActivityType.Run).WhoseValue.Should().Be(1);
        stats.ActivitiesByType.Should().ContainKey(ActivityType.Ride).WhoseValue.Should().Be(1);
    }

    [Fact]
    public async Task CreateActivityAsync_ShouldCreateActivityAndReturnDto()
    {
        // Arrange
        var athlete = new Athlete { Id = 1, FirstName = "Creator", LastName = "User", Email = "creator@user.com" };
        await _context.Athletes.AddAsync(athlete);
        await _context.SaveChangesAsync();

        var createDto = new CreateActivityDto
        {
            AthleteId = 1,
            Name = "New Activity",
            Distance = 1500,
            MovingTimeSeconds = 600,
            ActivityType = ActivityType.Swim
        };

        // Act
        var result = await _activityService.CreateActivityAsync(createDto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Activity");
        
        var inDb = await _context.Activities.FirstOrDefaultAsync(a => a.Name == "New Activity");
        inDb.Should().NotBeNull();
    }
}
