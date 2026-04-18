using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DotnetKumaUptimeDemo.Wpf.Models;

public class HealthCheckResponse
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Unhealthy";

    [JsonPropertyName("checks")]
    public List<ComponentCheck> Checks { get; set; } = [];

    [JsonPropertyName("totalDuration")]
    public double TotalDuration { get; set; }
}

public class ComponentCheck
{
    [JsonPropertyName("component")]
    public string Component { get; set; } = "";

    [JsonPropertyName("status")]
    public string Status { get; set; } = "Unhealthy";

    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    [JsonPropertyName("duration")]
    public double Duration { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("data")]
    public Dictionary<string, object> Data { get; set; } = [];
}
