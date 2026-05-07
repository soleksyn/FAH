using SportMatrix.Frontend.Models.ApiClient;

namespace SportMatrix.Frontend.Models;

public class AthleteListViewModel
{
    public List<AthleteDto> Athletes { get; set; } = new();
    public string? Error { get; set; }
    public bool Loading { get; set; } = false;

    // Pagination
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalAthletes { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalAthletes / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
