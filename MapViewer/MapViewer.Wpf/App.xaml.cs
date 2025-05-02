using MapViewer.Core.Utils;
using MapViewer.Core.Models;
using System.Configuration;
using System.Data;
using System.Windows;
using MapViewer.Core.ViewModels;
using MapViewer.Wpf.Commands;
using MapViewer.Core.Commands;

namespace MapViewer.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e) {

        // Setup main window
        MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(
                (vm) => new LoadMapCommand(vm),
                (vm) => new SaveMapCommand(vm),
                (vm) => new HandleMouseCommand(vm),
                (vm) => new HandleKeyCommand(vm))
        };
        MainWindow.Show();
        base.OnStartup(e);
    }
}


