namespace SportMatrix.Application.Services;

using AutoMapper;
using SportMatrix.Application.DTOs;
using SportMatrix.Application.Interfaces;
using SportMatrix.Domain.Entities;
using SportMatrix.Domain.Exceptions.Activities;
using SportMatrix.Domain.Exceptions.Athletes;
using Microsoft.EntityFrameworkCore;

public class ActivityService : IActivityService
{
    private readonly IApplicationDbContext context;
    private readonly IMapper mapper;

    public ActivityService(
    IApplicationDbContext context,
    IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    public async Task<ActivityDto> GetActivityByIdAsync(int id, CancellationToken cancellationToken)
    {
        Activity? activity = await this.context.Activities
            .Include(a => a.Athlete)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (activity == null)
        {
            throw new ActivityNotFoundException(id);
        }

        ActivityDto activityDto = this.mapper.Map<ActivityDto>(activity);

        return activityDto;
    }

    public async Task<IEnumerable<ActivityDto>> GetActivitiesByAthleteIdAsync(int athleteId, CancellationToken cancellationToken)
    {
        List<Activity> activities = await this.context.Activities
            .Include(a => a.Athlete)
            .Where(a => a.AthleteId == athleteId)
            .ToListAsync(cancellationToken);

        IEnumerable<ActivityDto> activityDtos = this.mapper.Map<IEnumerable<ActivityDto>>(activities);
        return activityDtos;
    }

    public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto activityDto, CancellationToken cancellationToken)
    {
        Activity activity = this.mapper.Map<Activity>(activityDto);

        await this.context.Activities.AddAsync(activity, cancellationToken);
        await this.context.SaveChangesAsync(cancellationToken);

        // Load activity with Athlete for mapping
        Activity activityWithAthlete = await this.context.Activities
            .Include(a => a.Athlete)
            .FirstAsync(a => a.Id == activity.Id, cancellationToken);

        ActivityDto resultDto = this.mapper.Map<ActivityDto>(activityWithAthlete);
        return resultDto;
    }

    public async Task UpdateActivityAsync(UpdateActivityDto activityDto, CancellationToken cancellationToken)
    {
        Activity? activity = await this.context.Activities.FirstOrDefaultAsync(a => a.Id == activityDto.Id, cancellationToken);

        if (activity == null)
        {
            throw new ActivityNotFoundException(activityDto.Id);
        }

        this.mapper.Map(activityDto, activity);
        activity.UpdatedAt = DateTime.Now;

        // EF Core tracks changes automatically - no need to call Update()
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteActivityAsync(int id, CancellationToken cancellationToken)
    {
        Activity? activity = await this.context.Activities.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (activity == null)
        {
            throw new ActivityNotFoundException(id);
        }

        this.context.Activities.Remove(activity);
        await this.context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ActivityStatisticsDto> GetAthleteActivityStatisticsAsync(int athleteId, CancellationToken cancellationToken)
    {
        // Verify athlete exists
        bool athleteExists = await this.context.Athletes.AnyAsync(a => a.Id == athleteId, cancellationToken);

        if (!athleteExists)
        {
            throw new AthleteNotFoundException(athleteId);
        }

        List<Activity> activities = await this.context.Activities
            .Where(a => a.AthleteId == athleteId)
            .ToListAsync(cancellationToken);

        if (!activities.Any())
        {
            return CreateEmptyStatistics();
        }

        return new ActivityStatisticsDto
        {
            TotalActivities = activities.Count,
            TotalDistance = activities.Sum(a => a.Distance) / 1000.0, // Convert meters to kilometers
            TotalDuration = TimeSpan.FromSeconds(activities.Sum(a => a.MovingTime)),
            TotalElevationGain = activities.Sum(a => a.TotalElevationGain),
            ActivitiesByType = activities
            .GroupBy(a => a.SportType)
            .ToDictionary(g => g.Key, g => g.Count()),
            ActivitiesByMonth = activities
            .GroupBy(a => a.StartDateLocal.Month)
            .ToDictionary(g => g.Key, g => g.Count()),
            AverageDistance = activities.Any() ? activities.Average(a => a.Distance) / 1000.0 : (double?)null, // Convert meters to kilometers
            LongestDistance = activities.Any() ? activities.Max(a => a.Distance) / 1000.0 : (double?)null, // Convert meters to kilometers
            MostCommonSport = activities
                .GroupBy(a => a.SportType)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault()
        };
    }

    private static ActivityStatisticsDto CreateEmptyStatistics()
    {
        return new ActivityStatisticsDto
        {
            TotalActivities = 0,
            TotalDistance = 0,
            TotalDuration = TimeSpan.Zero,
            TotalElevationGain = 0,
            ActivitiesByType = new Dictionary<string, int>(),
            ActivitiesByMonth = new Dictionary<int, int>(),
        };
    }
}
