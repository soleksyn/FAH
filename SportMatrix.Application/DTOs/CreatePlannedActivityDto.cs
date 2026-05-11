using SportMatrix.Domain.Enums;

namespace SportMatrix.Application;

public class CreatePlannedActivityDto
{
    public int TrainingPlanId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ActivityType ActivityType { get; set; }

    public DateTime PlannedDate { get; set; }

    public int? PlannedDurationMinutes { get; set; }

    public double? PlannedDistance { get; set; }
}
