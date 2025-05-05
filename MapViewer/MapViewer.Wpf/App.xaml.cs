using MapViewer.Core.Utils;
using MapViewer.Core.Models;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;
using MapViewer.Core.ViewModels;
using MapViewer.Wpf.Commands;
using MapViewer.Core.Commands;
using MapViewer.Core.Stores;
using MapViewer.Core.Services;
using CameraViewer.Core.Stores;

namespace MapViewer.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly NavigationStore _navigationStore;
    private readonly MapStore _mapStore;
    private readonly CameraStore _cameraStore;
    private readonly SettingsStore _settingsStore;

    public App()
    {
        _navigationStore = new();
        _mapStore = new();
        _cameraStore = new();
        _settingsStore = new(new Settings(Color.FromName("Black"), Color.FromName("White"), Color.FromName("Blue"), Color.FromArgb(0xff, 0xff, 0x0, 0x0), 3f, 5f, 100, 0.1f, 0.1f, 1f));
    }

    protected override void OnStartup(StartupEventArgs e) {


        // Setup main window
        MainWindow = new MainWindow()
        {
            DataContext = new MainViewModel(_navigationStore)
        };
        _navigationStore.CurrentViewModel = (WindowViewModel)MakeMapViewModel();
        MainWindow.Show();
        base.OnStartup(e);
    }

    private MapViewModel MakeMapViewModel()
    {
        return new(
            _mapStore,
            _cameraStore,
            _settingsStore,
            new NavigationService(_navigationStore, MakeSettingsViewModel),
            (vm) => new LoadMapCommand(vm),
            (vm) => new SaveMapCommand(vm),
            (vm) => new HandleMouseCommand(vm),
            (vm) => new HandleKeyCommand(vm)
            );
    }

    private SettingsViewModel MakeSettingsViewModel()
    {
        return new(
            _settingsStore,
            new NavigationService(_navigationStore, MakeMapViewModel)
            );
    }

}


