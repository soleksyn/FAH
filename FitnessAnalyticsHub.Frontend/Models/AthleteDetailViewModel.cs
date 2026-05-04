using FitnessAnalyticsHub.Frontend.Models.ApiClient;

namespace FitnessAnalyticsHub.Frontend.Models;

public class AthleteDetailViewModel
{
    public AthleteDto? Athlete { get; set; }
    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
}
