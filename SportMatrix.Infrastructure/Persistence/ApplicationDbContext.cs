using SportMatrix.Application.Interfaces;
using SportMatrix.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SportMatrix.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    public DbSet<Athlete> Athletes { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<TrainingPlan> TrainingPlans { get; set; }
    public DbSet<PlannedActivity> PlannedActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Seed data
        modelBuilder.Entity<Athlete>().HasData(
            new Athlete
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john.doe@example.com",
                City = "Kyiv",
                Country = "Ukraine",
                ProfilePictureUrl = "https://example.com/john.jpg",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Athlete
            {
                Id = 2,
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Email = "jane.smith@example.com",
                City = "Lviv",
                Country = "Ukraine",
                ProfilePictureUrl = "https://example.com/jane.jpg",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<Activity>().HasData(
            new Activity
            {
                Id = 1,
                AthleteId = 1,
                Name = "Morning Run",
                Description = "5km morning run in the park",
                Distance = 5000,
                MovingTime = 1800,
                ElapsedTime = 1800,
                TotalElevationGain = 50,
                SportType = "Run",
                StartDate = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                StartDateLocal = new DateTime(2026, 5, 3, 3, 0, 0, DateTimeKind.Utc),
                Timezone = "Europe/Kiev",
                CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                AverageSpeed = 2.78,
                MaxSpeed = 3.5,
                AverageHeartRate = 145,
                MaxHeartRate = 165,
                AverageCadence = 180
            },
            new Activity
            {
                Id = 2,
                AthleteId = 1,
                Name = "Cycling Training",
                Description = "20km cycling training",
                Distance = 20000,
                MovingTime = 3600,
                ElapsedTime = 3600,
                TotalElevationGain = 200,
                SportType = "Ride",
                StartDate = new DateTime(2026, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                StartDateLocal = new DateTime(2026, 5, 2, 3, 0, 0, DateTimeKind.Utc),
                Timezone = "Europe/Kiev",
                CreatedAt = new DateTime(2026, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 5, 2, 0, 0, 0, DateTimeKind.Utc),
                AverageSpeed = 5.56,
                MaxSpeed = 8.0,
                AverageHeartRate = 130,
                MaxHeartRate = 150,
                AverageCadence = 90
            },
            new Activity
            {
                Id = 3,
                AthleteId = 2,
                Name = "Swimming Session",
                Description = "1.5km swimming in the pool",
                Distance = 1500,
                MovingTime = 2400,
                ElapsedTime = 2400,
                TotalElevationGain = 0,
                SportType = "Swim",
                StartDate = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                StartDateLocal = new DateTime(2026, 5, 3, 3, 0, 0, DateTimeKind.Utc),
                Timezone = "Europe/Kiev",
                CreatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 5, 3, 0, 0, 0, DateTimeKind.Utc),
                AverageSpeed = 0.63,
                MaxSpeed = 0.8,
                AverageHeartRate = 140,
                MaxHeartRate = 160
            }
        );

        modelBuilder.Entity<TrainingPlan>().HasData(
            new TrainingPlan
            {
                Id = 1,
                AthleteId = 1,
                Name = "Marathon Preparation",
                Description = "12-week marathon training plan",
                StartDate = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc)
            },
            new TrainingPlan
            {
                Id = 2,
                AthleteId = 2,
                Name = "Triathlon Training",
                Description = "8-week triathlon preparation",
                StartDate = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2026, 6, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        modelBuilder.Entity<PlannedActivity>().HasData(
            new PlannedActivity
            {
                Id = 1,
                TrainingPlanId = 1,
                Title = "Long Run",
                Description = "15km long run",
                PlannedDate = new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc),
                PlannedDistance = 15000,
                PlannedDuration = 5400,
                SportType = "Run"
            },
            new PlannedActivity
            {
                Id = 2,
                TrainingPlanId = 1,
                Title = "Interval Training",
                Description = "5x1km intervals",
                PlannedDate = new DateTime(2026, 5, 9, 0, 0, 0, DateTimeKind.Utc),
                PlannedDistance = 8000,
                PlannedDuration = 2400,
                SportType = "Run"
            },
            new PlannedActivity
            {
                Id = 3,
                TrainingPlanId = 2,
                Title = "Brick Workout",
                Description = "Bike to run transition",
                PlannedDate = new DateTime(2026, 5, 8, 0, 0, 0, DateTimeKind.Utc),
                PlannedDistance = 50000,
                PlannedDuration = 7200,
                SportType = "Brick"
            }
        );
    }
}
