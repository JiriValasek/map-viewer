using MapViewer.Core.Commands;
using MapViewer.Core.Models;
using MapViewer.Core.Utils;
using MapViewer.Core.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Wpf.Commands
{
    /// <summary>
    /// Command for map saving.
    /// </summary>
    public class SaveMapCommand : BaseAsyncCommand
    {
        private readonly MapViewModel _mapViewModel;

        /// <summary>
        /// Command for map loading.
        /// </summary>
        /// <param name="mapViewModel">ViewModel for the map view.</param>
        public SaveMapCommand(MapViewModel mapViewModel)
        {
            _mapViewModel = mapViewModel;
            _mapViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _mapViewModel.Map is not null && base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            SaveFileDialog saveDialog = new()
            {
                Filter = null,
                RestoreDirectory = true,
                OverwritePrompt = true,
                CheckPathExists = true,
                ValidateNames = true,
                AddToRecent = true
            };

            if (saveDialog.ShowDialog() == true && _mapViewModel.Map is not null)
            {
                await MapFileUtils.SaveMapAsync(saveDialog.FileName, _mapViewModel.Map.Data);
            }
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MapViewModel.Map))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
