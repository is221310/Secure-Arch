using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net;

public class ApiHelper
{
    private readonly HttpClient _httpClient;
    private readonly NavigationManager _navigation;

    public ApiHelper(HttpClient httpClient, NavigationManager navigation)
    {
        _httpClient = httpClient;
        _navigation = navigation;
    }

    // Generische Methode, die Daten lädt und bei 401 auf /login weiterleitet
    public async Task<(bool IsSuccess, T? Data)> GetAsync<T>(string url)
    {
        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _navigation.NavigateTo("/login");
            return (false, default);
        }

        if (!response.IsSuccessStatusCode)
        {
            return (false, default);
        }

        var data = await response.Content.ReadFromJsonAsync<T>();
        return (data != null, data);
    }
}