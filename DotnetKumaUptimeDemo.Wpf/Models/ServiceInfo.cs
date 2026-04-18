using System.Text.Json.Serialization;

namespace DotnetKumaUptimeDemo.Wpf.Models;

public class ServiceInfo
{
    [JsonPropertyName("service")]
    public string Service { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}
