using System.ComponentModel.DataAnnotations;

namespace SportMatrix.AIAssistant.Application.DTOs;

/// <summary>
/// Request DTO für Training Recommendations
/// </summary>
public class GrpcJsonTrainingRecommendationsRequestDto
{
    [Required]
    public int AthleteId { get; set; }
}
