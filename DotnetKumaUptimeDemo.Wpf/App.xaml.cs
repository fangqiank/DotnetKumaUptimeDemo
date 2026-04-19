using System.Windows;
using Microsoft.Extensions.Configuration;

namespace DotnetKumaUptimeDemo.Wpf;

public partial class App : Application
{
    public static IConfiguration Configuration { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        base.OnStartup(e);
    }
}
