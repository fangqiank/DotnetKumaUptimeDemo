using System.Windows;
using DotnetKumaUptimeDemo.Wpf.ViewModels;

namespace DotnetKumaUptimeDemo.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
            vm.StartAutoRefresh();
    }
}
