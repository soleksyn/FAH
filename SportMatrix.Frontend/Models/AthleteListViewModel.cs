using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Models;

public class AthleteListViewModel
{
    public List<AthleteDto> Athletes { get; set; } = new();
    public string? Error { get; set; }
    public bool Loading { get; set; } = false;
}
