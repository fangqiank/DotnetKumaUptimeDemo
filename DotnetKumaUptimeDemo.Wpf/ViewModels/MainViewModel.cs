using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DotnetKumaUptimeDemo.Wpf.Models;
using DotnetKumaUptimeDemo.Wpf.Services;

namespace DotnetKumaUptimeDemo.Wpf.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly DispatcherTimer _timer;

    [ObservableProperty]
    private string _overallStatus = "Loading...";

    [ObservableProperty]
    private double _totalDuration;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _serviceInfoText = "";

    [ObservableProperty]
    private string _usersText = "";

    [ObservableProperty]
    private string _errorMessage = "";

    public ObservableCollection<ComponentCheck> Checks { get; } = [];

    public MainViewModel()
    {
        _api = new ApiService(App.Configuration);
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
        _timer.Tick += async (_, _) =>
        {
            try { await RefreshAsync(); }
            catch (Exception ex) { ErrorMessage = ex.Message; }
        };
    }

    public void StartAutoRefresh()
    {
        _timer.Start();
        RefreshAsync().ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                OverallStatus = "Error";
                ErrorMessage = t.Exception.InnerException?.Message ?? "Unknown error";
            }
        });
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        if (IsLoading) return;
        IsLoading = true;
        ErrorMessage = "";

        try
        {
            await RefreshHealthAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Health check failed: {ex.Message}";
            OverallStatus = "Error";
        }

        try
        {
            await Task.WhenAll(RefreshStatusAsync(), RefreshUsersAsync());
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Data refresh failed: {ex.Message}";
        }

        IsLoading = false;
    }

    private async Task RefreshHealthAsync()
    {
        var health = await _api.GetHealthAsync();
        if (health == null) return;

        OverallStatus = health.Status;
        TotalDuration = health.TotalDuration;

        Checks.Clear();
        foreach (var check in health.Checks)
            Checks.Add(check);
    }

    private async Task RefreshStatusAsync()
    {
        var status = await _api.GetStatusAsync();
        if (status == null) return;

        ServiceInfoText = $"{status.Service} v{status.Version}\nTimestamp: {status.Timestamp:yyyy-MM-dd HH:mm:ss}";
    }

    private async Task RefreshUsersAsync()
    {
        var users = await _api.GetUsersAsync();
        if (users == null)
        {
            UsersText = "(API unavailable - 503)";
            return;
        }

        UsersText = users.Count == 0
            ? "(no users)"
            : string.Join("\n", users.Select(u => $"[{u.Id}] {u.Name}"));
    }

    [RelayCommand]
    private async Task SimulateFailureAsync(string component)
    {
        await _api.SimulateFailureAsync(component);
        IsLoading = false;
        await RefreshAsync();
    }

    [RelayCommand]
    private async Task SimulateRecoveryAsync(string component)
    {
        await _api.SimulateRecoveryAsync(component);
        IsLoading = false;
        await RefreshAsync();
    }
}
