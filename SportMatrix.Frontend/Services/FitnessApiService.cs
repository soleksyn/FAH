using SportMatrix.Frontend.Models.ApiClient;
using System.Net.Http.Json;

namespace SportMatrix.Frontend.Services;

public class FitnessApiService
{
    private readonly HttpClient _httpClient;

    public FitnessApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ActivityStatisticsDto?> GetAthleteStatisticsAsync(int athleteId)
    {
        var response = await _httpClient.GetAsync($"/api/activity/statistics/{athleteId}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<ActivityStatisticsDto>();
    }

    public async Task<List<ActivityDto>> GetAthleteActivitiesAsync(int athleteId)
    {
        var response = await _httpClient.GetAsync($"/api/activity/athlete/{athleteId}");
        if (!response.IsSuccessStatusCode)
            return new List<ActivityDto>();
        var activities = await response.Content.ReadFromJsonAsync<List<ActivityDto>>()
            ?? new List<ActivityDto>();

        return activities
            .OrderByDescending(a => DateTime.Parse(a.StartDate))
            .ToList();
    }

    public async Task<List<ActivityDto>> GetRecentActivitiesAsync(int athleteId, int count = 10)
    {
        var activities = await GetAthleteActivitiesAsync(athleteId);
        return activities.Take(count).ToList();
    }

    public async Task<ActivityDto?> GetActivityByIdAsync(int activityId)
    {
        var response = await _httpClient.GetAsync($"/api/activity/{activityId}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<ActivityDto>();
    }

    
    
    
    public async Task<List<ActivityDto>> GetActivitiesByDateRangeAsync(int athleteId, DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToString("yyyy-MM-dd");
        var end = endDate.ToString("yyyy-MM-dd");
        var response = await _httpClient.GetAsync($"/api/activity/athlete/{athleteId}/range?start={start}&end={end}");
        if (!response.IsSuccessStatusCode)
            return new List<ActivityDto>();
        return await response.Content.ReadFromJsonAsync<List<ActivityDto>>()
            ?? new List<ActivityDto>();
    }
}
