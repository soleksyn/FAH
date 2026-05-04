namespace SportMatrix.AIAssistant.Tests.Extensions;

using SportMatrix.AIAssistant.Application.DTOs;
using SportMatrix.AIAssistant.Extensions;
using SportMatrix;
using Xunit;

public class GrpcMappingExtensionsTests
{
    [Fact]
    public void ToAIAssistantAthleteProfile_WithValidProfile_MapsCorrectly()
    {
        // Arrange
        Sportmatrix.AthleteProfile grpcProfile = new Sportmatrix.AthleteProfile
        {
            Name = "John Doe",
            FitnessLevel = "Intermediate",
            PrimaryGoal = "Weight Loss",
        };

        // Act
        Domain.Models.AthleteProfile result = grpcProfile.ToAIAssistantAthleteProfile();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.Equal("Intermediate", result.FitnessLevel);
        Assert.Equal("Weight Loss", result.PrimaryGoal);
        Assert.NotNull(result.Id);
        Assert.NotEmpty(result.Id);
        Assert.NotNull(result.Preferences);
    }

    [Fact]
    public void ToAIAssistantAthleteProfile_WithNullProfile_ReturnsDefaults()
    {
        // Arrange
        Sportmatrix.AthleteProfile grpcProfile = null;

        // Act
        Domain.Models.AthleteProfile result = grpcProfile.ToAIAssistantAthleteProfile();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Name);
        Assert.Equal(string.Empty, result.FitnessLevel);
        Assert.Equal(string.Empty, result.PrimaryGoal);
        Assert.NotNull(result.Id);
        Assert.NotNull(result.Preferences);
    }

    [Fact]
    public void ToMotivationRequestDto_WithValidRequest_MapsCorrectly()
    {
        // Arrange
        Sportmatrix.MotivationRequest grpcRequest = new Sportmatrix.MotivationRequest
        {
            AthleteProfile = new Sportmatrix.AthleteProfile
            {
                Name = "Alice",
                FitnessLevel = "Beginner",
                PrimaryGoal = "Muscle Building",
            },
        };

        // Act
        Application.DTOs.MotivationRequestDto result = grpcRequest.ToMotivationRequestDto();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AthleteProfile);
        Assert.Equal("Alice", result.AthleteProfile.Name);
        Assert.Equal("Beginner", result.AthleteProfile.FitnessLevel);
        Assert.Equal("Muscle Building", result.AthleteProfile.PrimaryGoal);
        Assert.Null(result.LastWorkout);
        Assert.Null(result.UpcomingWorkoutType);
        Assert.False(result.IsStruggling);
    }

    [Fact]
    public void ToWorkoutAnalysisRequestDto_WithValidRequest_MapsCorrectly()
    {
        // Arrange
        Sportmatrix.WorkoutAnalysisRequest grpcRequest = new Sportmatrix.WorkoutAnalysisRequest
        {
            AnalysisType = "Performance",
            AthleteProfile = new Sportmatrix.AthleteProfile
            {
                Name = "Bob",
                FitnessLevel = "Advanced",
            },
        };
        grpcRequest.RecentWorkouts.Add(new Sportmatrix.Workout
        {
            Date = "2025-01-15",
            ActivityType = "Run",
            Distance = 5000,
            Duration = 1800,
            Calories = 300,
        });

        // Act
        WorkoutAnalysisRequestDto result = grpcRequest.ToWorkoutAnalysisRequestDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Performance", result.AnalysisType);
        Assert.NotNull(result.AthleteProfile);
        Assert.Equal("Bob", result.AthleteProfile.Name);
        Assert.NotNull(result.RecentWorkouts);
        Assert.Single(result.RecentWorkouts);
        Assert.NotNull(result.AdditionalContext);
    }

    [Fact]
    public void ToWorkoutAnalysisRequestDto_WithNullAthleteProfile_CreatesDefault()
    {
        // Arrange
        Sportmatrix.WorkoutAnalysisRequest grpcRequest = new Sportmatrix.WorkoutAnalysisRequest
        {
            AnalysisType = "Health",
            AthleteProfile = null,
        };

        // Act
        WorkoutAnalysisRequestDto result = grpcRequest.ToWorkoutAnalysisRequestDto();

        // Assert
        Assert.NotNull(result.AthleteProfile);
        Assert.NotNull(result.RecentWorkouts);
    }

    [Fact]
    public void ToAIAssistantWorkoutData_WithValidWorkout_MapsCorrectly()
    {
        // Arrange
        Sportmatrix.Workout grpcWorkout = new Sportmatrix.Workout
        {
            Date = "2025-01-15",
            ActivityType = "Cycling",
            Distance = 15000,
            Duration = 3600,
            Calories = 500,
        };

        // Act
        Domain.Models.WorkoutData result = grpcWorkout.ToAIAssistantWorkoutData();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(DateTime.Parse("2025-01-15"), result.Date);
        Assert.Equal("Cycling", result.ActivityType);
        Assert.Equal(15000, result.Distance);
        Assert.Equal(3600, result.Duration);
        Assert.Equal(500, result.Calories);
    }

    [Theory]
    [InlineData("2025-01-01")]
    [InlineData("2024-12-31")]
    [InlineData("2025-07-15")]
    public void ToAIAssistantWorkoutData_WithDifferentDates_ParsesCorrectly(string dateString)
    {
        // Arrange
        Sportmatrix.Workout grpcWorkout = new Sportmatrix.Workout
        {
            Date = dateString,
            ActivityType = "Test",
            Distance = 1000,
            Duration = 600,
            Calories = 100,
        };

        // Act
        Domain.Models.WorkoutData result = grpcWorkout.ToAIAssistantWorkoutData();

        // Assert
        Assert.Equal(DateTime.Parse(dateString), result.Date);
    }

    [Fact]
    public void ToWorkoutAnalysisRequestDto_WithMultipleWorkouts_MapsAll()
    {
        // Arrange
        Sportmatrix.WorkoutAnalysisRequest grpcRequest = new Sportmatrix.WorkoutAnalysisRequest
        {
            AnalysisType = "Trends",
            AthleteProfile = new Sportmatrix.AthleteProfile(),
        };

        grpcRequest.RecentWorkouts.Add(new Sportmatrix.Workout { Date = "2025-01-01", ActivityType = "Run" });
        grpcRequest.RecentWorkouts.Add(new Sportmatrix.Workout { Date = "2025-01-02", ActivityType = "Bike" });

        // Act
        WorkoutAnalysisRequestDto result = grpcRequest.ToWorkoutAnalysisRequestDto();

        // Assert
        Assert.Equal(2, result.RecentWorkouts.Count);
        Assert.Equal("Run", result.RecentWorkouts[0].ActivityType);
        Assert.Equal("Bike", result.RecentWorkouts[1].ActivityType);
    }
}