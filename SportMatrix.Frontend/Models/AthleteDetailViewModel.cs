using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Models;

public class AthleteDetailViewModel
{
    public AthleteDto? Athlete { get; set; }
    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
}
