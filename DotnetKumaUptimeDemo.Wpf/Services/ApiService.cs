using System.Net.Http;
using System.Net.Http.Json;
using DotnetKumaUptimeDemo.Wpf.Models;

namespace DotnetKumaUptimeDemo.Wpf.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(string baseUrl = "http://localhost:5000")
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    public async Task<HealthCheckResponse?> GetHealthAsync()
    {
        var resp = await _http.GetAsync("/health");
        // 503 (Unhealthy) still has JSON body we need to parse
        if (resp.StatusCode != System.Net.HttpStatusCode.OK
            && resp.StatusCode != System.Net.HttpStatusCode.ServiceUnavailable)
            return null;
        return await resp.Content.ReadFromJsonAsync<HealthCheckResponse>();
    }

    public async Task<ServiceInfo?> GetStatusAsync()
    {
        var resp = await _http.GetAsync("/api/status");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<ServiceInfo>();
    }

    public async Task<List<UserInfo>?> GetUsersAsync()
    {
        var resp = await _http.GetAsync("/api/users");
        if (!resp.IsSuccessStatusCode) return null;
        return await resp.Content.ReadFromJsonAsync<List<UserInfo>>();
    }

    public async Task<bool> SimulateFailureAsync(string component)
    {
        var resp = await _http.PostAsync($"/api/simulate/failure?component={component}", null);
        return resp.IsSuccessStatusCode;
    }

    public async Task<bool> SimulateRecoveryAsync(string component)
    {
        var resp = await _http.PostAsync($"/api/simulate/recovery?component={component}", null);
        return resp.IsSuccessStatusCode;
    }
}
