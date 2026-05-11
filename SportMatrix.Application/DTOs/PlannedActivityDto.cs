using SportMatrix.Domain.Enums;

namespace SportMatrix.Application.DTOs;

public class PlannedActivityDto
{
    public int Id { get; set; }

    public int TrainingPlanId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ActivityType ActivityType { get; set; }

    public DateTime PlannedDate { get; set; }

    public TimeSpan? PlannedDuration { get; set; }

    public double? PlannedDistance { get; set; }

    public ActivityDto? CompletedActivity { get; set; }

    public bool IsCompleted => this.CompletedActivity != null;
}
