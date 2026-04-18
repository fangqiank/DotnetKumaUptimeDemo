using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DotnetKumaUptimeDemo.Wpf.Converters;

public class HealthStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()?.ToLower() switch
        {
            "healthy" => Brushes.Green,
            "degraded" => Brushes.Orange,
            "unhealthy" => Brushes.Red,
            "error" => Brushes.DarkRed,
            _ => Brushes.Gray
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
