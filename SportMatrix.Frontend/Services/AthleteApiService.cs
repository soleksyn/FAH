using SportMatrix.Frontend.Models.ApiClient;
using System.Net.Http.Json;

namespace SportMatrix.Frontend.Services;

public class AthleteApiService
{
    private readonly HttpClient _httpClient;

    public AthleteApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AthleteDto>> GetAllAthletesAsync()
    {
        var response = await _httpClient.GetAsync("/api/athlete");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<AthleteDto>>()
            ?? new List<AthleteDto>();
    }

    public async Task<AthleteDto?> GetAthleteByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"/api/athlete/{id}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<AthleteDto>();
    }

    public async Task<AthleteDto?> GetAthleteByEmailAsync(string email)
    {
        var response = await _httpClient.GetAsync($"/api/athlete/email/{email}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<AthleteDto>();
    }

    public async Task<AthleteDto?> CreateAthleteAsync(CreateAthleteRequest athlete)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/athlete", athlete);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AthleteDto>();
    }

    public async Task UpdateAthleteAsync(UpdateAthleteRequest athlete)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/athlete/{athlete.Id}", athlete);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAthleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/athlete/{id}");
        response.EnsureSuccessStatusCode();
    }
}
