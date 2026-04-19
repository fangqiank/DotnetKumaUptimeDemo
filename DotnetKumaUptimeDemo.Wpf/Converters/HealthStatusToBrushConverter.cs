using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DotnetKumaUptimeDemo.Wpf.Converters;

public class HealthStatusToBrushConverter : IValueConverter
{
    private static readonly SolidColorBrush Healthy = new(Color.FromRgb(0x1f, 0x8a, 0x65));
    private static readonly SolidColorBrush Degraded = new(Color.FromRgb(0xc0, 0x85, 0x32));
    private static readonly SolidColorBrush Unhealthy = new(Color.FromRgb(0xcf, 0x2d, 0x56));
    private static readonly SolidColorBrush Error = new(Color.FromRgb(0xcf, 0x2d, 0x56));
    private static readonly SolidColorBrush Unknown = new(Color.FromRgb(0xa1, 0xa0, 0x9b));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()?.ToLower() switch
        {
            "healthy" => Healthy,
            "degraded" => Degraded,
            "unhealthy" => Unhealthy,
            "error" => Error,
            _ => Unknown
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class HealthStatusToForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Dark text on light badges
        return value?.ToString()?.ToLower() switch
        {
            "healthy" => new SolidColorBrush(Color.FromRgb(0x1f, 0x8a, 0x65)),
            "degraded" => new SolidColorBrush(Color.FromRgb(0xc0, 0x85, 0x32)),
            "unhealthy" => new SolidColorBrush(Color.FromRgb(0xcf, 0x2d, 0x56)),
            _ => new SolidColorBrush(Color.FromRgb(0xa1, 0xa0, 0x9b))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
