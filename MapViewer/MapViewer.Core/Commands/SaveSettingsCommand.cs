using MapViewer.Core.Services;
using MapViewer.Core.Stores;
using MapViewer.Core.Models;
using MapViewer.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MapViewer.Core.Commands
{
    public class SaveSettingsCommand : BaseCommand
    {
        private readonly NavigationService _navigationService;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly SettingsStore _settingsStore;

        public SaveSettingsCommand(NavigationService navigationService, SettingsViewModel settingsViewModel, SettingsStore settingsStore)
        {
            _navigationService = navigationService;
            _settingsViewModel = settingsViewModel;
            _settingsStore = settingsStore;
            _settingsViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
           return _settingsViewModel.CanSave && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            _settingsViewModel.IsSaving = true;
            _settingsStore.Settings = new Settings(
                _settingsViewModel.MinAltitudeColor,
                _settingsViewModel.MaxAltitudeColor,
                _settingsViewModel.CircleColor,
                _settingsViewModel.CenterColor,
                _settingsViewModel.LineWidth,
                _settingsViewModel.CenterSize,
                _settingsViewModel.SegmentCount,
                _settingsViewModel.ZoomSensitivity,
                _settingsViewModel.RotationStep,
                _settingsViewModel.MovementStep
                );
            _settingsViewModel.IsSaving = false;
            _navigationService.Navigate();
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == nameof(SettingsViewModel.CanSave))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
