using MapViewer.Core.Models;
using MapViewer.Core.ViewModels;
using System.ComponentModel;

namespace MapViewer.Core.Commands
{
    /// <summary>
    /// Command reseting Viewport3D camera to the default position.
    /// </summary>
    class ResetViewCommand : BaseCommand
    {
        /// <summary>
        /// MapViewModel bounded to the Viewport3D.
        /// </summary>
        private readonly MapViewModel _mapViewModel;

        public ResetViewCommand(MapViewModel mapViewModel)
        {
            _mapViewModel = mapViewModel;
            _mapViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override bool CanExecute(object? parameter)
        {
            return _mapViewModel.Camera is not null && _mapViewModel.Map is not null && base.CanExecute(parameter);
        }

        public override void Execute(object? parameter)
        {
            if (_mapViewModel.Camera is not null && _mapViewModel.Map is not null)
            {
                _mapViewModel.Camera = new Camera(
                    _mapViewModel.Map.Data.Width,
                    _mapViewModel.Map.Data.XCenter,
                    _mapViewModel.Map.Data.YCenter,
                    _mapViewModel.Map.Data.MaxAltitude);
                _mapViewModel.Circle = null;
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
